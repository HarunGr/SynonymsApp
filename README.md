# SynonymApp API

A lightweight synonym graph management backend built with .NET.
---

## ✨ Features

- **Minimal API** (ASP.NET Core 8 / .NET 8)
- **Synonym Graph stored in-memory**
- **Persistence via Channels** (async background writing)
- **SQLite** for lightweight JSON-based storage
- **User authentication** with JWT (Login / Register)
- **MediatR** for clean command and query separation
- **FluentValidation** for input validation
- **CORS** enabled (temporarily set to allow all)
- **OpenAPI/Swagger** support

---

## 🧠 How It Works

- At **startup**, the synonym graph is **loaded from SQLite**, where it is stored as a single JSON blob.
- All operations (adding words or synonyms) happen on the **in-memory dictionary** (`Dictionary<string, HashSet<string>>`).
- After each write, the **entire graph is pushed to a background channel** for persistence.
- The **channel is handled in the background**, ensuring the API remains fast and responsive.
- No explicit "save" or "update" logic is needed; the snapshot is written on each change.

---

## 🛠 Tech Stack

| Tech                      | Purpose                         |
|--------------------------|----------------------------------|
| .NET 8                   | Core framework                   |
| ASP.NET Core Minimal API | Lightweight API surface          |
| SQLite                   | Persistent JSON storage          |
| MediatR                  | CQRS implementation              |
| Channels                 | Background write persistence     |
| FluentValidation         | Request validation               |
| JWT                      | User authentication              |

---

## 🔐 Authentication

Simple JWT-based authentication with:

- `POST /auth/register`: Register a new user (username + password)
- `POST /auth/login`: Login and receive a JWT token

Password is hashed using SHA256 (can be improved with salted bcrypt if needed).

Use the returned token in `Authorization: Bearer <token>` headers.

Note that the JWT authorization is done as simple as could be - so no secrets kept anywhere else than in code, done mostly just to demonstrate the auth.
---

## 🧪 Endpoints

| Method | Route                                | Description                            |
|--------|--------------------------------------|----------------------------------------|
| GET    | `/api/v1/synonyms/{word}`           | Returns all synonyms for a given word  |
| POST   | `/api/v1/synonyms`                  | Adds a synonym link between two words  |
| POST   | `/auth/register`                    | Registers a user                       |
| POST   | `/auth/login`                       | Authenticates a user and returns token |

---
