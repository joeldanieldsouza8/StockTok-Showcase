# StockTok - Financial Social Network

> **⚠️ Notice to Reviewers:**
>
> This repository was significantly refactored on **Feb 3rd, 2026**.
>
> If you reviewed this code prior to this date, please note that the `master` branch has been updated to reflect a modern **Microservices Architecture** using YARP and a cleaner Domain-Driven Design approach.
>
> **Key Improvements:**
> * Refactored backend into separated services (News, User, Community).
> * Implemented API Gateway (YARP).
> * Added Unit Tests with xUnit & Moq.
> * Sanitized configuration and security settings.

---

## 📖 Project Overview

**StockTok** is a social platform designed for financial enthusiasts. It combines real-time market data with community interaction, allowing users to discuss stocks, track news, and interact with peers.

This project serves as a showcase of a **Distributed Microservices Architecture** built with **.NET** and **Next.js**.

## 🏗 Architecture

The backend is structured using **Clean Architecture** principles, enforcing a strict separation of concerns between the Domain, Infrastructure, and API layers.

### System Components

| Service | Technology | Description |
| :--- | :--- | :--- |
| **API Gateway** | .NET / YARP | The entry point for all client requests. Handles routing, load balancing, and reverse proxying to downstream clusters. |
| **User Service** | .NET / EF Core | Manages user identity, profiles, and authentication state (integrated with Auth0). |
| **News Service** | .NET / External API | Aggregates financial news from the MarketAux API. Includes caching and data transformation logic. |
| **Community Service** | .NET / EF Core | Handles social features including posts, comments, and threads. |
| **Frontend** | Next.js / TypeScript | A modern, server-side rendered React application. |

### Data Strategy

Each microservice owns its own database context and migration history, adhering to the **Database-per-Service** pattern to ensure loose coupling.

* **UserDB:** PostgreSQL (Port 5432)
* **CommunityDB:** PostgreSQL (Port 5433)
* **NewsDB:** PostgreSQL (Port 5434)

## 🛠 Tech Stack

* **Backend:** .NET 9, Entity Framework Core, YARP (Reverse Proxy)
* **Frontend:** Next.js, TypeScript, Tailwind CSS
* **Databases:** PostgreSQL (Dockerized)
* **Testing:** xUnit, Moq
* **Authentication:** Auth0 (OAuth 2.0 / OIDC)
* **DevOps:** Docker Compose

## 🚀 Getting Started

Follow these steps to set up the project locally.

### 1. Prerequisites

* [.NET SDK](https://dotnet.microsoft.com/download)
* [Docker Desktop](https://www.docker.com/products/docker-desktop)
* [Node.js](https://nodejs.org/)

### 2. Configuration Setup

**Security Note:** This project uses sanitized configuration files. You must rename the example files to activate them.

**Root Environment (Databases)**
```bash
# In the root directory
cp .env.example .env
```

**Frontend Environment**

Run this in the frontend directory:
```bash
cd frontend
cp .env.example .env.local
```

> **Note:** You will need to populate `frontend/.env.local` with your own Auth0 credentials if you wish to test the full login flow.

**Backend Configuration**

The backend services (User, News, Community) use `appsettings.Development.json`.

Each service requires Auth0 credentials and service-specific settings. Below is an example configuration structure:
```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
        "NewsDatabase": ""
    },
    "Auth0": {
        "Domain": "",
        "Audience": ""
    },
    "NewsApiSettings": {
        "BaseUrl": "https://api.marketaux.com/v1/",
        "ApiToken": ""
    }
}
```

> **Important:** You must populate the following fields in each service's `appsettings.Development.json`:
>
> * **Auth0.Domain** - Your Auth0 tenant domain (e.g., `your-tenant.auth0.com`)
> * **Auth0.Audience** - Your Auth0 API audience identifier
> * **ConnectionStrings** - The appropriate database connection string for each service
>
> Additionally, the News service requires an API Key. Navigate to `backend/Src/Services/News/News.Api/appsettings.Development.json` and replace the empty `ApiToken` value with a valid MarketAux API key (or leave as is to test the application without live news data).

### 3. Run Infrastructure (Docker)

Start the PostgreSQL containers for all three microservices.
```bash
docker-compose up -d
```

### 4. Run the Application

You can run the backend services individually or using your IDE's compound run configuration.

**Option A: Using CLI**

Open separate terminals for the Gateway and Services:
```bash
dotnet run --project backend/Src/Gateway/ApiGateway
dotnet run --project backend/Src/Services/User/User.Api
dotnet run --project backend/Src/Services/News/News.Api
dotnet run --project backend/Src/Services/Community/Community.Api
```

**Option B: Visual Studio / Rider**

Use the "Start All" Run Configuration (if imported) to launch all services and the Gateway simultaneously.

### 5. Run the Frontend
```bash
cd frontend
npm install
npm run dev
```

The application will be available at http://localhost:3000.

## 🧪 Testing

The solution includes a dedicated Unit Test project for the News Service, focusing on business logic validation and mocking external dependencies.

To run tests:
```bash
dotnet test backend/News.Tests
```

## 📂 Project Structure
```
├── backend/
│   ├── ApiGateway/       # YARP Configuration & Routing
│   ├── Community.Api/    # Community Microservice
│   ├── News.Api/         # News Microservice
│   ├── User.Api/         # User Microservice
│   └── News.Tests/       # Unit Tests (xUnit)
├── frontend/             # Next.js Application
├── compose.yaml          # Docker Infrastructure
└── .env.example          # Environment Variable Template
```