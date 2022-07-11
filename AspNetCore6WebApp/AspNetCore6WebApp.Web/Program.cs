using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.DLL;
using AspNetCore6WebApp.Business;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
//builder.Services.AddControllersWithViews().AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.PropertyNamingPolicy = null;
//    options.JsonSerializerOptions.DictionaryKeyPolicy = null;
//});
//builder.Services.AddDbContextPool<AspNetCore6WebAppDbContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));
//});
builder.Services.AddDbContextPool<AspNetCore6WebAppDbContext>(options =>
{
    options.UseSqlite(string.Format(builder.Configuration.GetConnectionString(Param.AppSettings.ConnectionStrings.SQLite), builder.Configuration[Param.AppSettings.SqliteDatabaseFilePath]));
});
builder.Services.AddLogging(loggingBuilder =>
{
    var loggingSection = builder.Configuration.GetSection(Param.AppSettings.Logging);
    loggingBuilder.AddFile(loggingSection);
});
builder.Services.AddScoped<IAppFuncLevelDao, AppFuncLevelDao>();
builder.Services.AddScoped<IAppFunctionDao, AppFunctionDao>();
builder.Services.AddScoped<ICategoryDao, CategoryDao>();
builder.Services.AddScoped<IDepartmentDao, DepartmentDao>();
builder.Services.AddScoped<IProductDao, ProductDao>();
builder.Services.AddScoped<IRoleDao, RoleDao>();
builder.Services.AddScoped<ISubCategoryDao, SubCategoryDao>();
builder.Services.AddScoped<IUserDao, UserDao>();
builder.Services.AddScoped<IVendorDao, VendorDao>();
builder.Services.AddScoped<IAppFuncLevelBo, AppFuncLevelBo>();
builder.Services.AddScoped<IAppFunctionBo, AppFunctionBo>();
builder.Services.AddScoped<ICategoryBo, CategoryBo>();
builder.Services.AddScoped<IDepartmentBo, DepartmentBo>();
builder.Services.AddScoped<IProductBo, ProductBo>();
builder.Services.AddScoped<ISubCategoryBo, SubCategoryBo>();
builder.Services.AddScoped<IRoleBo, RoleBo>();
builder.Services.AddScoped<IUserBo, UserBo>();
builder.Services.AddScoped<IVendorBo, VendorBo>();
builder.Services.AddScoped<TT.IDbHelper, TT.DbHelper>();
builder.Services.AddScoped<TT.IExcelDataReaderHelper, TT.ExcelDataReaderHelper>();
builder.Services.AddScoped<TT.IPagingHelper, TT.PagingHelper>();
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
