﻿using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Services;
using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Banking.Data.Repository;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Bus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using MicroRabbit.Banking.Domain.Commands;
using MicroRabbit.Banking.Domain.EventHandlers;
using MicroRabbit.Transfer.Application.Interfaces;
using MicroRabbit.Transfer.Application.Services;
using MicroRabbit.Transfer.Domain.Interfaces;
using MicroRabbit.Transfer.Data.Repository;
using MicroRabbit.Transfer.Data.Context;

namespace MicroRabbit.Infra.IoC;

public static class DependencyContainer
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddTransient<IRequestHandler<CreateTransferCommand, bool>, TransferCommandHandler>();
        // MediatR Mediator
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Domain Bus
        services.AddTransient<IEventBus, RabbitMQBus>();
        
        // // Application Services
        // services.AddTransient<IAccountService, AccountService>();
        // services.AddTransient<ITransferService, TransferService>();

        // // Data
        // services.AddTransient<IAccountRepository, AccountRepository>();
        // services.AddTransient<BankingDbContext>();
        // services.AddTransient<ITransferRepository, TransferRepository>();
        // services.AddTransient<TransferDbContext>();

        return services;
    }
}
