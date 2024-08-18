using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

const string RequestQueueName = "temperatureRequestQueue"; 
const string ResponseQueueName = "temperatureResponseQueue";

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: RequestQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueDeclare(queue: ResponseQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

Console.WriteLine($" [*] Waiting for messages in {RequestQueueName}.");
// Thread.Sleep(5000);
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    // Thread.Sleep(5000);
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var correlationId = ea.BasicProperties.CorrelationId;
    Console.WriteLine($" [x] Received message: {message} with CorrelationId: {correlationId}");

    var random = new Random();
    double temperature = random.NextDouble() * 10000;
    double humidity = random.NextDouble() * 10000;

    var responseMessage = new
    {
        temperature,
        humidity
    };
    var jsonResponse = JsonConvert.SerializeObject(responseMessage, Formatting.Indented);
    var responseBody = Encoding.UTF8.GetBytes(jsonResponse);

    // Create properties for the response message
    var replyProps = channel.CreateBasicProperties();
    replyProps.CorrelationId = correlationId;
    // Thread.Sleep(5000);
    // Publish the response message
    channel.BasicPublish(exchange: string.Empty,
                         routingKey: ResponseQueueName,
                         basicProperties: replyProps,
                         body: responseBody);
    Console.WriteLine($" [x] Sent response: {responseMessage} with CorrelationId: {correlationId}");

    // Manually acknowledge the message after processing
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};
// Thread.Sleep(5000);
channel.BasicConsume(queue: RequestQueueName, autoAck: false, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
