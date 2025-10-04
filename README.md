# AvikstromPortfolio 🌐

AvikstromPortfolio is an ASP.NET Core MVC web application showcasing my portfolio projects.  
It demonstrates production-grade coding practices with SignalR, API integration, caching and background services.

---

## Features

### Home
- Overview of the portfolio web app  
- Links to my GitHub repositories

### About Me
- **Bio** – A short introduction about me  
- **Resume** – Displays my resume (PDF) directly in the app  
- **Contact** – A contact form recruiters/employers can fill out to get in touch 

### Portfolio
- **Live Flight Board (SignalR demo)**  
  - Real-time flight board powered by SignalR  
  - Background polling service with caching and rotation  
  - Integration with the FlightInfoApi for arrivals/departures  

- **Formula 1 Season Info (API demo)**  
  - Shows F1 Next Race and Standings

---

## Technologies
- ASP.NET Core 8 MVC  
- SignalR for real-time communication  
- Entity Framework Core with postgreSQL
- Razor Views for rendering  
- Newtonsoft.Json for JSON parsing  
- AeroDataBox API (via FlightInfoApi)
- FastF1 API (via f1_info_api)
- Deployment on Microsoft Azure

---