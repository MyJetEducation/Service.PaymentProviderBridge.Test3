using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyJetWallet.Sdk.GrpcSchema;
using MyJetWallet.Sdk.Service;
using Prometheus;
using Service.PaymentProviderBridge.Test3.Modules;
using Service.Core.Client.Constants;
using Service.Grpc;
using Service.PaymentDeposit.Domain.Models;
using Service.PaymentProviderBridge.Test3.Services;
using SimpleTrading.ServiceStatusReporterConnector;

namespace Service.PaymentProviderBridge.Test3
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.BindGrpc();
            services.AddHostedService<ApplicationLifetimeManager>();
            services.AddMyTelemetry(Configuration.TelemetryPrefix, Program.Settings.ZipkinUrl);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseMetricServer();
            app.BindServicesTree(Assembly.GetExecutingAssembly());
            app.BindIsAlive();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcSchema<PaymentProviderGrpcService, IPaymentProviderGrpcService>();
                endpoints.MapGrpcSchemaRegistry();
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"); });
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<ServiceModule>();
        }
    }
}
