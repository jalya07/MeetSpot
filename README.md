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

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- Docker Desktop
- SQL Server (via Docker)
- Redis (via Docker)

### 1. Start Docker containers

```bash
# SQL Server
docker run -d -p 1433:1433 --name sql \
  -e ACCEPT_EULA=Y \
  -e MSSQL_SA_PASSWORD=CodeWithArjun123 \
  mcr.microsoft.com/azure-sql-edge

# Redis
docker run -d -p 6379:6379 --name meetspot-redis redis
```

### 2. Apply migrations

```bash
dotnet ef database update \
  --project MeetSpot.Infrastructure \
  --startup-project MeetSpot.API
```

### 3. Run the projects

```bash
# Terminal 1 — API (port 5249)
dotnet run --project MeetSpot.API

# Terminal 2 — Frontend (port 5156)
dotnet run --project MeetSpot.Web
```

### 4. Open in browser
- **Frontend:** http://localhost:5156
- **API Docs (Scalar):** http://localhost:5249/scalar/v1

---

## 👤 Default Admin Account
Email:    jalavh@code.edu.az
Password: Admin123!

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