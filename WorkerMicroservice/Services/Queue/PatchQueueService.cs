using System.Text;
using Commons.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WorkerMicroservice.Repositories.Cache;
using WorkerMicroservice.Services.Patch;
using Prometheus;

namespace WorkerMicroservice.Services.Queue
{
    public class PatchQueueService : IPatchQueueService
    {

        private static readonly Gauge PatchInProgress = Metrics.CreateGauge("patch_queue_in_progress", "Patch execution in progress");
        private static readonly Counter PatchCounter = Metrics.CreateCounter("patch_queue_counter", "Count the patch execution");
        private static readonly Histogram PatchDuration = Metrics.CreateHistogram("patch_queue_duration", "Measure the duration of the patch execution process");

        private readonly IPatchPersonService _patchPersonService;
        private readonly ICacheRepository _cacheRepository;

        public PatchQueueService(IPatchPersonService? patchPersonService,
            ICacheRepository? cacheRepository)
        {
            this._patchPersonService = patchPersonService!;
            this._cacheRepository = cacheRepository!;
        }

        public void StartListening()
        {
            var factory = new ConnectionFactory() { HostName = Environment.GetEnvironmentVariable("RABBIT_CONNECTION_STRING") };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "patch",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {

                    PatchCounter.Inc();
                    using (PatchDuration.NewTimer())
                    {
                        using (PatchInProgress.TrackInProgress())
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            //Get the transaction object
                            PatchPersonResponse transactionObject = Newtonsoft.Json.JsonConvert.DeserializeObject<PatchPersonResponse>(message)!;
                            //Update processing state
                            transactionObject.Status = TransactionStatus.PROCESSING;
                            await this._cacheRepository.SaveObject(transactionObject.TransactionId.ToString(), transactionObject, 24);
                            //Starting the post process
                            Person? personCreated = null;
                            try
                            {
                                personCreated = await this._patchPersonService.Patch(transactionObject.PersonId, transactionObject.Request!);
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
                channel.BasicConsume(queue: "patch",
                                     autoAck: true,
                                     consumer: consumer);

            }
        }
    }
}

