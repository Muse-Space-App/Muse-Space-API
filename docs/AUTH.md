# Authentication Guide

MuseSpace uses **JWT Bearer Token** authentication with optional **OTP email verification**.

## Flow Overview

```
┌────────┐    POST /api/auth/register    ┌────────┐
│ Client │ ──────────────────────────▶   │  API   │
│        │ ◀──────────────────────────   │        │
│        │    { token, user }            │        │
│        │                               │        │
│        │    POST /api/auth/login       │        │
│        │ ──────────────────────────▶   │        │
│        │ ◀──────────────────────────   │        │
│        │    { token, user }            │        │
│        │                               │        │
│        │    GET /api/artworks          │        │
│        │    Authorization: Bearer xxx  │        │
│        │ ──────────────────────────▶   │        │
│        │ ◀──────────────────────────   │        │
│        │    { artworks }               │        │
└────────┘                               └────────┘
```

## Endpoints

### Register

```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "artistname",
  "email": "artist@example.com",
  "password": "SecurePass123!",
  "displayName": "Artist Name"
}
```

**Response** (200):

```json
{
  "isSuccess": true,
  "data": {
    "token": "eyJhbGciOi...",
    "user": {
      "id": 1,
      "username": "artistname",
      "email": "artist@example.com",
      "displayName": "Artist Name"
    }
  }
}
```

### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "artist@example.com",
  "password": "SecurePass123!"
}
```

**Response** (200): same structure as register.

### OTP Verification

```http
POST /api/auth/send-otp
Content-Type: application/json

{
  "email": "artist@example.com"
}
```

```http
POST /api/auth/verify-otp
Content-Type: application/json

{
  "email": "artist@example.com",
  "otp": "123456"
}
```

## Token Usage

Include the JWT in the `Authorization` header for all protected endpoints:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

### Token Claims

| Claim | Description |
|-------|-------------|
| `sub` (NameIdentifier) | User ID |
| `email` | User email |
| `unique_name` | Username |
| `role` | User role (`User` or `Admin`) |

### Token Configuration

Configured in `appsettings.json`:

```json
{
  "JWT": {
    "SecretKey": "your-256-bit-secret-key",
    "Issuer": "MuseSpace",
    "Audience": "MuseSpace",
    "ExpiresInHours": 24
  }
}
```

## Password Requirements

- Passwords are hashed using BCrypt via `IPasswordHasher`
- Minimum 8 characters recommended

## Error Responses

| Status | Meaning |
|--------|---------|
| 401 | Missing or invalid token |
| 403 | Valid token but insufficient permissions |
| 400 | Validation error (bad email, weak password, etc.) |

---

← Back to [Documentation](DOCUMENTATION.md) · [README](../README.md)
