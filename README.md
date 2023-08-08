# ExchangeService Microservice

### Backend Developer Assessment Task (M#D#####t)
### Author: Bartlomiej Kempinski
### Date: 08.08.2023


## The Onion Architecture
For the task, I chose to implement the Onion Architecture due to its significant advantages when applied to microservices. The Onion Architecture's modular structure facilitates independent development and deployment of microservices, enabling better scalability, maintainability, and separation of concerns. Its clear delineation of layers - from the core business logic outward to infrastructure - promotes flexibility and enhances testability, making it an ideal choice for designing robust and efficient microservices systems.

![Alt text](doc/ExchangeServiceArch.png?raw=true "ExchangeService Onion Architecture")

## .NET Framework 7
Opting for .NET 7 over earlier versions brings advantages such as enhanced performance, updated language features, new APIs, and libraries that align with modern technologies. It offers bug fixes, stability improvements, and community support while future-proofing your solution and aligning with contemporary coding practices and architectural trends

## Database(s) of my choice
I have selected three distinct databases, leveraging Entity Framework Core: an in-memory implementation for rapid development, SQLite for both development and data persistence, and an option for the release version using SQL Server, ensuring scalability and robustness.

## Caching
For caching, I've opted for dual options: memory and Redis. Memory cache accelerates development, while Redis cache not only minimizes database calls but also provides distributed locking, ensuring data consistency across instances of microservices.

## RESTful APIs
I've chosen .NET WebAPI for building RESTful APIs due to its straightforward controller creation and built-in support for the OpenAPI interface (Swagger), which facilitates convenient testing throughout the development process.

## Unit Tests
Within this assessment task project, the focus of the unit tests revolves specifically around the application domain. This ensures thorough coverage and validation of the core functionality. The unit tests cover a portion of the code, showcasing the tools and methodologies employed during their creation.

## Logging
I've elected to use the Serilog logging library due to its established reputation. During development, its integration enables logs to be readily observed in the Visual Studio Output window, greatly expediting the development process. Additionally, Serilog writes logs to files, ensuring a persistent record of application activities for analysis and troubleshooting.

## Settings
* DataStoreType: Refers to the data store type used by Entity Framework. Possible values include: Memory, Sqlite, SqlServer
* DistributedCacheType: Denotes the type of cache and distributed lock mechanism employed. Possible values are: Memory, Redis
* TradesCountLimit: Sets the maximum number of trades allowed per hour (bonus 2)
* TradesCountExpiration: Specifies the time period for the aforementioned trade limit. The default timeframe is one hour
* DefaultExchangeRateProviderName: Establishes the default exchange rate provider to be used. Possible options are: fixer.io, exchangeratesapi.io
* ExchangeRateExpiration: Determines the expiration duration for exchange rate values (bonus 1).

## Getting started
Open the "appsettings.Development.json" file and configure the API access keys for Fixer and ExchangeRatesApi. By default, during development, the application will utilize the Memory data store and cache. You can modify this behavior by following the instructions provided in the settings above.

Since client authorization is not within the scope of this task, the application utilizes the client's IP address as a means of identification for this purpose.

REST API endpoints:
* /api/Exchange/Convert - converts currency
* /api/Exchange/Trade - trades currency and saves to store