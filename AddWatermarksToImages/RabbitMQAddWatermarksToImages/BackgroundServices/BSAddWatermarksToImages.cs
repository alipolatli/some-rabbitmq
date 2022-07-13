using RabbitMQAddWatermarksToImages.Services;

namespace RabbitMQAddWatermarksToImages.BackgroundServices
{
    public class BSAddWatermarksToImages:BackgroundService
    {
        readonly RabbitMQSubscriber _rabbitMQSubscriber;

        public BSAddWatermarksToImages(RabbitMQSubscriber rabbitMQSubscriber)
        {
            _rabbitMQSubscriber = rabbitMQSubscriber;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _rabbitMQSubscriber.ConfigConsumer();
            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMQSubscriber.Consume();
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
