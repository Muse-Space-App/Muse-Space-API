# MuseSpace API

A .NET 9 Web API backend for **MuseSpace** — an art-sharing and community platform where artists showcase their work, receive commissions, and engage with fans.

## Tech Stack

- **Runtime**: .NET 9 / ASP.NET Core
- **Database**: PostgreSQL (via Entity Framework Core)
- **Cache**: Redis (Upstash) with in-memory fallback
- **Media Storage**: Cloudinary
- **Auth**: JWT Bearer Tokens
- **Real-time**: SignalR (notifications)
- **AI**: Google Gemini (artwork descriptions)
- **Logging**: Serilog (console + rolling file)

## Project Structure

```
MuseSpace.sln
├── MuseSpace.API           # Controllers, middleware, hubs, Program.cs
├── MuseSpace.BLL           # Services, DTOs, mappings, business logic
├── MuseSpace.Core          # Entities, enums, interfaces, result types
└── MuseSpace.Infrastructure # Repositories, DbContext, migrations, external services
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- PostgreSQL instance
- Cloudinary account (free tier)

### Setup

1. Clone the repository
2. Copy `appsettings.example.json` to `appsettings.json` and fill in your credentials
3. Run migrations:
   ```bash
   dotnet ef database update --project MuseSpace.Infrastructure --startup-project MuseSpace.API
   ```
4. Start the API:
   ```bash
   dotnet run --project MuseSpace.API
   ```
5. Open Swagger UI at `https://localhost:7001/swagger`

## Documentation

- [Full Documentation](docs/DOCUMENTATION.md) — architecture overview, API conventions, and module summaries
- [Authentication Guide](docs/AUTH.md) — JWT auth flow, login, register, OTP verification
- [API Endpoints Reference](docs/API_ENDPOINTS.md) — complete list of all API endpoints

## Contributors

- **VViCh** — architecture, infrastructure, social, artwork, feed, groups, commissions, search, dashboard, recommendations, admin, payments, AI services
- **melvin12129986** — comments, likes, bookmarks, events, notifications
