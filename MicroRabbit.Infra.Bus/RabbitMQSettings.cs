﻿namespace MicroRabbit.Infra.Bus;

public class RabbitMQSettings
{
    public string Hostname { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Port { get; set; }
}
