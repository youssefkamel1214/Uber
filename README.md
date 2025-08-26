🚖 Uber Ride-Sharing API

A RESTful ASP.NET Core Web API that powers a ride-sharing platform.
It includes driver & passenger management, trip requests, tendering system, reviews, cancellations, authentication with JWT & refresh tokens, rate limiting, real-time WebSocket updates, and global exception handling for clean error responses.

📌 Features

✅ User Management

Driver & Passenger sign-up

Identity-based authentication with JWT & Refresh Tokens

Role-based Authorization (Driver, Passenger)

Rate Limiting on Auth Endpoints to prevent brute-force attacks

✅ Trips

Create, cancel, start, and complete trips

Real-time trip updates via WebSockets

✅ Tendering System

Drivers bid (Tender) on trips

Passengers accept/reject offers

✅ Reviews & Ratings

Leave reviews & ratings for trips

Automatic rating aggregation for drivers/passengers

✅ Cancellations

Cancellation with fees & refund handling

✅ Global Exception Handling

✅ Swagger (OpenAPI) documentation & testing

🛠️ Technologies Used

ASP.NET Core Web API

Entity Framework Core + PostgreSQL

Identity (ASP.NET Identity with roles)

JWT Authentication + Refresh Tokens

WebSockets for real-time communication

ASP.NET Core Rate Limiting Middleware

AutoMapper (optional mapping)

Serilog for logging

Swagger / Swashbuckle

📂 Project Structure
Uber.API/
  - Controllers/
    -- AuthniticationController.cs
    -- TripController.cs
    -- TenderController.cs
  - Models/
    -- Domain/
    -- DTOs/
    -- Configurations/
  - Services/
    -- AuthniticationService.cs
    -- TripService.cs
    -- NotificationManager.cs
  - Repositories/
    --  DriverRepository.cs
    -- PassengerRepository.cs
    -- TripRepository.cs
    -- TenderRepository.cs
    -- ReviewRepository.cs
  - Data/
    --  UberAuthDatabase.cs
  - Middleware/
    -- ExceptionHandlerMiddleware.cs
  - WebSockets/
    -- webSocketManager.cs
    -- WebSocketService.cs
  - appsettings.json
  -  Program.cs

🔐 Authentication & Authorization
🔑 JWT Authentication

Users authenticate via /api/Authnitication/signin

On success, receive JWT + Refresh Token

♻️ Refresh Tokens

Refresh via: POST /api/Authnitication/refreshtoken

If a valid refresh token is provided, a new JWT is issued and the refresh token marked as used

⛔ Rate Limiting

Applied on authentication routes (/signin, /refreshtoken, /SignUpDriver, /SignUpPassenger)

Prevents brute-force login attempts per IP

👥 Roles

Driver → Can offer bids, accept trips, complete trips

Passenger → Can request trips, accept/reject bids

Endpoints protected with [Authorize(Roles = "...")]

🌐 Real-Time Communication

Drivers listen to trip requests via WebSocket:

GET /api/Trip/LithenOnTrips


Passengers & Drivers join a trip channel for updates:

GET /api/Trip/LithenOnTripChannel?tripId={id}

🧪 API Testing with Swagger

Swagger UI available at:

https://localhost:{PORT}/swagger


To test protected routes:

Sign in to obtain a JWT

Click Authorize in Swagger

Paste the JWT token

🚀 Getting Started
🔧 Setup Instructions

Clone the repository:

git clone https://github.com/your-username/UberApi.git
cd UberApi


Configure PostgreSQL connection in appsettings.json:

"ConnectionStrings": {
  "WebApiDatabase": "Host=localhost;Database=UberDb;Username=postgres;Password=yourpassword"
},
"Jwt": {
  "Key": "YOURSECRETKEY",
  "Issuer": "UberApi",
  "Audience": "UberApiUsers"
}


Apply migrations:

dotnet ef database update


Run the application:

dotnet run

🗂️ Example API Endpoints
Authentication

POST /api/Authnitication/SignUpDriver

POST /api/Authnitication/SignUpPassenger

POST /api/Authnitication/signin

POST /api/Authnitication/refreshtoken

Trips

POST /api/Trip/requestTrip

POST /api/Trip/CancelTrip

POST /api/Trip/ConfirmPickUp

POST /api/Trip/CompleteTrip

POST /api/Trip/MakeReview

Tenders

POST /api/Tender/addTender

POST /api/Tender/updateTender

POST /api/Tender/ConfirmTender

POST /api/Tender/AcceptTenderOffer

POST /api/Tender/RejectTenderOffer

💡 Future Improvements

🚦 Trip pricing algorithm

🔔 Push notifications

🌍 Multi-language support

📃 License

MIT License. Feel free to use, modify, and contribute.
