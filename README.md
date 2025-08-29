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
Here we have removed the jQuery libraries and have added the HTMX library.  The HTMX library will allow our HTML application to make HTTP Requests using any of the following HTTP verbs: GET, POST, PUT, PATCH, DELETE.  Even though we have added the HTMX library, we are not using any of the HTMX attributes/functionality.

## Branch: 03-NavBar
This branch will implement the first HTMX functionality, starting with the navigation bar.
With each click of a navigation item, the content will change in-place without refreshing the entire page.  Some pages have deeper CRUD functionality which displays other pages.  This deeper functionality will be addressed later.  At this point, we can use either Partial Views or View Components.  For this demonstration, [View Components](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/view-components?view=aspnetcore-8.0) will be used to demonstrate maximum control by the developer.  Dependency Injection is automatically handled by Asp.Net for the View Component classes.

The only navigation items that were not changed: Home and the Contoso University Logo.
These items will still cause a complete page refresh.  This was intentional so that one can see the difference between a complete page refresh and a DOM manipulation done by HTMX.

The following controllers and their respective methods were changed.  This, in-turn, required a corresponding creation of a View Component class, with the respective Route name and one or more Views that will render a HTML fragment.  For directory structure, please see [View Components](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/view-components?view=aspnetcore-8.0)

| Controller | Route | View Component | Original View | New View |
| :--------------- | :---------- | :-------------------- | :----------------- | :-----------------
| HomeController | About | AboutViewComponent | About.cshtml | Default.cshtml |
| HomeController | Privacy | PrivacyViewComponent | Privacy.cshtml | Default.cshtml |
| CoursesController | Index | IndexViewComponent | Index.cshtml | Default.cshtml |
| StudentsController | Index | IndexViewComponent | Index.cshtml | Default.cshtml |
| InstructorsController | Index | IndexViewComponent | Index.cshtml | Default.cshtml |
|  |  |  | Index.cshtml | Default.cshtml |
|  |  |  | Index.cshtml | Courses.cshtml |
|  |  |  | Index.cshtml | Enrollments.cshtml |
| DepartmentsController | Index | IndexViewComponent | Index.cshtml | Default.cshtml |

