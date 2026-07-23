
using CommandLine;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using MQTTnet.AspNetCore;
using PowerManager.Server.Context;
using PowerManager.Server.Context.Entity;
using PowerManager.Server.Context.Store;
using PowerManager.Server.Controllers;
using PowerManager.Server.Handlers;
using Serilog;
using Serilog.Configuration;
using System.Configuration;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace PowerManager.Server
{
    public class Program
    {
        public sealed class CmdMain
        {
            [Option("config", Required = true, HelpText = "config file path")]
            public string ConfigFilePath { get; set; } = null!;

            [Option("log", Required = true, HelpText = "log dir path")]
            public string LogDirPath { get; set; } = null!;

            [Option("spaDevServerUrl", Default = "http://localhost:5173", HelpText = "ProxyToSpaDevelopmentServerUrl")]
            public string SpaDevServerUrl { get; set; } = null!;
        }

        public static async Task Main(string[] args)
        {
            ParserResult<CmdMain> result = await Parser.Default.ParseArguments<CmdMain>(args)
                .WithParsedAsync(async cmdMain =>
                {
                    YamlDotNet.Serialization.Deserializer deserializer = new YamlDotNet.Serialization.Deserializer();
                    FileInfo configFileInfo = new FileInfo(cmdMain.ConfigFilePath);
                    string str = File.ReadAllText(configFileInfo.FullName);
                    Configuration? configuration = deserializer.Deserialize<Configuration>(str);
                    ConfigurationValidator.Validate(configuration);
                    WebApplication app = CreateWebApplication(args, cmdMain, configuration);
                    await StartAsync(app, cmdMain, configuration);
                });
        }

        private static WebApplication CreateWebApplication(string[] args, CmdMain cmdMain, Configuration configuration)
        {
            X509Certificate2 certificate = X509CertificateLoader.LoadPkcs12FromFile(
                new FileInfo(Path.Combine(configuration.ContentRootPath, configuration.ServerCertificate)).FullName,
                configuration.ServerCertificatePassword
            );

            WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = configuration.ContentRootPath,
                WebRootPath = configuration.WebRootPath,
            });

            builder.Logging.Services.AddSerilog(configureLogger =>
            {
                configureLogger
                    .MinimumLevel.Information()
                    .Enrich.WithCaller()
                    .WriteTo.Console(Serilog.Events.LogEventLevel.Information, CallerEnricherOutputTemplate.Default)
                    .WriteTo.File(
                        new DirectoryInfo(cmdMain.LogDirPath).FullName,
                        "powermanager.log",
                        Serilog.Events.LogEventLevel.Information,
                        CallerEnricherOutputTemplate.Default,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 12
                    );
            });

            builder.WebHost.UseKestrel(options =>
            {
                if (configuration.WebHttpPort.HasValue)
                    options.ListenAnyIP(configuration.WebHttpPort.Value);
                if (configuration.WebHttpsPort.HasValue)
                {
                    options.ListenAnyIP(configuration.WebHttpsPort.Value, configure =>
                    {
                        configure.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                        configure.UseHttps(httpsOptions =>
                        {
                            httpsOptions.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.AllowCertificate;
                            httpsOptions.ServerCertificate = certificate;

                            if (configuration.CertificateChain is not null)
                            {
                                X509Certificate2Collection collection = X509CertificateLoader.LoadPkcs12CollectionFromFile(
                                    new FileInfo(configuration.ServerCertificate).FullName,
                                    configuration.ServerCertificatePassword
                                );

                                Dictionary<string, X509Certificate2> certificates = new Dictionary<string, X509Certificate2>();
                                foreach (X509Certificate2 certificate in collection.Where(cert => !cert.HasPrivateKey))
                                {
                                    string name = certificate.GetNameInfo(X509NameType.SimpleName, false);
                                    certificates.Add(name, certificate);
                                }

                                string[] certificateChain =
                                    configuration.CertificateChain.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                                httpsOptions.ServerCertificateChain = new X509Certificate2Collection();

                                foreach (string name in certificateChain)
                                {
                                    if (certificates.TryGetValue(name, out X509Certificate2? certificate))
                                        httpsOptions.ServerCertificateChain.Add(certificate);
                                }
                            }

                            httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
                            if (configuration.IncludeCipherSuites is not null)
                            {
                                string[] includeCipherSuites =
                                configuration.IncludeCipherSuites.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                                if (includeCipherSuites.Length > 0)
                                {
                                    httpsOptions.OnAuthenticate = (connectionContext, authenticationOptions) =>
                                    {
                                        authenticationOptions.AllowRenegotiation = false;
                                        HashSet<TlsCipherSuite> tlsCipherSuites = new HashSet<TlsCipherSuite>();
                                        foreach (string cipherSuite in includeCipherSuites)
                                        {
                                            if (Enum.TryParse(cipherSuite, out TlsCipherSuite tlsCipherSuite))
                                                tlsCipherSuites.Add(tlsCipherSuite);
                                        }


                                        if (tlsCipherSuites.Count > 0)
                                            authenticationOptions.CipherSuitesPolicy = new CipherSuitesPolicy(tlsCipherSuites);
                                    };
                                }
                            }
                        });
                    });
                }

                if (configuration.MqttPort.HasValue)
                {
                    options.ListenAnyIP(configuration.MqttPort.Value, configure =>
                    {
                        configure.UseMqtt();
                    });
                }

                if (configuration.MqttsPort.HasValue)
                {
                    options.ListenAnyIP(configuration.MqttsPort.Value, configure =>
                    {
                        configure.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                        configure.UseHttps(httpsOptions =>
                        {
                            httpsOptions.ServerCertificate = certificate;

                            if (configuration.CertificateChain is not null)
                            {
                                X509Certificate2Collection collection = X509CertificateLoader.LoadPkcs12CollectionFromFile(
                                    new FileInfo(configuration.ServerCertificate).FullName,
                                    configuration.ServerCertificatePassword
                                );

                                Dictionary<string, X509Certificate2> certificates = new Dictionary<string, X509Certificate2>();
                                foreach (X509Certificate2 certificate in collection.Where(cert => !cert.HasPrivateKey))
                                {
                                    string name = certificate.GetNameInfo(X509NameType.SimpleName, false);
                                    certificates.Add(name, certificate);
                                }

                                string[] certificateChain =
                                    configuration.CertificateChain.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                                httpsOptions.ServerCertificateChain = new X509Certificate2Collection();

                                foreach (string name in certificateChain)
                                {
                                    if (certificates.TryGetValue(name, out X509Certificate2? certificate))
                                        httpsOptions.ServerCertificateChain.Add(certificate);
                                }
                            }

                            httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
                            if (configuration.IncludeCipherSuites is not null)
                            {
                                string[] includeCipherSuites =
                                configuration.IncludeCipherSuites.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                                if (includeCipherSuites.Length > 0)
                                {
                                    httpsOptions.OnAuthenticate = (connectionContext, authenticationOptions) =>
                                    {
                                        authenticationOptions.AllowRenegotiation = false;
                                        HashSet<TlsCipherSuite> tlsCipherSuites = new HashSet<TlsCipherSuite>();
                                        foreach (string cipherSuite in includeCipherSuites)
                                        {
                                            if (Enum.TryParse(cipherSuite, out TlsCipherSuite tlsCipherSuite))
                                                tlsCipherSuites.Add(tlsCipherSuite);
                                        }


                                        if (tlsCipherSuites.Count > 0)
                                            authenticationOptions.CipherSuitesPolicy = new CipherSuitesPolicy(tlsCipherSuites);
                                    };
                                }
                            }
                        });
                        configure.UseMqtt();
                    });
                }

                options.ConfigureEndpointDefaults(configureOptions =>
                {
                    configureOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                });
            });

            //// Add services to the container.

            builder.Services.AddSystemd();
            builder.Services.AddWindowsService();
            
            builder.Services.AddHostedMqttServer(options =>
            {
                options.WithConnectionBacklog(configuration.MqttServerBacklog!.Value);
                options.WithoutDefaultEndpoint();
            });
            builder.Services.AddMqttConnectionHandler();
            builder.Services.AddConnections();

            builder.Services.AddDbContextFactory<PowerManagerContext>(builder =>
            {
                FileInfo dbPath = new FileInfo(Path.Combine(configuration.ContentRootPath, configuration.DbPath));
                DirectoryInfo? directory = dbPath.Directory;
                if (directory is not null && !directory.Exists)
                    directory.Create();
                builder.UseSqlite($"Data Source={dbPath.FullName}");
                using PowerManagerContext context = new PowerManagerContext((DbContextOptions<PowerManagerContext>)builder.Options);
                context.Database.Migrate();
            });
            
            builder.Services.AddSingleton(configuration);
            builder.Services.AddSingleton<MqttController>();
            builder.Services.AddSingleton<IReadStore<PowerManagerContext, ElectricPower>, ElectricPowerStore>();
            builder.Services.AddSingleton<IStore<PowerManagerContext, Device>, DeviceStore>();
            builder.Services.AddSingleton<IStore<PowerManagerContext, DeviceApi>, DeviceApiStore>();
            builder.Services.AddControllers();
            //// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSpaStaticFiles(configure =>
            {
                configure.RootPath = configuration.WebRootPath;
            });
            builder.Services.AddHsts(configureOptions =>
            {
                configureOptions.Preload = true;
                configureOptions.IncludeSubDomains = true;
                configureOptions.MaxAge = TimeSpan.FromDays(365);
            });
            builder.Services.AddHttpsRedirection(configureOptions =>
            {
                if (configuration.WebHttpsPort.HasValue)
                    configureOptions.HttpsPort = configuration.WebHttpsPort.Value;
            });
            builder.Services.AddAuthentication(configureOptions =>
            {
                configureOptions.DefaultAuthenticateScheme = "Certificate";
            }).AddCertificate("Certificate", configureOptions =>
            {
                configureOptions.AllowedCertificateTypes = Microsoft.AspNetCore.Authentication.Certificate.CertificateTypes.All;
                configureOptions.RevocationMode = X509RevocationMode.NoCheck;
                configureOptions.Events = new Microsoft.AspNetCore.Authentication.Certificate.CertificateAuthenticationEvents
                {
                    OnCertificateValidated = context =>
                    {
                        var cert = context.ClientCertificate;
                        if (cert is null)
                        {
                            context.Fail(new AuthenticationException());
                            return Task.CompletedTask;
                        }

                        context.Success();
                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("Certificate", policy =>
                {
                    policy.AddAuthenticationSchemes("Certificate");
                    policy.RequireAuthenticatedUser();
                });
            builder.Services.AddExceptionHandler<RestExceptionHandler>();
            builder.Services.AddProblemDetails();

            return builder.Build();
        }

        private static async Task StartAsync(WebApplication app, CmdMain cmdMain, Configuration configuration)
        {
            // Must be early so controller/pipeline exceptions are caught.
            app.UseExceptionHandler();

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseWhen(context => context.Connection.LocalPort == configuration.WebHttpPort, configure =>
            {
                configure.UseHttpsRedirection();
            });
            app.UseHsts();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapWhen(
                    context =>
                        !context.Request.Path.StartsWithSegments("/api") &&
                        !context.Request.Path.StartsWithSegments("/rest") &&
                        !context.Request.Path.StartsWithSegments("/dawondns") &&
                        !context.Request.Path.StartsWithSegments("/HA"),
                    app => app.UseSpa(spa => spa.UseProxyToSpaDevelopmentServer(cmdMain.SpaDevServerUrl))
                );
            }
            else
            {
                app.MapFallbackToFile("index.html");
            }

            MqttController mqttController = app.Services.GetRequiredService<MqttController>();

            app.MapMqtt("/dwd");
            app.UseMqttServer(configure =>
            {
                configure.ValidatingConnectionAsync += mqttController.ValidatingConnectionAsync;
                configure.ClientConnectedAsync += mqttController.ClientConnectedAsync;
                configure.InterceptingPublishAsync += mqttController.InterceptingPublishAsync;
                configure.ClientDisconnectedAsync += mqttController.ClientDisconnectedAsync;
            });

            await app.RunAsync();
        }
    }
}
