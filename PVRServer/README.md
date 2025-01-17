# PVR Server

## Overview
The PVR Server is a **gRPC server** designed to handle incoming requests from clients. It generates shipping labels based on the rules configured in the database and returns the label to the client for printing. By centralizing label generation in this way, it eliminates the need for each client machine to be updated when new couriers are added or rules are modified, reducing downtime for both clients and Datum.

This server serves as the **only** interface to the database. Other projects in the solution should interact with the PVR Server to ensure that all changes to the database are securely made and properly logged. This design ensures a centralized and auditable method of interacting with the database.

---

## Setting up the Database

To set up the database for this project, follow these steps:

### 1. Configure the Connection String

The PVR Server uses **`appsettings.json`** and **`appsettings.Development.json`** for configuration. However, to simplify local development, a template file named **`appsettings.Development.template.json`** is provided.

Follow these steps to set up the connection string for local development:

1. Copy the **`appsettings.Development.template.json`** file.
2. Rename the copy to **`appsettings.Development.json`**.
3. Update the `PVRConnStr` value with your local database connection details.
4. Open Visual Studio, select **`appsettings.Developement.json`** and change **`Copy to Output Directory`** to **`Copy if newer`**

Your `appsettings.Development.json` will look something like this:

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Debug"
    }
  },
  "ConnectionStrings": {
    "PVRConnStr": "Server=localhost;Database=PVRDatabase;User Id=PVR_user;Password=SecureProdPass!;Encrypt=True;TrustServerCertificate=True;"
  }
}
```

The **`appsettings.Development.json`** file is ignored in `.gitignore` to ensure sensitive information (like passwords) does not get pushed to the repository.

It's also important to make sure the database has been created with no tables inside of it. And provide the correct database user permissions.

---

### 2. Install EF Core Tools (If not already installed)
Before applying migrations, ensure that you have installed the necessary EF Core tools. Run the following command in your terminal:

```
dotnet tool install --global dotnet-ef
```

This installs the Entity Framework Core tools globally, allowing you to run migration commands from the terminal.

---

### 3. Apply Database Migrations

In order to apply the current migrations to your database, follow these steps:

#### a) Open a terminal or command prompt

Navigate to the **`PVRServer`** project directory, where your **`DbContext`** and migrations are located.

#### b) Run the migration command

Execute the following command to apply the migrations to your database:

```
dotnet ef database update --project PVRServer.csproj
```

This command will apply all pending migrations to the database specified in your **`appsettings.json`** or **`appsettings.Development.json`** connection string.

If you're setting up for the first time or need to ensure the migrations are up to date, this will create the necessary tables and schema in the database.

---

### 4. Verify the Database
Once the migrations are applied, you should verify that the database schema matches the expectations. You can do this by inspecting the database directly or running your application and ensuring it functions as expected.

---

### 5. Rollback Migrations (if needed)

If you need to undo a migration or roll back to a previous version, you can use the following command:

```
dotnet ef database update <PreviousMigrationName> --project PVRServer.csproj
```

Replace **`<PreviousMigrationName>`** with the name of the migration you want to roll back to. If you need to remove all migrations and start fresh, you can use:

```
dotnet ef database update 0 --project PVRServer.csproj
```

---

### 6. Troubleshooting
- If you encounter any issues while applying migrations, ensure that your connection string is correct and the database is accessible.
- Check the migration logs for errors related to permissions, missing tables, or schema mismatches.
