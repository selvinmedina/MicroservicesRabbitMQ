# Use the official .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 80

# Copy the solution file and all projects
COPY ../../../MicroservicesRabbitMQ.sln ./
COPY ../../../MicroRabbit.Domain.Core ./MicroRabbit.Domain.Core
COPY ../../../MicroRabbit.Infra.Bus ./MicroRabbit.Infra.Bus
COPY ../../../MicroRabbit.Infra.IoC ./MicroRabbit.Infra.IoC
COPY ../../../Microservices/Banking ./Microservices/Banking
COPY ../../../Microservices/Transfer ./Microservices/Transfer

# Restore dependencies for the entire solution
RUN dotnet restore

# Publish the specific project (MicroRabbit.Banking.Api)
RUN dotnet publish ./Microservices/Banking/Api/MicroRabbit.Banking.Api/MicroRabbit.Banking.Api.csproj -c Release -o /out

# Use the official ASP.NET runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "MicroRabbit.Banking.Api.dll"]
