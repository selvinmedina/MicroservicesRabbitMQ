﻿using System.Text;
using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MicroRabbit.Infra.Bus;

public sealed class RabbitMQBus : IEventBus
{
    private readonly RabbitMQSettings _settings;
    private readonly IMediator _mediator;
    private readonly Dictionary<string, List<Type>> _handlers;
    private readonly List<Type> _eventTypes;

    public RabbitMQBus(IMediator mediator, IOptions<RabbitMQSettings> settings)
    {
        _mediator = mediator;
        _handlers = new Dictionary<string, List<Type>>();
        _eventTypes = new List<Type>();
        _settings = settings.Value;
    }

    public void Publish<T>(T @event) where T : Event
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Hostname,
            UserName = _settings.UserName,
            Password = _settings.Password,
            Port = _settings.Port
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var eventName = @event.GetType().Name;

        channel.QueueDeclare(eventName, false, false, false, null);

        var message = JsonConvert.SerializeObject(@event);

        var body = Encoding.UTF8.GetBytes(message);
        
        channel.BasicPublish("", eventName, null, body);
    }

    public Task SendCommand<T>(T command) where T : Command
    {
        return _mediator.Send(command);
    }

    public void Subscribe<T, TH>()
        where T : Event
        where TH : IEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        if (!_eventTypes.Contains(typeof(T)))
        {
            _eventTypes.Add(typeof(T));
        }

        if (!_handlers.ContainsKey(eventName))
        {
            _handlers.Add(eventName, new List<Type>());
        }

        if (_handlers[eventName].Any(s => s.GetType() == handlerType))
        {
            throw new ArgumentException($"Handler Type {handlerType.Name} already is registered for '{eventName}'", nameof(handlerType));
        }

        _handlers[eventName].Add(handlerType);

        StartBasicConsume<T>();
    }

    private void StartBasicConsume<T>() where T : Event
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Hostname,
            UserName = _settings.UserName,
            Password = _settings.Password,
            DispatchConsumersAsync = true
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var eventName = typeof(T).Name;

        channel.QueueDeclare(eventName, false, false, false, null);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.Received += Consumer_Received;

        channel.BasicConsume(eventName, true, consumer);
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
    {
        var eventName = @event.RoutingKey;
        var message = Encoding.UTF8.GetString(@event.Body.Span);

        try
        {
            await ProcessEvent(eventName, message).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (_handlers.ContainsKey(eventName))
        {
            var subscriptions = _handlers[eventName];

            foreach (var subscription in subscriptions)
            {
                var handler = Activator.CreateInstance(subscription);

                if (handler == null) continue;

                var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                var @event = JsonConvert.DeserializeObject(message, eventType!);

                var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType!);

                await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });
            }
        }
    }
}
