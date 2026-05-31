# API Endpoints Reference

All endpoints are prefixed with `/api`. Protected endpoints require `Authorization: Bearer <token>`.

---

## Auth

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/auth/register` | No | Register a new user |
| POST | `/auth/login` | No | Login with credentials |
| POST | `/auth/send-otp` | No | Send OTP to email |
| POST | `/auth/verify-otp` | No | Verify OTP code |

See [AUTH.md](AUTH.md) for details.

---

## Artworks

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/artworks` | Yes | Upload a new artwork |
| GET | `/artworks/{id}` | No | Get artwork by ID |
| PUT | `/artworks/{id}` | Yes | Update artwork |
| DELETE | `/artworks/{id}` | Yes | Delete artwork |
| GET | `/artworks/user/{userId}` | No | Get artworks by user |
| GET | `/feed` | No | Get paginated artwork feed |

---

## Social

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/users/{userId}/follow` | Yes | Toggle follow/unfollow |
| GET | `/users/{userId}/profile` | No | Get user profile |
| GET | `/users/{userId}/followers` | No | Get followers list |
| GET | `/users/{userId}/following` | No | Get following list |

---

## Comments

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/artworks/{artworkId}/comments` | Yes | Create comment/reply |
| GET | `/artworks/{artworkId}/comments` | No | Get comments for artwork |
| GET | `/comments/{commentId}/replies` | No | Get replies for comment |
| PUT | `/comments/{commentId}` | Yes | Update comment |
| DELETE | `/comments/{commentId}` | Yes | Delete comment |

---

## Interactions

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/artworks/{artworkId}/like` | Yes | Toggle like |
| POST | `/artworks/{artworkId}/bookmark` | Yes | Toggle bookmark |
| POST | `/artworks/{artworkId}/share` | Yes | Record share |

---

## Events

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/events` | Yes | Create event |
| GET | `/events/{eventId}` | No | Get event details |
| PUT | `/events/{eventId}` | Yes | Update event |
| DELETE | `/events/{eventId}` | Yes | Delete event |
| GET | `/events` | No | Get upcoming events |
| GET | `/events/organizer/{organizerId}` | No | Get events by organizer |
| POST | `/events/{eventId}/rsvp` | Yes | RSVP to event |
| DELETE | `/events/{eventId}/rsvp` | Yes | Cancel RSVP |
| GET | `/events/{eventId}/rsvps` | No | Get event RSVPs |

---

## Groups

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/groups` | Yes | Create group |
| GET | `/groups/{groupId}` | No | Get group details |
| PUT | `/groups/{groupId}` | Yes | Update group |
| POST | `/groups/{groupId}/join` | Yes | Join group |
| POST | `/groups/{groupId}/leave` | Yes | Leave group |
| GET | `/groups/{groupId}/members` | No | Get members |
| POST | `/groups/{groupId}/posts` | Yes | Create post |
| GET | `/groups/{groupId}/posts` | No | Get posts |
| DELETE | `/groups/posts/{postId}` | Yes | Delete post |

---

## Commissions

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/commissions` | Yes | Create commission request |
| GET | `/commissions/requested` | Yes | Get my requested commissions |
| GET | `/commissions/received` | Yes | Get commissions I received |
| GET | `/commissions/{id}` | Yes | Get commission details |
| PATCH | `/commissions/{id}/status` | Yes | Update commission status |
| GET | `/commissions/{id}/messages` | Yes | Get commission messages |
| POST | `/commissions/{id}/messages` | Yes | Send commission message |

---

## Notifications

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/notifications` | Yes | Get user notifications |
| GET | `/notifications/unread-count` | Yes | Get unread count |
| POST | `/notifications/{id}/read` | Yes | Mark as read |
| POST | `/notifications/read-all` | Yes | Mark all as read |

Real-time notifications are delivered via **SignalR** at `/hubs/notifications`.

---

## Search

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/search?query=` | Yes | Unified search |
| GET | `/search/advanced` | Yes | Advanced search (Pixiv-like) |

### Advanced Search Parameters

| Param | Type | Description |
|-------|------|-------------|
| `Query` | string | Text search query |
| `ExactTag` | string | Filter by exact tag name |
| `SortBy` | string | `newest`, `popular`, or `views` |
| `Page` | int | Page number (default: 1) |
| `PageSize` | int | Items per page (default: 20) |

---

## Recommendations

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/recommendations` | Yes | Get personalized recommendations |
| GET | `/recommendations/similar/{artworkId}` | Yes | Get similar artworks |

---

## Tags

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/tag/popular` | No | Get popular tags |
| GET | `/tag/search?query=` | No | Search tags |
| GET | `/tag/{tagName}/artworks` | No | Get artworks by tag |

---

## Dashboard

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/dashboard/stats` | Yes | Get creator analytics |

---

## Payments

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/payments/{commissionId}/qr` | Yes | Generate QRIS payment QR |
| POST | `/payments/{commissionId}/confirm` | Yes | Confirm payment (client) |
| POST | `/payments/{commissionId}/verify` | Yes | Verify payment (artist) |

---

## Admin

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/admin/stats` | Admin | Get platform statistics |
| GET | `/admin/reports` | Admin | Get pending reports |
| POST | `/admin/reports/{id}/review` | Admin | Review a report |
| POST | `/admin/users/ban` | Admin | Ban a user |
| POST | `/admin/users/{id}/unban` | Admin | Unban a user |

---

← Back to [Documentation](DOCUMENTATION.md) · [README](../README.md)
