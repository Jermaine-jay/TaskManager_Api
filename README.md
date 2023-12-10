# Task Manager Api Application
____
#### The Application is a web application built with ASP.NET API, Entity Framework Core, Redis, PostgreSQL, and Docker. This helps individuals as well as groups to manage tasks on projects to increase productivity.


## Technologies
____
* ASP.NET Core Web API: The web framework used to build the application's architecture and handle user requests.

* Redis: A database technology used for caching.

* PostgreSQL: The relational database management system used to store and manage application data.
  
* Entity Framework Core (EF Core): A powerful and flexible Object-Relational Mapping (ORM) tool for working with the application's database.
  > - *Microsoft Entity Framework core Design V 7.0.1*
  > - *Microsoft Entity Framework core Tools V 7.0.1*
  > - *Npgsql Entity Framework core PostgreSQL V 7.0.4*
  > - *Npgsql Entity Framework core PostgreSQL Design V 1.1.0*

* Docker: A Software used to build, test, and deploy applications using containers


## Application Features
____
* User Authentication and Authorization: Secure user registration and login system, ensuring only authorized users can create and manage projects as well as tasks,
  Ensure only admins can make changes to the system. Note, that you can not login without email confirmation.

* User Update: Only confirmed users will be able to log in, create projects and tasks, pick tasks, and change details.

* Project Management: Only the creator of a project can assign user/users to a task and make changes to the project.

* Tasks: The task has a duration and a notification will be sent to a user assigned to a task 48 hours before the deadline. A notification is sent to a user when the task status is changed.

* User Profiles: Only authenticated users can use the application's features.

* Admin: Admins can delete tasks as well as projects.

## How it works
_____
* Registration: The app uses an email system to confirm a user's email when they register, and an email system that sends a link to their mail for password reset. The application also has well-secured authentication and authorization mechanisms.
  
* Email system: The email system uses a token-based approach to confirm a user's email address. When a user registers, a token is generated and sent to their email address. The user must then click on the link in the email to confirm their email address. This helps to prevent unauthorized users from creating accounts.
  
* Authentication and authorization: The application uses a combination of email and password authentication, as well as role-based authorization. This ensures that only authorized users can access the application.

## Getting Started
_____
* Clone the repository: git clone https://github.com/Germaine-jay/TaskManager_Api.git

* Install required packages: dotnet restore

* Update the database connection string in the appsettings.json file to point to your SQL Server instance.

* Apply database migrations: dotnet ef database update

* Run the application: dotnet run


## Contributing
_____
If you'd like to contribute to the Car Auction App, please follow these steps:

* Fork the repository.

* Create a new branch for your feature or bug fix: git checkout -b feature/your-feature-name

* Make your changes and test thoroughly.

* Commit your changes: git commit -m "Add your commit message here"

* Push to your forked repository: git push origin feature/your-feature-name

* Create a pull request, describing your changes and the problem they solve.

## application URL
* https://task-manager-944y.onrender.com
* https://task-manager-944y.onrender.com/swagger/index.html

## Default Users
___
| Email                    | Password   | Role       |
| -----------------------  | ---------- | ---------- |
| jermaine.jay00@gmail.com | 12345qwert | User       |
| mosalah11@outlook.com    | 12345qwert | Admin      |
| IbouKonate@gmail.com     | 12345qwert | User       |  
