# RabbitMQ
docker run -d --hostname rabbit-server --name rabbit-selvin-web -p 5672:5672 -p 15672:15672 rabbitmq:management

# Sql Server
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Administrator123$' -p 1433:1433 -d --name sqlserverselvin mcr.microsoft.com/mssql/server