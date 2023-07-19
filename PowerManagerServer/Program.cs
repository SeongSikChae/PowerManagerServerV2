
using CommandLine;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Configuration;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace PowerManagerServer
{
    public class Program
    {
        public sealed class CmdMain
        {
            [Option("config", Required = true, HelpText = "config file path")]
            public string ConfigFilePath { get; set; } = string.Empty;

            [Option("log", Required = true, HelpText = "log dir path")]
            public string LogDirPath { get; set; } = string.Empty;

            [Option("debug", Required = false, HelpText = "debug mode")]
            public bool DebugMode { get; set; }
        }

        public static async Task Main(string[] args)
        {
            ParserResult<CmdMain> result = await Parser.Default.ParseArguments<CmdMain>(args).WithParsedAsync(async cmdMain =>
            {
                YamlDotNet.Serialization.Deserializer deserializer = new YamlDotNet.Serialization.Deserializer();
                Configuration configuration = deserializer.Deserialize<Configuration>(File.ReadAllText(cmdMain.ConfigFilePath));

                var app = CreateApplication(args, cmdMain, configuration);
                await app.RunAsync();
            });

            await result.WithNotParsedAsync(async errors =>
            {
                if (errors.IsVersion())
                    PrintVersion(errors.Output());
                await Task.CompletedTask;
            });
        }

        public static void PrintVersion(TextWriter? writer)
        {
            writer?.WriteLine($"PowerManagerLib: {RevisionUtil.GetRevisoin<PowerManagerLib.RevisionAttribute>()}");
            writer?.WriteLine($"PowerManagerServer: {RevisionUtil.GetRevisoin<PowerManagerServer.RevisionAttribute>()}");
        }

        public static WebApplication CreateApplication(string[] args, CmdMain cmdMain, Configuration configuration)
        {
            ConfigurationValidator.Validate(configuration);
            Environment.SetEnvironmentVariable("log.dir", cmdMain.LogDirPath);
            Environment.SetEnvironmentVariable("log.file", "server.log");

            var builder = WebApplication.CreateBuilder(args);
            builder.Host
            .UseSystemd()
            .UseWindowsService(configure =>
            {
                configure.ServiceName = "PowerManagerServer";
            });

            builder.WebHost.UseKestrel(options =>
            {
                options.ConfigureEndpointDefaults(options =>
                {
                    if (configuration.HTTP2)
                        options.Protocols = HttpProtocols.Http1AndHttp2;
                    else
                        options.Protocols = HttpProtocols.Http1;
                });
                options.ListenAnyIP(configuration.WebHttpPort);
                options.ListenAnyIP(configuration.WebHttpsPort, configure =>
                {
                    ConfigureListenOptions(configure, configuration.ServerCertificate, configuration.ServerCertificatePassword, configuration.IncludeCipherSuites);
                });
                options.ListenAnyIP(configuration.HttpApiPort);
                options.ListenAnyIP(configuration.ApiPort, configure =>
                {
                    ConfigureListenOptions(configure, configuration.ServerCertificate, configuration.ServerCertificatePassword, configuration.IncludeCipherSuitesForApi);
                });
            }).ConfigureLogging(configure =>
            {
                configure.SetMinimumLevel(cmdMain.DebugMode ? LogLevel.Debug : LogLevel.Information);
            });

            var startup = new Startup(builder.Configuration, cmdMain, configuration);
            startup.ConfigureServices(builder.Services);

            var app = builder.Build();
            startup.Configure(app);
            return app;
        }

        private static void ConfigureListenOptions(ListenOptions configure, string serverCertificate, string serverCertificatePassword, List<string> includeCipherSuites)
        {
            configure.UseHttps(options =>
            {
                options.ClientCertificateValidation = (certificate, chain, errors) =>
                {
                    if (errors == SslPolicyErrors.None)
                        return true;
                    else return false;
                };
                options.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                options.ServerCertificate = new X509Certificate2(new FileInfo(serverCertificate).FullName, serverCertificatePassword);
                options.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                if (includeCipherSuites is not null && includeCipherSuites.Count > 0)
                {
                    options.OnAuthenticate = (connectionContext, authenticationOptions) =>
                    {
                        List<TlsCipherSuite> tlsCipherSuites = new List<TlsCipherSuite>();
                        foreach (string cipherSuiteStr in includeCipherSuites)
                        {
                            if (Enum.TryParse(cipherSuiteStr, out TlsCipherSuite tlsCipherSuite))
                                tlsCipherSuites.Add(tlsCipherSuite);
                        }

                        if (tlsCipherSuites.Count > 0)
                            authenticationOptions.CipherSuitesPolicy = new CipherSuitesPolicy(tlsCipherSuites);
                    };
                }
            });
        }
    }
}