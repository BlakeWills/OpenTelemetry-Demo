# .NET OpenTelemetry walk-through

This repository contains a solution for a fictional weather forecasting company, called weather.io.

I originally built this system as a demo for my "Better Observability in .NET with OpenTelemetry" talk, but I think the community may find it useful to use as a bit of a "playground" to familiarise themselves with OpenTelemetry, as well as some of the open-source observability tooling, such as Grafana and Prometheus.

## Architecture

![Architecture Diagram](Architecture.png)

## What can you do with this repo?

Both the WeatherAPI and the ForecastService are already instrumented, so you could start by exploring the configuration, and then following the [AuthenticationService Instrumentarion](docs/AuthenticationServiceInstrumentation.md) guide to walk through using OpenTelemetry to instrument a service, from scratch. 

You could also 

## Getting Started

1. Clone the repo
2. Run `docker compose up -d` from the root of the repository.
3. Kick of some requests using the following PowerShell snippet:

```powershell
while($true) {
    (iwr https://localhost:8080/WeatherForecast -Headers @{ Authorization = "Basic Ymxha2U6cEA1NXcwcmQ=" }).Content
    | ConvertFrom-Json; Start-Sleep -Seconds 2
}
```

4. Explore Grafana by loading up http://localhost:3000 in your browser.

## Roadmap

This repository doesn't cover everything, yet. Here's some things I'd like to add, contributions are welcome if you'd like to have a go yourself:

- Grafana Dashboards for visualising service health.
- Instrument the services with custom metrics.
- Add new services to show additional features, such as trace context propogation across message boundaries. E.G. Azure ServiceBus, RabbitMq, or even Kafka.
