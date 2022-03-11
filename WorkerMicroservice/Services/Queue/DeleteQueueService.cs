using System.Text;
using Commons.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WorkerMicroservice.Repositories.Cache;
using WorkerMicroservice.Services.Delete;
using Prometheus;

namespace WorkerMicroservice.Services.Queue
{
    public class DeleteQueueService : IDeleteQueueService
    {

        private static readonly Gauge DeleteInProgress = Metrics.CreateGauge("delete_queue_in_progress", "Delete execution in progress");
        private static readonly Counter DeleteCounter = Metrics.CreateCounter("delete_queue_counter", "Count the delete execution");
        private static readonly Histogram DeleteDuration = Metrics.CreateHistogram("delete_queue_duration", "Measure the duration of the delete execution process");

        private readonly IDeletePersonService _deletePersonService;
        private readonly ICacheRepository _cacheRepository;

        public DeleteQueueService(IDeletePersonService? deletePersonService,
            ICacheRepository? cacheRepository)
        {
            this._deletePersonService = deletePersonService!;
            this._cacheRepository = cacheRepository!;
        }

        public void StartListening()
        {
            var factory = new ConnectionFactory() { HostName = Environment.GetEnvironmentVariable("RABBIT_CONNECTION_STRING") };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "delete",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    DeleteCounter.Inc();
                    using (DeleteDuration.NewTimer())
                    {
                        using (DeleteInProgress.TrackInProgress())
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            //Get the transaction object
                            DeletePersonResponse transactionObject = Newtonsoft.Json.JsonConvert.DeserializeObject<DeletePersonResponse>(message)!;
                            //Update processing state
                            transactionObject.Status = TransactionStatus.PROCESSING;
                            await this._cacheRepository.SaveObject(transactionObject.TransactionId.ToString(), transactionObject, 24);
                            //Starting the post process
                            bool deleted = false;
                            try
                            {
                                await this._deletePersonService.Delete(transactionObject.PersonId);
                                deleted = true;
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
                            if(deleted)
                            {
                                transactionObject.Status = TransactionStatus.COMPLETED;
                                await this._cacheRepository.SaveObject(transactionObject.TransactionId.ToString(), transactionObject, 24);
                            }
                        }
                    }
                };
                channel.BasicConsume(queue: "delete",
                                     autoAck: true,
                                     consumer: consumer);

            }
        }
    }
}

