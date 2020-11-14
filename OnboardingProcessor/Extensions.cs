﻿using Microsoft.Extensions.DependencyInjection;
using OnboardingMessages;
using Rebus.Config;
using Rebus.Persistence.FileSystem;
using Rebus.Persistence.InMem;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Transport.FileSystem;

namespace OnboardingProcessor
{
    public static class Extensions
    {
        public static void AddRebusAsSendAndReceive(this IServiceCollection services)
        {
            services.AddRebus(
                cfg => cfg
                   .Logging(l => l.Serilog())
                   .Routing(r => r.TypeBased().MapAssemblyOf<OnboardNewCustomer>("MainQueue"))
                   .Transport(t => t.UseFileSystem("c:/rebus-advent", "MainQueue"))
                   .Options(t => t.SimpleRetryStrategy(errorQueueAddress: "ErrorQueue"))
                   .Subscriptions(s => s.UseJsonFile("c:/rebus-advent/subscriptions.json"))
                   .Sagas(s => s.UseFilesystem("c:/rebus-advent/sagas"))
                );

            services.AutoRegisterHandlersFromAssemblyOf<Backend>();
        }
    }
}