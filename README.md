# Muse Space API

The backend service for the Muse Space platform. This RESTful API serves as the central nervous system of the application, responsible for securely managing user authentication, persisting content and social graphs, processing real-time interactions, and enforcing strict business rules for art commissions.

---

## Tech Stack & Tooling

- **Framework**: .NET 8 (Core Web API)
- **Language**: C# 12
- **Database**: PostgreSQL (Relational Data Storage)
- **ORM**: Entity Framework Core 8 (Code-First Migrations)
- **Authentication & Security**: ASP.NET Core Identity & JWT (JSON Web Tokens) with Role-Based Access Control (RBAC).
- **Real-time Communication**: SignalR (WebSockets for live notifications).
- **Media Storage**: Cloudinary SDK (Image/Video uploads and transformations).
- **Mapping**: AutoMapper (DTO to Entity conversions).
- **Documentation**: Swashbuckle / Swagger (OpenAPI 3.0).

---

## Architecture Pattern

The repository is structured using a strict **N-Tier Architecture** to ensure high maintainability, testability, and clear separation of concerns across the codebase:

- **`MuseSpace.Core` (Domain Layer)**: 
  - The lowest level of the application. 
  - Contains Entity models (`Artwork`, `User`, `Commission`), Enums, Data-Transfer Objects (DTOs), Interface definitions for repositories (`IUserRepository`), and standard result wrappers (`GenericResult` / `PagedResult`). 
  - *Rule: This layer has zero dependencies on any other project layer or external framework (like EF Core).*
- **`MuseSpace.Infrastructure` (Data Access Layer)**: 
  - Contains the concrete implementations of the repository interfaces defined in Core.
  - Houses the Entity Framework Core implementation, the `ApplicationDbContext`, and all database migrations.
- **`MuseSpace.BLL` (Business Logic Layer)**: 
  - Contains the core services (e.g., `AuthService`, `ArtworkService`, `InteractionService`, `CommissionService`). 
  - Handles all application logic, complex validation, authorization checks, and AutoMapper configurations.
- **`MuseSpace.API` (Presentation Layer)**: 
  - The entry point of the application. 
  - Contains the Controllers routing HTTP requests, Dependency Injection (DI) configurations, middleware pipelines (CORS, Error Handling), and SignalR Hubs (`NotificationHub`).

---

## Key API Modules

- **Authentication Module**: Secure Registration, Login, and JWT generation.
- **User Profile Module**: Fetching public profiles, following/unfollowing creators, and fetching follower lists via pagination.
- **Artwork Module**: Uploading art to Cloudinary, soft-deleting posts, and serving the main chronological/algorithmic feeds.
- **Interaction Module**: Liking and commenting on artworks. Triggers SignalR events internally.
- **Recommendation Module**: Semantic intersection algorithm to fetch similar artworks based on tags and descriptions.
- **Commission Module**: Managing commission requests (Creation, Accepting, Declining, Completing) and ensuring state-machine logic is followed.
- **Notification Module**: Serving paginated historical notifications and unread counts for users.

---

## Getting Started

### Prerequisites
1. **.NET 8 SDK** installed on your machine.
2. **PostgreSQL** server running locally or accessible via network.
3. A **Cloudinary** account (for media uploads).

### Installation & Setup

1. **Clone the repository** (if you haven't cloned the parent repo already):
   ```bash
   git clone https://github.com/Muse-Space-App/Muse-Space-API.git
   cd Muse-Space-API
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Configure Environment Variables**:
   Navigate to the `MuseSpace.API` directory and create or modify the `appsettings.Development.json` file. Ensure you fill in the following configurations:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=MuseSpace;Username=postgres;Password=yourpassword"
     },
     "JwtSettings": {
       "SecretKey": "YOUR_SUPER_SECRET_JWT_KEY_AT_LEAST_32_CHARS_LONG",
       "Issuer": "MuseSpaceAPI",
       "Audience": "MuseSpaceClient",
       "ExpiryMinutes": 60
     },
     "CloudinarySettings": {
       "CloudName": "your_cloud_name",
       "ApiKey": "your_api_key",
       "ApiSecret": "your_api_secret"
     }
   }
   ```

4. **Run Database Migrations**:
   Ensure your PostgreSQL server is running. Then apply the migrations to create the database schema:
   ```bash
   dotnet ef database update --project MuseSpace.Infrastructure --startup-project MuseSpace.API
   ```

5. **Run the Application**:
   Start the development server.
   ```bash
   cd MuseSpace.API
   dotnet run
   ```

6. **Swagger UI**:
   Once running, you can explore and interact with all the API endpoints by navigating to:
   `https://localhost:7198/swagger` (Port may vary depending on your `launchSettings.json`).
