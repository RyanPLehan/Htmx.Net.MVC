# Htmx.Net.MVC
A sample tutorial to show how to use HTMX and Tailwind in a standard .Net MVC application.  This tutorial will use the [Microsoft Contoso University](https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/data/ef-mvc/intro/samples/5cu) MVC sample application, originally written in .Net 5, as the base line.

To show progression, and to allow for easier download, I will use separate branches with a naming convention that should be easy to follow.  All branches will begin with *Progressions/* followed by the name in sequential order.

### Derivations
The following are derivations that I applied to the original MS sample application.
1. Upgrade from .Net 5 to .Net 8 (Core)
2. Removed dependency on MSSQL in favor of an in-memory SQLite.
    -  Database is recreated everytime the application starts up.

## Branch: 01-Baseline
This is the original MS Contoso University sample .Net MVC application with the forementioned derivations applied.

## Branch: 02-HTMX
Here we have removed the jQuery libraries and have added the HTMX library.  The HTMX library will allow our HTML application to make HTTP Requests using any of the following HTTP verbs: GET, POST, PUT, PATCH, DELETE.
