namespace CategoryService.Infrastructure.Messaging.RabbitMQ
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = "localhost";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string CategoryQueueName { get; set; } = "category_queue";
    }
}
