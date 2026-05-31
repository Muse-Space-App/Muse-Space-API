# MuseSpace API — Documentation

## Architecture

MuseSpace follows a **4-layer clean architecture**:

```
┌─────────────────────────────────────────────┐
│                  API Layer                  │
│   Controllers · Middleware · Hubs · DI      │
├─────────────────────────────────────────────┤
│            Business Logic Layer             │
│   Services · DTOs · Mappings · Interfaces   │
├─────────────────────────────────────────────┤
│                 Core Layer                  │
│   Entities · Enums · Interfaces · Results   │
├─────────────────────────────────────────────┤
│            Infrastructure Layer             │
│   Repositories · DbContext · Migrations     │
│   External Services (Cloudinary)            │
└─────────────────────────────────────────────┘
```

### Dependency Flow

- **API** → BLL → Core
- **Infrastructure** → Core
- **API** → Infrastructure (only for DI registration)

## API Conventions

### Response Format

All endpoints return a `GenericResult<T>`:

```json
{
  "isSuccess": true,
  "data": { ... },
  "message": null,
  "errorType": null
}
```

On error:

```json
{
  "isSuccess": false,
  "data": null,
  "message": "Resource not found",
  "errorType": "NotFound"
}
```

### Pagination

Paginated endpoints return `PagedResult<T>`:

```json
{
  "items": [ ... ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 42,
  "totalPages": 3
}
```

### Authentication

All protected endpoints require a `Bearer` token in the `Authorization` header. See the [Authentication Guide](AUTH.md) for details.

## Modules

| Module | Description | Docs |
|--------|-------------|------|
| **Auth** | JWT login, register, OTP verification | [AUTH.md](AUTH.md) |
| **Artworks** | CRUD for artworks, feed, masonry layout support | [API_ENDPOINTS.md](API_ENDPOINTS.md#artworks) |
| **Social** | Follow/unfollow, user profiles | [API_ENDPOINTS.md](API_ENDPOINTS.md#social) |
| **Comments** | Comment and reply threads on artworks | [API_ENDPOINTS.md](API_ENDPOINTS.md#comments) |
| **Interactions** | Likes, bookmarks, shares | [API_ENDPOINTS.md](API_ENDPOINTS.md#interactions) |
| **Events** | CRUD for community events, RSVPs | [API_ENDPOINTS.md](API_ENDPOINTS.md#events) |
| **Groups** | Group creation, membership, posts | [API_ENDPOINTS.md](API_ENDPOINTS.md#groups) |
| **Commissions** | Commission requests, messaging, status flow | [API_ENDPOINTS.md](API_ENDPOINTS.md#commissions) |
| **Notifications** | Real-time notifications via SignalR | [API_ENDPOINTS.md](API_ENDPOINTS.md#notifications) |
| **Search** | Unified and advanced (Pixiv-like) search | [API_ENDPOINTS.md](API_ENDPOINTS.md#search) |
| **Recommendations** | Personalized and similar-artwork recommendations | [API_ENDPOINTS.md](API_ENDPOINTS.md#recommendations) |
| **Dashboard** | Creator analytics and stats | [API_ENDPOINTS.md](API_ENDPOINTS.md#dashboard) |
| **Payments** | Mock QRIS payment flow | [API_ENDPOINTS.md](API_ENDPOINTS.md#payments) |
| **Admin** | Moderation, reports, user bans | [API_ENDPOINTS.md](API_ENDPOINTS.md#admin) |

## External Services

| Service | Purpose | Config Key |
|---------|---------|------------|
| **PostgreSQL** | Primary database | `ConnectionStrings:DefaultConnection` |
| **Cloudinary** | Image/video storage and CDN | `Cloudinary:*` |
| **Redis (Upstash)** | Distributed caching | `Redis:ConnectionString` |
| **Google Gemini** | AI artwork descriptions | `Gemini:ApiKey` |
| **SMTP (Gmail)** | Transactional emails | `Email:*` |

## Rate Limiting

The API uses a fixed-window rate limiter:

- **Window**: 10 seconds
- **Limit**: 5 requests per window per user
- Applies to endpoints decorated with `[EnableRateLimiting("fixed")]`
