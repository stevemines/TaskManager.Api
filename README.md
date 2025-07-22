# TaskManager.Api

## Overview

TaskManager.Api is a RESTful API for managing tasks, built with modern .NET technologies. It provides endpoints for creating, updating, retrieving, and deleting tasks, supporting basic task management workflows.

This API is designed to be lightweight and efficient, leveraging the minimal API pattern. It uses SQLite as a data store for development and testing purposes, making it easy to set up and run locally or in a containerized environment.

The Data is contained in a presistant database stored in the root of the TaskManager.Api project directory, which is created automatically when the application starts. This allows for easy data persistence across application restarts.
Data persist between application restarts, making it suitable for development and testing scenarios.

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

### Build and Run the Docker image
`docker compose up --build`


### Hosting ASP.NET Core images with Docker Compose over HTTPS
Using the information provided at the link below for reference, setup your developemnt cert before running the application. This is required to run the application over HTTPS in a Docker container.
The docker-compse.yml file is configured to use the development certificate for HTTPS support.

[Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-3.1)
## OpenAPI Document Support
API definition is available via the links below.The documentation describes the API as a whole, including all its endpoints and operations.

`.../openapi/v1.json`

## Sample Requests
### Get All Tasks
```
curl -X 'GET' \  
'https://localhost:8081/tasks/v1' \ 
-H 'accept: application/json'
```

### Create a Task
```
curl -X 'POST' \
  'https://localhost:8081/tasks/v1' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "title": "string",
  "description": "string"
}'
```
### Update a Task
```
curl -X 'PUT' \
  'https://localhost:8081/tasks/v1/3fa85f64-5717-4562-b3fc-2c963f66afa6' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "title": "string",
  "description": "string"
}'
```
### Delete a Task
```
curl -X 'DELETE' \
  'https://localhost:8081/tasks/v1/3fa85f64-5717-4562-b3fc-2c963f66afa6' \
  -H 'accept: application/json'
```

### Complete a Task
```
curl -X 'PUT' \
  'https://localhost:8081/tasks/v1/3fa85f64-5717-4562-b3fc-2c963f66afa6/Complete' \
  -H 'accept: application/json'
  ```

## Optional

The Task Manager REST API can run standalone as well. You can run the TaskManager.Api project and make requests to various endpoints using the Swagger UI (or a client of your choice):

`.../swagger/index.html`

![alt text](https://github.com/stevemines/TaskManager.Api/blob/master/Screenshot%202025-07-21%20115759.png?raw=true)
