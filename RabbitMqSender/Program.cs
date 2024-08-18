// See https://aka.ms/new-console-template for more information
using System.Text;
using RabbitMQ.Client;

Console.WriteLine("Hello, World! Sender.");

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "hello",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

const string message = "Hello World!";
var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(exchange: string.Empty,
    routingKey: "hello",
    basicProperties: null,
    body: body);
Console.WriteLine($" [x] Sent {message}");



string queueName = "temperatureResponseQueue";
using var channel2 = connection.CreateModel();

channel2.QueueDeclare(queue: queueName,
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

const string message2 = "{\n  \"sensorId\": \"sensor-12345\",\n  \"location\": \"Living Room\",\n  \"timestamp\": \"2024-08-15T14:30:00Z\",\n  \"temperature\": {\n    \"value\": 22.5,\n    \"unit\": \"Celsius\"\n  },\n  \"humidity\": {\n    \"value\": 55,\n    \"unit\": \"%\"\n  }\n}\n";
var body2 = Encoding.UTF8.GetBytes(message2);

channel2.BasicPublish(exchange: string.Empty,
    routingKey: queueName,
    basicProperties: null,
    body: body2);
Console.WriteLine($" [x] Sent {message2}");

// Console.WriteLine(" Press [enter] to exit.");
// Console.ReadLine();