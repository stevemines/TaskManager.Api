# TaskManager.Api

## Overview

TaskManager.Api is a RESTful API for managing tasks, built with modern .NET technologies. It provides endpoints for creating, updating, retrieving, and deleting tasks, supporting basic task management workflows.

## Tech Stack

- **.NET 9**: Utilizes the latest features and performance improvements of the .NET platform.
- **Minimal APIs**: Implements lightweight HTTP APIs using the minimal API approach for simplicity and performance.
- **C#**: Core programming language for all business logic and data models.
- **SQLite Data Store**: Uses a SQLite database for development and testing purposes.
- **xUnit**: Unit testing framework for .NET, used for automated tests.
- **Moq**: Mocking library for isolating dependencies in unit tests.

## Design Decisions

- **Minimal API Pattern**: Chosen for its concise syntax and reduced boilerplate, making the API easy to maintain and extend.
- **Separation of Concerns**: Business logic is encapsulated in service classes, while data models and DTOs are clearly separated.
- **Testability**: Interfaces and dependency injection are used to facilitate unit testing and promote loose coupling.
- **Versioned Endpoints**: API endpoints are versioned (e.g., `TaskManagerEndpointsV1`) to support future enhancements without breaking existing clients.
- **Extensibility**: The architecture allows for easy replacement of the SQLite data store with any other persistent databases in the future.

## Running with Docker

You can build and run the TaskManager.Api application using Docker. Make sure you have [Docker](https://www.docker.com/get-started) installed.

### Build the Docker image