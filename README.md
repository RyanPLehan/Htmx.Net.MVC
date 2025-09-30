# Htmx.Net.MVC
A sample tutorial to show how to use HTMX in a standard .Net MVC application.  This tutorial will use the [Microsoft Contoso University](https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/data/ef-mvc/intro/samples/5cu) MVC sample application, originally written in .Net 5, as the base line.

To show progression, and to allow for easier download, I will use separate branches with a naming convention that should be easy to follow.  All branches will begin with *Progressions/* followed by the name in sequential order.

## Branch: 01-Baseline
This is the original MS Contoso University sample ASP.Net MVC application with the following changes applied.
1. Upgrade from .Net 5 to .Net 8 (Core)
2. Removed dependency on MSSQL in favor of an in-memory SQLite.
    -  Database is recreated everytime the application starts up.

## Branch: 02-HTMX
Here we have removed the jQuery libraries and have added the HTMX library.  The HTMX library will allow our HTML application to make HTTP Requests using any of the following HTTP verbs: GET, POST, PUT, PATCH, DELETE.  Even though we have added the HTMX library, we are not using any of the HTMX attributes/functionality.

## Branch: 03-NavBar
This branch will implement the first HTMX functionality, starting with the navigation bar.
With each click of a navigation item, the content will change in-place without refreshing the entire page.  Some pages have deeper CRUD functionality which displays other pages.  This deeper functionality will be addressed later.  At this point, we can use either Partial Views or View Components.  For this demonstration, [View Components](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/view-components?view=aspnetcore-8.0) will be used to demonstrate maximum control by the developer.  Dependency Injection is automatically handled by Asp.Net for the View Component classes.

The only navigation items that were not changed: Home and the Contoso University Logo.
These items will still cause a complete page refresh.  This was intentional so that one can see the difference between a complete page refresh and a DOM manipulation done by HTMX.

The following controllers and their respective routes were changed.  This, in-turn, required a corresponding creation of a View Component class, with the respective Route name and one or more Views that will render a HTML fragment.  For directory structure, please see [View Components](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/view-components?view=aspnetcore-8.0)

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

## Branch: 04-DirectRoute
Up to this point, all the Navigation Bar links, with the exception of the Contoso University logo and the Home, will update in place with HTML fragments from their respective url.

Howerver, many times users will have saved a link to a specific route, such that, the browser will by pass the website's default landing page.  For example, while the application is running and you open a new tab in your browser and paste: https://localhost:5001/departments  (localhost:5001 is my server), you will get an HTML fragment of the Departments.

This is a problem with using HTMX for the following reasons:
1. Bypassing the landing page may not load the following:
    - Logo(s)
    - Headers
    - Navigation Bar
    - Footer
    - CSS
    - Javascript libraries, such as HTMX
2. Going to a direct route will execute a component that may have the following results:
    - Return a fragmented HTML page without HTMX to place it into the proper DOM element.
    - May experience a runtime error due to dependencies of HTMX related properties (ie. checking for hx-* keys in the request header)

The following are solutions to address this issue:
1. Redirect to default Landing Page
    - This would require a check for the presence of HX-Request key in the Request header.  If the key is not present then the application will issue a redirect.
        - This will need to be coded either in each View Component, Controller, or some type of general middleware code.
    - This may be not be suitable for most users.
2. For every view component, create a corresponding full view.  Again, checking for HX-Rquest key in the Request header, a condition statement will be used to return either a full or fragmented HTML page.
3. Write a piece of middleware code to check for existence of the HX-Request key in the Request header.  Then do **one** of the following options:
    - If the key is present, then allow the request to move through the pipeline.  Thus executing the view component.
    - If the key is not present, then do **one** of the following:
        - Check if the URL is the base url to the home page, if so, just return the default Landing Page
        - Construct a modified Landing Page, but pass just the route portion of the url into landing page, such that, when the page is loaded, HTMX will issue a HTTP-GET to the route.

For this demonstration, this applicaiton will choose solution #3, with the following modifications:
1. Modify Contoso University Logo and Home navigation bar links to return HTML fragments.
2. Create a generic landing page such that when the page loads, HTMX will automatically invoke a given route by using the 
3. The given route will, by default, be the home page or the route passed in by the middelware component.

## Branch: 05-CRUD
This branch will specially deal with the Create Read Update and Delete (CRUD) operations.  For some routes, ie /Home/Departments, we perform a Read operation to display preliminary information to the user.  However, we will now dive deeper into each route/endpoint for enhanced CRUD operations.

We will continue to use View Components, but the views will display a [Modal Dialog](https://www.w3.org/WAI/ARIA/apg/patterns/dialog-modal/#:~:text=Dialog%20(Modal)%20Pattern-,About%20This%20Pattern,outside%20an%20active%20dialog%20window.)

The following updates were performed:
1. For proper Modal Dialog, an update of Bootstrap to version 5+ was performed.
2. Remove of direct references to */index*
3. Replace generic text with more descriptive text when items were selected.

Ideas of using Modals/Dialogs with HTMX came from [Modal forms with Django+HTMX](https://blog.benoitblanchon.fr/django-htmx-modal-form/)

One of the main issues, with the original MS Contoso University application, is that for every Route CRUD operation, there is a corresponding view.  These views usually only differ by layout and buttons.  Therefore, for each Route, there is a single partial view to display entity details, thus leaving a CRUD operation specific view as the parent.

Many of the "Index" views were split into a Master/Detail page.  The Master page contains, mainly, static information, while the Detail page contains data that has a high probability of changing.

This branch contains both client and server operations.  Meaning, server operations are requesting via HTMX and client side operations, ie closing diaglog boxes, are handled by vanilla Javascript.