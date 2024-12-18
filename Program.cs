using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// HttpContextAccessor hizmetini ekleyin
builder.Services.AddHttpContextAccessor();

// DB Baðlantýsý
builder.Services.AddDbContext<BerberSite.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC servislerini ekleyin
builder.Services.AddControllersWithViews();

// Session
builder.Services.AddSession();

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
  
}

app.Run();
