# MeetSpot 🏢

> Meeting Room Booking System — Final Project @ Code Academy 2026

MeetSpot is a full-stack meeting room booking platform built with ASP.NET Core. Users can find available meeting rooms, book them instantly, receive QR codes for check-in, and manage bookings through a personal dashboard. Admins have full control via a dedicated panel.

---

## 🛠 Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend API | ASP.NET Core .NET 10 |
| Frontend | ASP.NET Core MVC + HttpClient |
| Database | SQL Server (Docker) |
| Cache | Redis (Docker) |
| ORM | Entity Framework Core (Code First) |
| Auth | JWT + Refresh Token + 2FA |
| Real-time | SignalR |
| Email | MailKit (Gmail SMTP) |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Docs | Scalar (OpenAPI) |
| Tests | xUnit + Moq (10 tests) |

---

## 🚀 Local Development

### Prerequisites
- .NET 10 SDK
- Docker Desktop

### 1. Start SQL Server + Redis (dev compose builds & runs everything)

```bash
cp .env.example .env      # then edit .env with local values
docker compose up --build
```

The API applies EF Core migrations automatically on startup (retrying until SQL
Server is ready) and seeds an initial admin from `AdminSeed__*` in `.env`.

- **Frontend:** http://localhost:5156
- **API:** http://localhost:5249
- **API Docs (Scalar, dev only):** http://localhost:5249/scalar/v1

### Running without Docker

```bash
# Terminal 1 — API (port 5249)
dotnet run --project MeetSpot.API
# Terminal 2 — Frontend (port 5156)
dotnet run --project MeetSpot.Web
```

Migrations still apply automatically at API startup; no manual `dotnet ef` step
is required.

---

## 🌐 Production Deployment (Docker Compose + Caddy)

`docker-compose.prod.yml` runs SQL Server, Redis, the API, the Web frontend and
a **Caddy** reverse proxy that terminates HTTPS (automatic Let's Encrypt certs).
Internal ports are not published — only Caddy's 80/443 are exposed.

### 1. Point a domain at the server
Use your own domain, or an [sslip.io](https://sslip.io) hostname that encodes
the server's public IP (e.g. `203.0.113.10.sslip.io`). Ports **80** and **443**
must be reachable from the internet for TLS issuance.

### 2. Configure `.env`

```bash
cp .env.example .env
```

Set at least: `DOMAIN`, `SQL_SA_PASSWORD` (+ matching password in
`ConnectionStrings__DefaultConnection`), a 32+ char `Jwt__Key`, the SMTP
`Email__*` values, `AdminSeed__*`, and the `https://<DOMAIN>` URLs for
`AppSettings__FrontendBaseUrl`, `Cors__Origins` and `FileStorage__PublicBaseUrl`.

### 3. Launch

```bash
docker compose -f docker-compose.prod.yml up -d --build
```

Then open `https://<your-domain>`. Migrations apply and the admin is seeded
automatically on first boot.

### Database backups

```bash
SQL_SA_PASSWORD=<pass> ./scripts/backup-sql.sh ./backups
```

---

## 👤 Admin Account
The initial admin is seeded from `AdminSeed__Email` / `AdminSeed__Password` in
`.env` the first time the database is created. Set a strong password there.

---

## 📁 Project Structure
MeetSpot/
├── MeetSpot.API          # Controllers, Hubs, Middleware, BackgroundServices
├── MeetSpot.Core         # Entities, DTOs, Interfaces, Validators, Mappings
├── MeetSpot.Infrastructure  # DbContext, Migrations, Services
├── MeetSpot.Web          # MVC Frontend (Controllers, Views, Services)
└── MeetSpot.Tests        # xUnit Tests (10 tests)

---

## ✨ Features

### Required (F1-F8)
- **F1** — Room catalog with photos, floor, capacity, equipment
- **F2** — Real-time availability search by date, time, capacity
- **F3** — Booking with QR code + email confirmation
- **F4** — Cancellation with role-based penalty fees
- **F5** — Weekly room calendar view (8:00–18:00)
- **F6** — Conflict detection (no double bookings)
- **F7** — Personal dashboard with bookings & QR codes
- **F8** — Admin panel (rooms, users, analytics)

### Original Features
- 🖼️ Hover photo effect on room cards (switch between multiple photos)
- ⭐ Real review system with admin moderation (Approve → Feature → Home)
- 🏢 Animated company logo ticker (two columns, infinite scroll)
- ☰ Mega-menu navigation (Trendyol-style)
- 📅 Room Calendar — weekly booking grid per room
- 📊 Room Availability Overview — all rooms × all hours in one view
- 🎠 Company slideshow on /Company page
- 🖼️ Hero Slides management via Admin panel
- 🤖 Auto-release no-show logic (BackgroundService, every 5 min)
- 💎 3 account types with different rules (Regular / Premium / Corporate)

---

## 👥 User Types

| Type | Booking | Cancellation | Balance |
|------|---------|--------------|---------|
| Regular | 5 days ahead | 50% penalty (<4-5h) | Required |
| Premium | Same day | 25% penalty (<1-2h) | Required |
| Corporate | Same day | No penalty | Not needed |
| Admin | Anytime | No penalty | N/A |

---

## 🐳 Docker Hub

```bash
docker pull jalya/meetspot-api
docker pull jalya/meetspot-web
```
---

## 🧪 Running Tests

```bash
dotnet test MeetSpot.Tests
```

---

## 📌 Git Branches

All features developed in separate branches, merged to `main` when complete:

`feature/entities` → `feature/auth` → `feature/rooms` → `feature/booking` → `feature/2fa-docker-tests` → `feature/frontend-home` → `feature/room-images` → `feature/auth-pages` → `feature/dashboard` → `feature/admin-panel` → `feature/reviews` → `feature/hero-slides` → `feature/static-pages` → `feature/blog-balance` → `feature/profile-final` → `main`

---

**Student:** Jalya Həsənli (jalya07)  
**Course:** Code Academy — Final Project 2026  
**Project:** #17 — Meeting Room Booking System (Variant B)