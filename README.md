#  Helping Hand - Community Volunteer Platform

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=.net)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=flat&logo=asp.net)](https://dotnet.microsoft.com/en-us/apps/aspnet)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?style=flat&logo=.net)](https://learn.microsoft.com/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=flat&logo=microsoft-sql-server)](https://www.microsoft.com/en-us/sql-server)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=flat&logo=docker)](https://www.docker.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

> A comprehensive **ASP.NET 8 Razor Pages** web application that connects community members who need help with volunteers willing to provide assistance. Built with modern technologies, containerized for easy deployment, and designed for scalability.

##  Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Quick Start](#quick-start)
- [Project Structure](#project-structure)
- [Core Concepts](#core-concepts)
- [API Overview](#api-overview)
- [Database Schema](#database-schema)
- [Development Guide](#development-guide)
- [Docker Setup](#docker-setup)
- [Contributing](#contributing)
- [License](#license)
- [Support](#support)

---

##  Overview

**Helping Hand** is a community-driven platform that:
- Connects **requesters** (people who need help) with **volunteers** (people willing to help)
- Manages help requests with status tracking and notifications
- Includes volunteer rating and verification systems
- Provides admin oversight and community management tools
- Runs entirely in Docker for consistent development and deployment
- Implements robust security with ASP.NET Identity and role-based access control

### Problem Statement
Many communities lack efficient platforms to connect people who need help with willing volunteers. This application solves that by providing a centralized, easy-to-use platform with proper verification, rating, and management features.

### Solution
Helping Hand offers:
- **Simple request creation** - Users can quickly post help requests
- **Smart matching** - Volunteers can find requests matching their skills
- **Trust & safety** - ID verification and rating system
- **Communication** - In-app notifications and messaging
- **Admin tools** - Community managers can oversee all activities

---

##  Features

### 👥 User Roles & Access Control

#### **Registered Users**
- ✅ Create and manage help requests
- ✅ Volunteer for available requests
- ✅ View personal dashboard with request history
- ✅ Receive notifications for request updates
- ✅ Rate and review volunteers/requesters
- ✅ Update profile and availability
- ✅ Upload ID verification documents
- ✅ Communicate via comments on requests

#### **Volunteers**
- ✅ Browse available help requests
- ✅ Filter requests by category, urgency, and date
- ✅ Apply to volunteer for requests
- ✅ Manage volunteer applications
- ✅ View volunteer statistics
- ✅ Build reputation through ratings
- ✅ View request templates for guidance

#### **Administrators**
- ✅ Full user management (activate/deactivate accounts)
- ✅ Moderate all requests and comments
- ✅ Manage request categories
- ✅ View system analytics
- ✅ Generate reports
- ✅ Manage volunteer verification
- ✅ Handle disputes and reviews

###  Core Features

#### **Help Request Management**
-  **Create Requests** - Post what you need help with
  - Title and detailed description
  - Category selection (Grocery, Elderly Care, Medical, etc.)
  - Urgency level (Low, Medium, High)
  - Preferred completion date
  - Auto-expiration after 14 days if unclaimed

- 🔍 **Browse & Search**
  - Filter by category, status, and urgency
  - Search by keyword
  - Sort by date, urgency, or rating

-  **Request Status Tracking**
  - **Open** - Awaiting volunteers
  - **Pending** - Volunteer applied, awaiting confirmation
  - **Assigned** - Volunteer confirmed
  - **In Progress** - Actively being worked on
  - **Completed** - Awaiting confirmation
  - **Closed** - Both parties confirmed completion

#### **Volunteer Management**
-  **Volunteer Applications**
  - Apply to requests
  - Track application status
  - Communicate with requesters

-  **Rating System**
  - Rate volunteers/requesters on a 5-star scale
  - Leave written reviews
  - Build reputation score
  - View historical ratings

-  **ID Verification**
  - Upload government-issued ID
  - Admin verification process
  - Trust badge display

#### **Notifications & Communication**
-  **Real-time Notifications**
  - New volunteer applications
  - Status updates
  - Messages from other users
  - Expiration warnings

-  **In-app Comments**
  - Discuss details on each request
  - Track conversation history
  - Moderated by admins

#### **Dashboard & Profile**
-  **Personal Dashboard**
  - Active requests
  - Volunteer history
  - Pending applications
  - Ratings and reviews

-  **Profile Management**
  - Personal information
  - Contact details
  - Emergency contact
  - Availability information
  - Profile photo (future enhancement)

#### **Admin Features**
-  **User Management**
  - View all users
  - Manage user roles
  - Lock/unlock accounts
  - Monitor suspicious activity

-  **Analytics & Reports**
  - Active requests count
  - Volunteer participation metrics
  - Category popularity
  - User growth trends

-  **Category Management**
  - Create/edit/delete request categories
  - Set category descriptions

---

##  Technology Stack

### **Backend**
- **Language**: C# 12 (.NET 8)
- **Framework**: ASP.NET Core 8.0
- **Architecture**: Razor Pages + MVC (Hybrid)
- **ORM**: Entity Framework Core 8.0
- **Authentication**: ASP.NET Identity
- **Database**: SQL Server 2022 Express

### **Frontend**
- **Templating**: Razor Pages / Razor Views
- **Styling**: Bootstrap 5
- **JavaScript**: Vanilla JS + jQuery (minimal)
- **Icons**: Bootstrap Icons

### **Data & Persistence**
- **Database**: SQL Server 2022
- **Migrations**: EF Core Migrations (Code-First)
- **Caching**: In-memory (ASP.NET Core built-in)
- **File Storage**: Local filesystem (uploads/ids)

### **DevOps & Deployment**
- **Containerization**: Docker & Docker Compose
- **Development**: Visual Studio 2026 Community
- **Version Control**: Git + GitHub
- **CI/CD Ready**: Dockerfile and compose files included

### **Development Tools**
- **IDE**: Visual Studio Community 2026
- **Package Manager**: NuGet
- **API Testing**: Postman-ready endpoints
- **Database**: SQL Server Management Studio (SSMS)

### **Dependencies**
```xml
<!-- Core Framework -->
Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.0
Microsoft.AspNetCore.Identity.UI 8.0.0
Microsoft.EntityFrameworkCore.SqlServer 8.0.0
Microsoft.EntityFrameworkCore.Tools 8.0.0

<!-- Bootstrap UI -->
Bootstrap 5.x (via CDN in layouts)
Bootstrap Icons (via CDN)
```

---

##  Quick Start

### Prerequisites
- .NET 8 SDK or Visual Studio 2026
- Docker Desktop (optional, but recommended)
- SQL Server 2022 Express or Docker SQL Server container
- Git

### Option 1: Using Docker (Recommended)

```bash
# Clone the repository
git clone https://github.com/ode77/helping-hand
cd helping-hand

# Start all services
docker-compose up -d

# Open in browser
open http://localhost:5000
```

** First startup**: ~50 seconds (SQL Server initialization)  
** Subsequent startups**: ~10 seconds

### Option 2: Local Development

```bash
# Clone the repository
git clone https://github.com/ode77/helping-hand
cd helping-hand

# Restore dependencies
dotnet restore

# Apply migrations (if needed)
dotnet ef database update

# Run the application
dotnet run

# Open in browser
open http://localhost:5000
```

### Option 3: Visual Studio 2026

1. Clone repository
2. Open `HelpingHand.sln` in Visual Studio
3. Set `HelpingHand` as startup project
4. Press **F5** to run
5. Application opens at `https://localhost:7000`

### First-Time Setup

1. **Register Account**: Click "Register" and create a new account
   - Password requirements: 8+ characters, uppercase, lowercase, digit, special character

2. **Login**: Use your credentials to access the application

3. **Create a Help Request** (as Requester):
   - Go to Home → New Request
   - Fill in title, description, category, and urgency
   - Submit

4. **Volunteer for a Request** (as Volunteer):
   - Go to Browse Requests
   - Find a request
   - Click "Apply to Help"
   - Wait for requester confirmation

5. **Admin Access** (test only):
   - Contact admin for role assignment
   - Access `/admin` page for management tools

---

##  Project Structure

```
helping-hand/
│
├─  README.md                          (This file - project documentation)
├─  ARTIFACTS.md                       (Complete artifacts file)
├─  HelpingHand.sln                    (Solution file)
│
├─  Docker Files
│  ├─ Dockerfile                         (Application container build)
│  ├─ docker-compose.yml                 (Multi-container orchestration)
│  └─ .dockerignore                      (Build optimization)
│
├─ 📖 Docker Documentation
│  ├─ README_DOCKER.md                   (Docker overview)
│  ├─ DOCKER_STATUS.md                   (Quick start)
│  ├─ DOCKER_SETUP.md                    (Complete guide)
│  ├─ DOCKER_COMMAND_REFERENCE.md        (Command reference)
│  ├─ DOCKER_IMPLEMENTATION_SUMMARY.md   (Technical details)
│  ├─ DOCKER_VERIFICATION_CHECKLIST.md   (Checklist)
│  └─ DOCKER_MANIFEST.md                 (Summary)
│
├─  VS Code Integration
│  ├─ .devcontainer/
│  │  └─ devcontainer.json               (Dev Containers config)
│  └─ .vscode/
│     └─ tasks.json                      (Build tasks)
│
├─  HelpingHand/ (Main Application)
│  │
│  ├─  Controllers/
│  │  ├─ AccountController.cs            (Authentication & registration)
│  │  ├─ HomeController.cs               (Landing page & redirect)
│  │  ├─ HelpRequestController.cs        (Request CRUD operations)
│  │  ├─ NotificationController.cs       (Notification management)
│  │  └─ AdminController.cs              (Admin functions)
│  │
│  ├─  Pages/ (Razor Pages)
│  │  ├─ Account/
│  │  │  ├─ Login.cshtml
│  │  │  ├─ Register.cshtml
│  │  │  ├─ Manage/
│  │  │  │  ├─ Index.cshtml
│  │  │  │  └─ PersonalData.cshtml
│  │  │  └─ AccessDenied.cshtml
│  │  │
│  │  ├─ Dashboard/
│  │  │  └─ Index.cshtml                 (User dashboard)
│  │  │
│  │  ├─ Admin/
│  │  │  ├─ Index.cshtml                 (Admin panel)
│  │  │  ├─ Users.cshtml                 (User management)
│  │  │  └─ Categories.cshtml            (Category management)
│  │  │
│  │  ├─ Requests/
│  │  │  ├─ Browse.cshtml                (Browse all requests)
│  │  │  ├─ Details.cshtml               (Request details)
│  │  │  ├─ Create.cshtml                (Create new request)
│  │  │  └─ MyRequests.cshtml            (User's requests)
│  │  │
│  │  └─ Volunteer/
│  │     ├─ Applications.cshtml          (Volunteer applications)
│  │     └─ Statistics.cshtml            (Volunteer stats)
│  │
│  ├─  Models/ (Data Models)
│  │  ├─ ApplicationUser.cs              (Extended Identity user)
│  │  ├─ HelpRequest.cs                  (Help request entity)
│  │  ├─ VolunteerApplication.cs         (Volunteer application)
│  │  ├─ VolunteerRating.cs              (Rating entity)
│  │  ├─ RequestComment.cs               (Comments on requests)
│  │  ├─ Notification.cs                 (User notifications)
│  │  ├─ Category.cs                     (Request categories)
│  │  ├─ RequestTemplate.cs              (Request templates)
│  │  ├─ RequestStatus.cs                (Status enum)
│  │  ├─ RequestUrgency.cs               (Urgency enum)
│  │  └─ ErrorViewModel.cs               (Error display)
│  │
│  ├─  Data/
│  │  └─ ApplicationDbContext.cs          (EF Core DbContext)
│  │
│  ├─  Repositories/ (Data Access)
│  │  ├─ IHelpRequestRepository.cs        (Request interface)
│  │  ├─ HelpRequestRepository.cs         (Request implementation)
│  │  ├─ ICategoryRepository.cs           (Category interface)
│  │  ├─ CategoryRepository.cs            (Category implementation)
│  │  ├─ INotificationRepository.cs       (Notification interface)
│  │  ├─ NotificationRepository.cs        (Notification implementation)
│  │  ├─ IVolunteerApplicationRepository.cs
│  │  ├─ VolunteerApplicationRepository.cs
│  │  ├─ IRatingRepository.cs             (Rating interface)
│  │  ├─ RatingRepository.cs              (Rating implementation)
│  │  ├─ ICommentRepository.cs            (Comment interface)
│  │  ├─ CommentRepository.cs             (Comment implementation)
│  │  ├─ ITemplateRepository.cs           (Template interface)
│  │  └─ TemplateRepository.cs            (Template implementation)
│  │
│  ├─  ViewModels/
│  │  ├─ CreateHelpRequestViewModel.cs    (Create request form)
│  │  ├─ RequestDetailsViewModel.cs       (Request with comments)
│  │  ├─ DashboardViewModel.cs            (Dashboard data)
│  │  ├─ NotificationViewModel.cs         (Notification list)
│  │  ├─ RatingViewModel.cs               (Rating form)
│  │  ├─ ProfileViewModel.cs              (User profile)
│  │  ├─ VolunteerApplicationViewModel.cs (Volunteer app form)
│  │  ├─ RequesterConfirmViewModel.cs     (Confirm completion)
│  │  ├─ AdminUserViewModel.cs            (Admin user details)
│  │  ├─ LoginViewModel.cs                (Login form)
│  │  ├─ RegisterViewModel.cs             (Registration form)
│  │  └─ RequestCardViewModel.cs          (Request list item)
│  │
│  ├─  Migrations/ (EF Core)
│  │  ├─ 20260429135354_InitialCreate.cs
│  │  ├─ 20260512144547_AddUserContactDetailsAndPendingStatus.cs
│  │  ├─ 20260512152928_AddAllNewFeatures.cs
│  │  └─ 20260512165205_AddVolunteerDoneStatusAndIdVerification.cs
│  │
│  ├─  appsettings.json                 (Configuration)
│  ├─  appsettings.Development.json     (Dev configuration)
│  ├─ HelpingHand.csproj                  (Project file)
│  └─ Program.cs                          (Startup & DI configuration)
│
├─  wwwroot/ (Static Files)
│  ├─ css/
│  │  └─ site.css                         (Custom styles)
│  ├─ js/
│  │  └─ site.js                          (Custom JavaScript)
│  ├─ lib/                                (Bootstrap, jQuery, etc.)
│  └─ uploads/
│     └─ ids/                             (ID document storage)
│
├─  .gitignore                          (Git ignore rules)
├─  .env.example                        (Environment variable template)
└─  Configuration Files
   ├─ appsettings.json
   └─ appsettings.Development.json
```

---

##  Core Concepts

### Database-First vs Code-First Approach
This project uses **Code-First** with Entity Framework Core:
- Models define the database schema
- Migrations track schema changes
- `dotnet ef` commands manage the database

### Repository Pattern
Data access is abstracted through repositories:
- `IHelpRequestRepository` - Request operations
- `ICategoryRepository` - Category operations
- `INotificationRepository` - Notification operations
- Repositories are injected via dependency injection

### Dependency Injection
All services are registered in `Program.cs`:
```csharp
builder.Services.AddScoped<IHelpRequestRepository, HelpRequestRepository>();
```

### Authentication & Authorization
- ASP.NET Identity handles user management
- Password policy: 8+ chars, uppercase, lowercase, digit, special character
- Roles: User (default), Admin
- Cookies used for session persistence

### Request Status Workflow
```
Open → Pending → Assigned → In Progress → Completed → Closed
  ↓                                           ↓
  └─────────────────────────────────────────┘
  (If not claimed within 14 days, expires)
```

### Request Expiration
- Requests automatically expire after 14 days
- Expiration date calculated on creation
- Admin can manually close expired requests

---

## 📡 API Overview

The application uses a **Razor Pages + MVC hybrid** architecture:

### Controllers (Traditional HTTP)

#### **AccountController**
```
POST    /Account/Login           - User login
POST    /Account/Register        - User registration
POST    /Account/Logout          - User logout
GET     /Account/Profile         - User profile page
```

#### **HelpRequestController**
```
GET     /HelpRequest/Browse             - Browse all requests
GET     /HelpRequest/Details/{id}       - View request details
GET     /HelpRequest/Create             - Create request form
POST    /HelpRequest/Create             - Submit new request
GET     /HelpRequest/Edit/{id}          - Edit request form
POST    /HelpRequest/Edit/{id}          - Update request
POST    /HelpRequest/Delete/{id}        - Delete request
POST    /HelpRequest/Apply/{id}         - Apply as volunteer
POST    /HelpRequest/Confirm/{id}       - Confirm completion
POST    /HelpRequest/AddComment/{id}    - Add comment
POST    /HelpRequest/Rate/{id}          - Rate volunteer/requester
```

#### **NotificationController**
```
GET     /Notification/List              - Get notifications
POST    /Notification/MarkAsRead/{id}   - Mark as read
POST    /Notification/Delete/{id}       - Delete notification
```

#### **AdminController**
```
GET     /Admin                          - Admin dashboard
GET     /Admin/Users                    - User management
GET     /Admin/Categories               - Category management
POST    /Admin/User/Lock/{id}           - Lock user account
POST    /Admin/User/Unlock/{id}         - Unlock user account
POST    /Admin/User/AssignRole          - Assign user role
POST    /Admin/Category/Create          - Create category
POST    /Admin/Category/Edit/{id}       - Edit category
POST    /Admin/Category/Delete/{id}     - Delete category
```

### Razor Pages (View-based)

- `/Pages/Account/*` - Authentication pages
- `/Pages/Dashboard/Index` - User dashboard
- `/Pages/Requests/*` - Request management
- `/Pages/Volunteer/*` - Volunteer features
- `/Pages/Admin/*` - Admin features

---

## 💾 Database Schema

### Core Entities

#### **ApplicationUser** (Extends IdentityUser)
```csharp
public class ApplicationUser: IdentityUser
{
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string Availability { get; set; }
    public string EmergencyContactName { get; set; }
    public string EmergencyContactPhone { get; set; }
}
```

#### **HelpRequest**
```csharp
public class HelpRequest
{
    public int HelpRequestId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public RequestStatus Status { get; set; }
    public RequestUrgency Urgency { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PreferredDate { get; set; }
    public DateTime ExpiresAt { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public string RequesterId { get; set; }
    public ApplicationUser? Requester { get; set; }

    public string? VolunteerId { get; set; }
    public ApplicationUser? Volunteer { get; set; }

    public bool VolunteerConfirmedDone { get; set; }
    public bool RequesterConfirmedDone { get; set; }
}
```

#### **VolunteerApplication**
```csharp
public class VolunteerApplication
{
    public int VolunteerApplicationId { get; set; }
    public int HelpRequestId { get; set; }
    public HelpRequest? HelpRequest { get; set; }

    public string VolunteerId { get; set; }
    public ApplicationUser? Volunteer { get; set; }

    public DateTime AppliedAt { get; set; }
    public ApplicationStatus Status { get; set; }
}
```

#### **VolunteerRating**
```csharp
public class VolunteerRating
{
    public int RatingId { get; set; }
    public int HelpRequestId { get; set; }
    public HelpRequest? HelpRequest { get; set; }

    public string RaterId { get; set; }
    public ApplicationUser? Rater { get; set; }

    public string RateeId { get; set; }
    public ApplicationUser? Ratee { get; set; }

    public int Rating { get; set; } // 1-5 stars
    public string ReviewText { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

#### **RequestComment**
```csharp
public class RequestComment
{
    public int CommentId { get; set; }
    public int HelpRequestId { get; set; }
    public HelpRequest? HelpRequest { get; set; }

    public string UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public string CommentText { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

#### **Category**
```csharp
public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
}
```

#### **Notification**
```csharp
public class Notification
{
    public int NotificationId { get; set; }
    public string UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }

    public int? HelpRequestId { get; set; }
    public HelpRequest? HelpRequest { get; set; }

    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Entity Relationships
```
User (ApplicationUser)
├─ HelpRequest (as Requester)
├─ HelpRequest (as Volunteer)
├─ VolunteerApplication
├─ VolunteerRating (as Rater)
├─ VolunteerRating (as Ratee)
├─ RequestComment
└─ Notification

HelpRequest
├─ Category
├─ ApplicationUser (Requester)
├─ ApplicationUser (Volunteer)
├─ VolunteerApplication (multiple)
├─ VolunteerRating (multiple)
├─ RequestComment (multiple)
└─ Notification (multiple)
```

---

##  Development Guide

### Prerequisites for Development

```bash
# Required
- .NET 8 SDK
- Visual Studio 2026 (or VS Code + .NET CLI)
- SQL Server 2022 Express (or Docker)
- Git

# Optional but Recommended
- SQL Server Management Studio (SSMS)
- Postman (for API testing)
- Docker Desktop
```

### Setting Up Development Environment

#### **Step 1: Clone Repository**
```bash
git clone https://github.com/ode77/helping-hand.git
cd helping-hand
```

#### **Step 2: Restore Dependencies**
```bash
dotnet restore
```

#### **Step 3: Configure Database Connection**

Edit `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HelpingHandDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

#### **Step 4: Run Migrations**
```bash
dotnet ef database update
```

#### **Step 5: Start Application**
```bash
dotnet run
```

### Common Development Tasks

#### **Add a New Migration**
```bash
# Create migration
dotnet ef migrations add DescriptiveChangeName

# Review generated code in Migrations folder
# Apply to database
dotnet ef database update
```

#### **Run with Debugger**
```bash
# Visual Studio: Press F5
# VS Code: Debug → Start Debugging
# CLI: dotnet run
# Then set breakpoints in code editor
```

#### **View Database**
```bash
# Using SSMS
Server: (local)\SQLEXPRESS
Database: HelpingHandDb

# Using SQL Server CLI
sqlcmd -S (local) -U sa -P YourPassword -d HelpingHandDb
```

#### **Check Entity Framework Model**
```bash
# Generate and view the model script
dotnet ef migrations script
```

### Code Organization & Patterns

#### **Adding a New Feature (Step-by-Step)**

1. **Create Model** (in `Models/`)
   ```csharp
   public class NewEntity
   {
       public int Id { get; set; }
       public string Name { get; set; }
   }
   ```

2. **Add to DbContext** (in `Data/ApplicationDbContext.cs`)
   ```csharp
   public DbSet<NewEntity> NewEntities { get; set; }
   ```

3. **Create Migration**
   ```bash
   dotnet ef migrations add AddNewEntity
   dotnet ef database update
   ```

4. **Create Repository Interface** (in `Repositories/`)
   ```csharp
   public interface INewEntityRepository
   {
       Task<List<NewEntity>> GetAllAsync();
       Task<NewEntity> GetByIdAsync(int id);
       Task AddAsync(NewEntity entity);
   }
   ```

5. **Implement Repository** (in `Repositories/`)
   ```csharp
   public class NewEntityRepository : INewEntityRepository
   {
       private readonly ApplicationDbContext _context;
       // Implementation
   }
   ```

6. **Register in DI** (in `Program.cs`)
   ```csharp
   builder.Services.AddScoped<INewEntityRepository, NewEntityRepository>();
   ```

7. **Create Controller/Page**
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class NewEntityController : ControllerBase
   {
       private readonly INewEntityRepository _repository;
       // Use _repository in actions
   }
   ```

### Testing Locally

#### **Test User Accounts** (after first run)
```
Username: testuser@example.com
Password: TestPassword123!
```

#### **Test Request Workflow**
1. Create a request as User A
2. Login as User B
3. Apply to volunteer
4. Accept as User A
5. Mark complete
6. Confirm completion
7. Rate each other

---

##  Docker Setup

### Using Docker (Recommended)

```bash
# Start everything (app + database)
docker-compose up -d

# View running containers
docker-compose ps

# View logs
docker-compose logs -f

# Stop containers
docker-compose down
```

### Why Docker?

✅ **Consistency** - Same environment locally and in production  
✅ **Easy Setup** - No manual SQL Server installation  
✅ **Isolation** - Containers don't affect host system  
✅ **Scalability** - Easy to add services (Redis, etc.)  
✅ **Documentation** - All setup documented in code

### Docker Files Included

- `Dockerfile` - Application image definition
- `docker-compose.yml` - Multi-container orchestration
- `.dockerignore` - Optimization
- `.devcontainer/` - VS Code Dev Containers support
- See `README_DOCKER.md` for detailed Docker guide

---

##  Additional Resources

### Documentation Files
- **README_DOCKER.md** - Docker setup guide
- **DOCKER_SETUP.md** - Complete Docker implementation
- **DOCKER_COMMAND_REFERENCE.md** - Docker commands
- **ARTIFACTS.md** - Complete project artifacts

### External Resources
- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [Razor Pages](https://learn.microsoft.com/aspnet/core/razor-pages/)
- [ASP.NET Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity/)
- [Docker Documentation](https://docs.docker.com/)

---

##  Contributing

Contributions are welcome! Please:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/AmazingFeature`)
3. **Commit** changes (`git commit -m 'Add AmazingFeature'`)
4. **Push** to branch (`git push origin feature/AmazingFeature`)
5. **Open** a Pull Request

### Contribution Guidelines
- Follow existing code style and conventions
- Add tests for new features
- Update documentation
- Ensure all tests pass
- Get approval from maintainers

---

##  License

This project is licensed under the **MIT License** - see LICENSE file for details.

---

##  Support & Contact

### Getting Help
1. **Check Documentation** - See `README_DOCKER.md` and guides
2. **GitHub Issues** - Report bugs and request features
3. **Discussion Board** - Ask questions and share ideas
4. **Email** - Contact project maintainers

### Community
-  **GitHub Discussions** - Join community conversations
-  **Bug Reports** - Help improve the project
-  **Feature Requests** - Suggest new functionality
-  **Star the Project** - Show your support!

---

##  Roadmap

### Phase 1 (Current) ✅
- ✅ Core request management
- ✅ Volunteer system
- ✅ Rating system
- ✅ Notifications
- ✅ Admin panel
- ✅ Docker setup

### Phase 2 (Planned)
- [ ] Profile photos
- [ ] In-app messaging system
- [ ] Advanced search & filtering
- [ ] Email notifications
- [ ] SMS notifications
- [ ] Analytics dashboard

### Phase 3 (Future)
- [ ] Mobile app (React Native)
- [ ] API v1 (REST)
- [ ] GraphQL support
- [ ] Real-time chat (SignalR)
- [ ] Maps integration
- [ ] Payment processing
- [ ] Multi-language support

---

##  Project Statistics

| Metric | Value |
|--------|-------|
| **Language** | C# |
| **.NET Version** | 8.0 |
| **Framework** | ASP.NET Core |
| **Database** | SQL Server 2022 |
| **Controllers** | 5 |
| **Models** | 10+ |
| **Repositories** | 7 |
| **Migrations** | 4 |
| **Lines of Code** | ~5000+ |
| **Test Coverage** | In progress |

---

##  Acknowledgments

- Built with [ASP.NET Core](https://dotnet.microsoft.com/)
- Styled with [Bootstrap](https://getbootstrap.com/)
- Containerized with [Docker](https://www.docker.com/)
- Hosted on [GitHub](https://github.com/)

---

##  Version History

- **v1.0.0** (Current)
  - Initial release
  - Core features implemented
  - Docker support added
  - Comprehensive documentation

---

<div align="center">

**Made with Love for the community**

[⬆ Back to Top](#-helping-hand---community-volunteer-platform)

</div>
