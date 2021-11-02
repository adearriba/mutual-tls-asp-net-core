# Mutual TLS using ASP.NET Core

ASP.NET Core example for working in a mTLS (2-way-SSL) project.

## Server configuration

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
