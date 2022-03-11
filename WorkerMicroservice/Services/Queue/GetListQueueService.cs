using System.Text;
using Commons.Models;
using People.HttpMicroservice.Services.Get;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WorkerMicroservice.Repositories.Cache;
using Prometheus;

namespace WorkerMicroservice.Services.Queue
{
    public class GetListQueueService : IGetListQueueService
    {

        private static readonly Gauge GetInProgress = Metrics.CreateGauge("get_all_queue_in_progress", "Get all execution in progress");
        private static readonly Counter GetCounter = Metrics.CreateCounter("get_all_queue_counter", "Count the get all execution");
        private static readonly Histogram GetDuration = Metrics.CreateHistogram("get_all_queue_duration", "Measure the duration of the get all execution process");

        private readonly IGetPersonService _getPersonService;
        private readonly ICacheRepository _cacheRepository;

        public GetListQueueService(IGetPersonService? getPersonService,
            ICacheRepository? cacheRepository)
        {
            this._getPersonService = getPersonService!;
            this._cacheRepository = cacheRepository!;
        }

        public void StartListening()
        {
            var factory = new ConnectionFactory() { HostName = Environment.GetEnvironmentVariable("RABBIT_CONNECTION_STRING") };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "get-list",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    GetCounter.Inc();
                    using (GetDuration.NewTimer())
                    {
                        using (GetInProgress.TrackInProgress())
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            //Get the transaction object
                            GetPersonListResponse transactionObject = Newtonsoft.Json.JsonConvert.DeserializeObject<GetPersonListResponse>(message)!;
                            //Update processing state
                            transactionObject.Status = TransactionStatus.PROCESSING;
                            await this._cacheRepository.SaveObject(transactionObject.TransactionId.ToString(), transactionObject, 24);
                            //Starting the post process
                            List<Person>? personList = null;
                            try
                            {
                                personList = await this._getPersonService.Get();
                            }
                            catch (WorkerResponseException ex)
                            {
                                //Update the error status
                                transactionObject.Status = TransactionStatus.ERROR;
                                transactionObject.Error = ex;
                                await this._cacheRepository.SaveObject(transactionObject.TransactionId.ToString(), transactionObject, 24);
                            }
                            catch (Exception ex)
                            {
                                //Update the error status
                                transactionObject.Status = TransactionStatus.ERROR;
                                transactionObject.Error = new WorkerResponseException(500, "Internal server error", ex);
                                await this._cacheRepository.SaveObject(transactionObject.TransactionId.ToString(), transactionObject, 24);
                            }

                            //Update the succesful state
                            if(personList != null)
                            {
                                transactionObject.Response = personList;
                                transactionObject.Status = TransactionStatus.COMPLETED;
                                await this._cacheRepository.SaveObject(transactionObject.TransactionId.ToString(), transactionObject, 24);
                            }
                        }
                    }
                };
                channel.BasicConsume(queue: "get-list",
                                     autoAck: true,
                                     consumer: consumer);

            }
        }
    }
}

