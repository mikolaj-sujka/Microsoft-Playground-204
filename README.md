# AZ-204 Microsoft Playground

AZ-204 Microsoft Playground is a centralized .NET learning repository for practicing Microsoft Azure Developer Associate concepts through small, realistic, hands-on modules.

The goal of this repository is to learn Azure services by building practical examples instead of isolated hello-world samples. The solution is designed around the main AZ-204 skill areas: Azure compute, Azure storage, Azure security, monitoring and troubleshooting, and service integrations.

## Covered Azure areas

### Azure compute

This module focuses on hosting and running .NET applications in Azure. It includes examples for Azure App Service, Azure Functions, containerized workloads, Azure Container Registry, Azure Container Instances, and Azure Container Apps.

Planned examples:

* ASP.NET Core Web API deployed to Azure App Service
* Azure Functions with HTTP, Timer, Queue, Service Bus, and Event Grid triggers
* Dockerized .NET API pushed to Azure Container Registry
* Container deployment to Azure App Service, Azure Container Instances, and Azure Container Apps

### Azure storage

This module focuses on building applications that use Azure data services. It includes Azure Blob Storage and Azure Cosmos DB examples using the official Azure SDKs.

Planned examples:

* Uploading and downloading files with Azure Blob Storage
* Setting and reading blob metadata and properties
* Generating shared access signatures
* Configuring blob lifecycle management and access tiers
* Performing Cosmos DB container and item operations with the .NET SDK
* Using Cosmos DB consistency levels and Change Feed processing

### Azure security

This module focuses on authentication, authorization, secrets, and secure service-to-service communication.

Planned examples:

* Protecting APIs with Microsoft Entra ID
* Using scopes, roles, and JWT claims
* Accessing Azure services with Managed Identity
* Reading secrets, keys, and certificates from Azure Key Vault
* Using Azure App Configuration for centralized configuration and feature flags
* Calling Microsoft Graph from a .NET application
* Creating and using shared access signatures securely

### Monitoring and troubleshooting

This module focuses on observability, diagnostics, and production troubleshooting patterns.

Planned examples:

* Instrumenting ASP.NET Core and Azure Functions with Application Insights
* Logging requests, traces, exceptions, dependencies, and custom events
* Writing basic KQL queries for troubleshooting
* Creating availability tests
* Configuring alerts and action groups

### Azure integrations

This module focuses on connecting applications with Azure platform services and event-driven architecture.

Planned examples:

* Publishing APIs through Azure API Management
* Creating API products and applying APIM policies
* Building event-based flows with Azure Event Grid
* Sending telemetry and event streams to Azure Event Hubs
* Implementing reliable messaging with Azure Service Bus queues and topics
* Using Azure Queue Storage for lightweight background processing

## Repository structure

```text
Az204.MicrosoftPlayground.sln
├── src
│   ├── Az204.Playground.Api
│   ├── Az204.Playground.Functions
│   ├── Az204.Playground.Application
│   ├── Az204.Playground.Infrastructure
│   └── Az204.Playground.Contracts
├── infra
│   ├── bicep
│   └── scripts
└── docs
    ├── app-service.md
    ├── azure-functions.md
    ├── blob-storage.md
    ├── cosmos-db.md
    ├── security.md
    ├── monitoring.md
    ├── integrations.md
    └── exam-traps.md
```

## Purpose

This repository is intended to support practical AZ-204 preparation by connecting official Microsoft Azure concepts with real .NET implementation examples. Each module should include code, notes, configuration examples, and short explanations of the most important exam scenarios and service comparisons.
