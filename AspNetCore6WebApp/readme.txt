AspNetCore6WebApp
--------------------------------------------------
It is a sample program of ASP.NET Core 6 web application.
.Net 6
--------------------------------------------------
Open a "ASP.NET Core Web App" project for "AspNetCore6WebApp.Web".
Open a "Class Library" project for "AspNetCore6WebApp.Business".
Open a "Class Library" project for "AspNetCore6WebApp.Entities".
Open a "Class Library" project for "AspNetCore6WebApp.DLL".
Open a "Console App" project for "AspNetCore6WebApp.Console".
Open a "Class Library" project for "TT".

Install the below packages in NuGet of the "AspNetCore6WebApp.DLL" project.
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.Sqlite

Install the below packages in NuGet of the "AspNetCore6WebApp.Console" project.
Microsoft.Extensions.Configuration.Binder
Microsoft.Extensions.Configuration.EnvironmentVariables
Microsoft.Extensions.Configuration.Json
Microsoft.Extensions.Hosting
NReco.Logging.File

Install the below packages in NuGet of the "AspNetCore6WebApp.Web" project.
NReco.Logging.File

Install the below packages in NuGet of the "TT" project.
ExcelDataReader
ExcelDataReader.DataSet

Add the below under <Project> in the project file of "AspNetCore6WebApp.Business".
<ItemGroup>
<ProjectReference Include="..\AspNetCore6WebApp.DLL\AspNetCore6WebApp.DLL.csproj" />
<ProjectReference Include="..\TT\TT.csproj" />
</ItemGroup>

Add the below under <Project> in the project file of "AspNetCore6WebApp.DLL".
<ItemGroup>
<ProjectReference Include="..\AspNetCore6WebApp.Entities\AspNetCore6WebApp.Entities.csproj" />
<ProjectReference Include="..\TT\TT.csproj" />
</ItemGroup>

Add the below under <Project> in the project file of "AspNetCore6WebApp.Console".
<ItemGroup>
<ProjectReference Include="..\AspNetCore6WebApp.Entities\AspNetCore6WebApp.Entities.csproj" />
<ProjectReference Include="..\AspNetCore6WebApp.Business\AspNetCore6WebApp.Business.csproj" />
<ProjectReference Include="..\TT\TT.csproj" />
</ItemGroup>

Add the below under <Project> in the project file of "AspNetCore6WebApp.Web".
<ItemGroup>
<ProjectReference Include="..\AspNetCore6WebApp.Business\AspNetCore6WebApp.Business.csproj" />
<ProjectReference Include="..\AspNetCore6WebApp.DLL\AspNetCore6WebApp.DLL.csproj" />
<ProjectReference Include="..\TT\TT.csproj" />
</ItemGroup>
--------------------------------------------------
Compiler Warning (level 2) CS0108
https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/cs0108
Working with Nullable Reference Types
https://docs.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types
CS8618 Nullable Reference types in C# – Best practices
https://www.dotnetcurry.com/csharp/nullable-reference-types-csharp
--------------------------------------------------
https://getbootstrap.com/docs/5.0/examples/offcanvas-navbar/
--------------------------------------------------
Razor Pages with EF Core in ASP.NET Core - Sort, Filter, Paging
https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/sort-filter-page?view=aspnetcore-6.0
PAGINATION WITH ENTITY FRAMEWORK IN ASP.NET MVC
https://learningprogramming.net/net/asp-net-mvc/pagination-with-entity-framework-in-asp-net-mvc/
--------------------------------------------------
Open a "Windows Forms App" project for "AspNetCore6WebApp.WinForm".
Add the below under <Project> in the project file of "AspNetCore6WebApp.WinForm".
<ItemGroup>
<ProjectReference Include="..\AspNetCore6WebApp.Entities\AspNetCore6WebApp.Entities.csproj" />
<ProjectReference Include="..\AspNetCore6WebApp.Business\AspNetCore6WebApp.Business.csproj" />
<ProjectReference Include="..\TT\TT.csproj" />
</ItemGroup>

Install the below packages in NuGet of the "AspNetCore6WebApp.WinForm" project.
Microsoft.Extensions.Configuration.Binder
Microsoft.Extensions.Configuration.EnvironmentVariables
Microsoft.Extensions.Configuration.Json
Microsoft.Extensions.Hosting
--------------------------------------------------
--------------------------------------------------
--------------------------------------------------
--------------------------------------------------
--------------------------------------------------
