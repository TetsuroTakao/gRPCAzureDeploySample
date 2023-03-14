using Grpc.Net.Client;
using GrpcGreeterClient;
using System.Security.Cryptography.X509Certificates;

using System.Text;

var httpHandler = new HttpClientHandler();
X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
var cert = store.Certificates.Where(c => c.GetCertHashString() =="E73488EF2CD742DE44A31ECA660F9FD8AD3D46C6").FirstOrDefault();
httpHandler.ClientCertificates.Add(cert!);
using var channel = GrpcChannel.ForAddress("https://grpcsample.azurewebsites.net:5243",new GrpcChannelOptions { HttpHandler = httpHandler });
var client = new Greeter.GreeterClient(channel);
var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine(string.Format("{0}, Press any key to exit...",cert!.FriendlyName));
Console.ReadKey();

