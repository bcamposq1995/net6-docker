using Prometheus;
using WorkerMicroservice.Services.Queue;

namespace WorkerMicroservice;

public class Worker : BackgroundService
{
    private static readonly Gauge InProgress = Metrics.CreateGauge("worker_in_progress", "Workers execution in progress");

    private readonly IPostQueueService? _postQueueService;
    private readonly IPutQueueService? _putQueueService;
    private readonly IPatchQueueService? _patchQueueService;
    private readonly IDeleteQueueService? _deleteQueueService;
    private readonly IGetByIdQueueService? _getByIdQueueService;
    private readonly IGetListQueueService? _getListQueueService;

    public Worker(IPostQueueService postQueueService, IPutQueueService putQueueService,
        IPatchQueueService patchQueueService, IDeleteQueueService deleteQueueService,
        IGetByIdQueueService getByIdQueueService, IGetListQueueService getListQueueService)
    {
        this._postQueueService = postQueueService;
        this._putQueueService = putQueueService;
        this._patchQueueService = patchQueueService;
        this._deleteQueueService = deleteQueueService;
        this._getByIdQueueService = getByIdQueueService;
        this._getListQueueService = getListQueueService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (InProgress.TrackInProgress())
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                this._postQueueService!.StartListening();
                this._patchQueueService!.StartListening();
                this._putQueueService!.StartListening();
                this._deleteQueueService!.StartListening();
                this._getByIdQueueService!.StartListening();
                this._getListQueueService!.StartListening();
                await Task.Delay(100, stoppingToken);
            }
        }
    }

}

