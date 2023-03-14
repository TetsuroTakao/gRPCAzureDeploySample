using GrpcGreeter.Services;

using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.Authentication.Certificate;

HttpClientHandler handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(options =>
    {
        options.AllowedCertificateTypes = CertificateTypes.All;
    });
builder.Services.AddAuthorization();
builder.WebHost.ConfigureKestrel(options => 
{ 
    options.ListenAnyIP(5008); 
    options.ListenAnyIP(7116, listenOptions => 
    { 
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
        listenOptions.UseHttps(options =>
            {
                options.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                options.ClientCertificateValidation = (certificate, chain, errors) => { 
                    options.ServerCertificate = certificate;
                    return true; 
                };
            }
        );
    });
});
builder.Services.AddGrpcReflection();
builder.Services.AddGrpc();
var app = builder.Build();
IWebHostEnvironment env = app.Environment;
if (env.IsDevelopment())
{
    app.MapGrpcReflectionService();
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
