# Travel & Tourism Platform — 

## Architecture

```
Travel.sln
├── Travel.API   ASP.NET Core Web API — controllers, JWT, Swagger, exception middleware
├── Travel.BLL   Business logic — DTOs, service interfaces/implementations, AutoMapper, validators
├── Travel.DAL   Data access — DbContext, entities, EF Core configurations, repositories, Unit of Work
└── Travel.Web   ASP.NET Core MVC — public RTL site + Admin area, talks to Travel.API only via a typed HttpClient
```


