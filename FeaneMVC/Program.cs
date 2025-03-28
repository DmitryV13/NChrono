using FinalProject.DbModel;
using FinalProject.Models;
using FoodShop.Repository;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Attributes;
using WebApplication1.Factory;
using WebApplication1.Interfaces;
using WebApplication1.Repository;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Настройка сессий
builder.Services.AddDistributedMemoryCache(); // Для хранения данных сессий в памяти
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Время жизни сессии
    options.Cookie.HttpOnly = true; // Доступность сессии только через HTTP
    options.Cookie.IsEssential = true; // Обязательность cookie
});
builder.Services.AddScoped<AdminOrModeratorModeAttribute>();
builder.Services.AddScoped<AdminModeAttribute>();
builder.Services.AddScoped<AdminOrVipModeAttribute>();
builder.Services.AddScoped<VipModeAttribute>();
builder.Services.AddScoped<ModeratorModeAttribute>();
builder.Services.AddTransient<CartFactory>(provider =>
    new VipFactoryCart(provider.GetRequiredService<ApplicationDbContext>()));
builder.Services.AddTransient<CartFactory>(provider =>
    new RegularUserCart(provider.GetRequiredService<ApplicationDbContext>()));
builder.Services.AddScoped<IReservation, ReservationRepository>();
builder.Services.AddScoped<IPaymentGateway, PaymentProcessor>();
builder.Services.AddScoped<IUSer, UserRepository>();
builder.Services.AddScoped<ICartService, RegularUserCartService>();
builder.Services.AddScoped<ICartService, VIPUserCartService>();
builder.Services.AddScoped<IDishes, DishRepository>();
builder.Services.AddSingleton<INotification>(sp => NotificationService.Instance);
builder.Services.AddScoped<WebApplication1.Interfaces.ISession, SessionRepository>();

var app = builder.Build();

// Настройка конвейера обработки HTTP-запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Добавляем поддержку сессий
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
