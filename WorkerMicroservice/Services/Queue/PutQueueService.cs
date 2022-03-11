using System.Text;
using Commons.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WorkerMicroservice.Repositories.Cache;
using WorkerMicroservice.Services.Put;
using Prometheus;

namespace WorkerMicroservice.Services.Queue
{
    public class PutQueueService : IPutQueueService
    {

        private static readonly Gauge PutInProgress = Metrics.CreateGauge("put_queue_in_progress", "Put execution in progress");
        private static readonly Counter PutCounter = Metrics.CreateCounter("put_queue_counter", "Count the put execution");
        private static readonly Histogram PutDuration = Metrics.CreateHistogram("put_queue_duration", "Measure the duration of the put execution process");

        private readonly IPutPersonService _putPersonService;
        private readonly ICacheRepository _cacheRepository;

        public PutQueueService(IPutPersonService? putPersonService,
            ICacheRepository? cacheRepository)
        {
            this._putPersonService = putPersonService!;
            this._cacheRepository = cacheRepository!;
        }

        public void StartListening()
        {
            var factory = new ConnectionFactory() { HostName = Environment.GetEnvironmentVariable("RABBIT_CONNECTION_STRING") };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "put",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {

                    PutCounter.Inc();
                    using (PutDuration.NewTimer())
                    {
                        using (PutInProgress.TrackInProgress())
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            //Get the transaction object
                            PutPersonResponse transactionObject = Newtonsoft.Json.JsonConvert.DeserializeObject<PutPersonResponse>(message)!;
                            //Update processing state
                            transactionObject.Status = TransactionStatus.PROCESSING;
                            await this._cacheRepository.SaveObject(transactionObject.TransactionId.ToString(), transactionObject, 24);
                            //Starting the post process
                            Person? personCreated = null;
                            try
                            {
                                personCreated = await this._putPersonService.Put(transactionObject.PersonId, transactionObject.Request!);
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
                            if(personCreated != null)
                            {
                                transactionObject.Response = personCreated;
                                transactionObject.Status = TransactionStatus.COMPLETED;
                                await this._cacheRepository.SaveObject(transactionObject.TransactionId.ToString(), transactionObject, 24);
                            }
                        }
                    }
                };
                channel.BasicConsume(queue: "put",
                                     autoAck: true,
                                     consumer: consumer);

            }
        }
    }
}

