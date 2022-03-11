using System.Text;
using Commons.Models;
using Prometheus;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WorkerMicroservice.Repositories.Cache;
using WorkerMicroservice.Services.Post;

namespace WorkerMicroservice.Services.Queue
{
    public class PostQueueService : IPostQueueService
    {

        private static readonly Gauge PostInProgress = Metrics.CreateGauge("post_queue_in_progress", "Post execution in progress");
        private static readonly Counter PostCounter = Metrics.CreateCounter("post_queue_counter", "Count the post execution");
        private static readonly Histogram PostDuration = Metrics.CreateHistogram("post_queue_duration", "Measure the duration of the post execution process");

        private readonly IPostPersonService _postPersonService;
        private readonly ICacheRepository _cacheRepository;

        public PostQueueService(IPostPersonService? postPersonService,
            ICacheRepository? cacheRepository)
        {
            this._postPersonService = postPersonService!;
            this._cacheRepository = cacheRepository!;
        }

        public void StartListening()
        {
            var factory = new ConnectionFactory() { HostName = Environment.GetEnvironmentVariable("RABBIT_CONNECTION_STRING") };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "post",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    PostCounter.Inc();
                    using (PostDuration.NewTimer())
                    {
                        using (PostInProgress.TrackInProgress())
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            //Get the transaction object
                            PostPersonResponse transactionObject = Newtonsoft.Json.JsonConvert.DeserializeObject<PostPersonResponse>(message)!;
                            //Update processing state
                            transactionObject.Status = TransactionStatus.PROCESSING;
                            await this._cacheRepository.SaveObject(transactionObject.TransactionId.ToString(), transactionObject, 24);
                            //Starting the post process
                            Person? personCreated = null;
                            try
                            {
                                personCreated = await this._postPersonService.Post(transactionObject.Request!);
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
                channel.BasicConsume(queue: "post",
                                     autoAck: true,
                                     consumer: consumer);

            }
        }
    }
}

