
# Mutual TLS using ASP.NET Core

ASP.NET Core example for working in a mTLS (2-way-SSL) project.

## Server configuration [ MutualTLS.API ]

Inside of `Program.cs`, configure `ClientCertificateMode` to `RequireCertificate`.
```csharp
webBuilder.ConfigureKestrel( options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
    });
});
```

In `Startup.cs`, add the certificate authentication configuration in `ConfigureServices`. In this code, for having the logic in a separate file, I created an extension method named: `AddCertificateAuthentication`.
```csharp
services.AddCertificateAuthentication();
```

The actual code for `AddCertificateAuthentication` is in `MutualTLS.API/Extensions/AddCertificateAuthenticationExtension.cs`:
```csharp
public static IServiceCollection AddCertificateAuthentication(this IServiceCollection services)
{
  // Add dependency injection for certificate validator
  services.AddTransient<ICertificateValidationService, CertificateValidationService>();

  // Configure the actual certificate authentication
  services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
        .AddCertificate( options =>
        {
          // All will allow "chained" and "sef-signed" certificates. Please configure as desired.
          options.AllowedCertificateTypes = CertificateTypes.All;

            // OnCertificateValidated lets you add additional validation which is using the correct service by dependency injection
            options.Events = new CertificateAuthenticationEvents
            {
                OnCertificateValidated = context =>
                {
                    var validationService = context.HttpContext.RequestServices
                    .GetRequiredService<ICertificateValidationService>();

                    if (!validationService.IsValid(context.ClientCertificate))
                    {
                        context.Fail("Invalid certificate.");
                        return Task.CompletedTask;
                    }

                    context.Success();
                    return Task.CompletedTask;
                }
            };
        })
        //This will add a memory cache
        .AddCertificateCache();

    return services;
}
```

At last, please add Authenticatino and Authorization to `Startup.cs` in `Configure` method:
```csharp
app.UseAuthentication();
app.UseAuthorization();
```
## Client [ MutualTLS.Client ]

First of all, you need to create a certificate. You can use openssl to do so:
```chsarp
openssl req -x509 -sha256 -nodes -newkey rsa:2048 -days 365 -keyout localhost-mtls.key -out localhost-mtls.crt
```

**Note**: You need to install the public key (`.crt` file) in the server trust store.

Transform `.crt` and `.key` file to a `.pfx` file. 
```
openssl pkcs12 -export -out localhost-mtls.pfx -inkey localhost-mtls.key -in localhost-mtls.crt
```

Include the `.pfx` file in your project or add it to your key vault.

To use the certificate inside your request, you could create a `Client` and reuse it for all the requests. This client will encapsulate the certificate. In this case I created a very  dummy client as an example `ClientWithCertificate.cs`
```csharp
public ClientWithCertificate()
{
    _client = new RestClient("https://localhost:5001");
    
    // Get the certificate from a file or from Key vault
    var currentDirectory = Directory.GetCurrentDirectory();
    var pfx = $"{currentDirectory}\\Certificates\\localhost-mtls.pfx";
    var cert = new X509Certificate2(pfx);

	// Add the certificate in the client
    _client.ClientCertificates = new X509CertificateCollection { cert };
}
```
That would be all.

**Server logs:**
```
info: MutualTLS.API.Controllers.MTLSController[0]
      [GET /MTLS] - Called by: LOCALHOST MTLS
```

**Client logs:**
```
With Certificate: Status code OK
Without Certificate: Status code 0
```
