# RepairShop
This repository houses a Simple Modular Monolith API, serving as a proof of concept.

Drawing inspiration from Service Weaver Framework.

https://opensource.googleblog.com/2023/03/introducing-service-weaver-framework-for-writing-distributed-applications.html .

## Requirements 
Docker

## Installation 👩‍💻
1. Clone this repository
2. Run 

```
Docker compose up 
```

Swagger is exposed at https://localhost:5134/swagger/index.html

## Overview: 
The service comprises three distinct modules: Users, Tickets, and Notifications.

Each module provides basic CRUD operations via REST and can operate autonomously or as part of the monolithic application, depending on configurations defined in the appconfig.

Users module implements the UsersAPI.
Tickets module implements the TicketsAPI.
Notifications module implements the RulesAPI.

## The Notifications module serves dual purposes:
Exposure of RulesAPI: Responsible for CRUD operations for the rules that dictate how rates will be limited.
Processing Notifications: Handles the processing of notifications triggered by other modules.

