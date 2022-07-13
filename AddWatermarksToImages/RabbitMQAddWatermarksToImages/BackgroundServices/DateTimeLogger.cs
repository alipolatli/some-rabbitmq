namespace RabbitMQAddWatermarksToImages.BackgroundServices
{
    public class DateTimeLogger : BackgroundService
    {
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
        public override Task ExecuteTask => base.ExecuteTask;
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }

    //public class DateTimeLogger:IHostedService,IDisposable
    //{

    //    private int executionCount = 0;
    //    Timer? _timer= null;
    //    public DateTimeLogger()
    //    {
    //        int a = 4;
    //        ref int b = ref a;

    //    }

    //    public Task StartAsync(CancellationToken cancellationToken)
    //    {
    //        Console.WriteLine("Start async çalıştı.");
    //        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    //        return Task.CompletedTask;
    //    }
    //    public void DoWork(object? state)
    //    {
    //        var count = Interlocked.Increment(ref executionCount);
    //        Console.WriteLine($"dowork çalışıyor. count => {count}");
    //    }

    //    public Task StopAsync(CancellationToken cancellationToken)
    //    {
    //        Console.WriteLine("Stop async çalıştı.");
    //        _timer?.Change(Timeout.Infinite, 0);
    //        return Task.CompletedTask;
    //    }

    //    public void Dispose()
    //    {
    //        Console.WriteLine("Dispose ediliyor.");
    //        _timer?.Dispose();
    //        Console.WriteLine("Dispose edildi.");

    //    }
    //}
}
