using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using ASP.NET_WebAPI6.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
 


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEntityFrameworkSqlServer()
    .AddDbContext<DBContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
builder.Services.AddControllers();


ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
{
    // Load the trusted CA certificate from file
    //string trustedCACertificatePath = "path/to/trusted-ca-certificate.crt";
    string trustedCACertificatePath = "./cert.crt";
    X509Certificate2 trustedCACertificate = new X509Certificate2(trustedCACertificatePath);

    chain.ChainPolicy.ExtraStore.Add(trustedCACertificate);
    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreCertificateAuthorityRevocationUnknown |
                                          X509VerificationFlags.IgnoreCtlSignerRevocationUnknown |
                                          X509VerificationFlags.IgnoreEndRevocationUnknown |
                                          X509VerificationFlags.IgnoreRootRevocationUnknown;

    bool isValid = chain.Build((X509Certificate2)certificate);

    return isValid;
};

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
