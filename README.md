# RepairShop
Simple Modular monolith API - proof of concept.   

A bit inspired by https://opensource.googleblog.com/2023/03/introducing-service-weaver-framework-for-writing-distributed-applications.html .

## Requirements 
Docker

## Installation 👩‍💻
1. Clone this repository
2. Run 

```
Docker compose up 
```

Swagger is exposed at https://localhost:5134/swagger/index.html

## Short description: 

The service exposes 3 different modules: Users, Tickets, Notifications. 

Each module implements basic CRUD operations via REST and can be Run as a standalone module as well as a monolith together with the wholeapplication depending on the settings at appconfig. 

Users implements the UsersAPI
Tickets implements the TicketsAPI 
Notifications implements the RulesAPI

Notifications module has 2 responsabilities: 
1. It exposes the RulesAPI that are responsible for limiting the amount of notifications that are being delivered to our users. 
2. It process every notification that is being triggered on the other modules. 



