// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using System.Threading.Tasks;
using gRPC_RateLimiter_Client;
using Google.Protobuf.WellKnownTypes;


Console.WriteLine("Hello, gRPC World!");
// The port number must match the port of the gRPC server.
using var channel = GrpcChannel.ForAddress("https://localhost:7232");
var client = new Greeter.GreeterClient(channel);

var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
Console.WriteLine("Greeting: " + reply.Message);


var reply2 = await client.SayHelloListAsync(new HelloRequest { Name = "GreeterClient" });
Console.WriteLine("Greeting: " + reply2.Messages);

#region Test Token Bucket
var replydict1 = await client.SayHelloDictionaryAsync(new HelloRequest { Name = "GreeterClient" });
Console.WriteLine("Greeting: " + replydict1.Messages);

var replydict2 = await client.SayHelloDictionaryAsync(new HelloRequest { Name = "GreeterClient" });
Console.WriteLine("Greeting: " + replydict2.Messages);
#endregion Test Token Bucket



#region Test Sliding Window
var tasks = new List<Task<HelloReply>>();
for (int i = 0; i < 5; i++)
{
    tasks.Add(client.SayHelloSlidingAsync(new HelloRequest { Name = $"GreeterClient{i}" }).ResponseAsync);
}
var responses = await Task.WhenAll(tasks);
foreach (var res in responses)
{
    Console.WriteLine($"Greeting: {res.Message}");
}

var res2 = await client.SayHelloSlidingAsync(new HelloRequest { Name = $"GreeterClient{5}" });
Console.WriteLine($"Greeting: {res2.Message}");
#endregion Test Sliding Window


#region Test Concurrency error
var tasks2 = new List<Task<HelloReplyList>>();
for (int i = 0; i < 7; i++)
{
    tasks2.Add(client.SayHelloListAsync(new HelloRequest { Name = $"GreeterClient{i}" }).ResponseAsync);
}
var responses2 = await Task.WhenAll(tasks2);
foreach (var res in responses2)
{
    Console.WriteLine($"Greeting: {res.Messages}");
}
#endregion Test Concurrency error






Console.WriteLine("Press any key to exit...");
Console.ReadKey();
