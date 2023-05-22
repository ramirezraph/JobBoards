
using JobBoards.Data;
using JobBoards.Data.Persistence.Initialization;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    builder.Services.AddControllersWithViews();

    builder.Services.AddSession();
    builder.Services.AddHttpContextAccessor();

    builder.Services
        .AddWebAppData(builder.Configuration)
        .AddDbInitializer();
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSession();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.Run();
}


