using FeaneMVC.Repositories.Interfaces;
using FeaneMVC.Repositories;
using FeaneMVC.Repository;
using FinalProject.DbModel;
using FinalProject.Models;
using FoodShop.Repository;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Attributes;
using WebApplication1.Factory;
using WebApplication1.Interfaces;
using WebApplication1.Repository;

var builder = WebApplication.CreateBuilder(args);

// ���������� ��������
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// ��������� ������
builder.Services.AddDistributedMemoryCache(); // ��� �������� ������ ������ � ������
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // ����� ����� ������
    options.Cookie.HttpOnly = true; // ����������� ������ ������ ����� HTTP
    options.Cookie.IsEssential = true; // �������������� cookie
});
builder.Services.AddHttpClient();
builder.Services.AddScoped<AdminOrModeratorModeAttribute>();
builder.Services.AddScoped<AdminModeAttribute>();
builder.Services.AddScoped<AdminOrVipModeAttribute>();
builder.Services.AddScoped<VipModeAttribute>();
builder.Services.AddScoped<ModeratorModeAttribute>();
builder.Services.AddTransient<CartFactory>(provider =>
    new VipFactoryCart(provider.GetRequiredService<ApplicationDbContext>()));
builder.Services.AddTransient<CartFactory>(provider =>
    new RegularUserCart(provider.GetRequiredService<ApplicationDbContext>()));
builder.Services.AddScoped<IFilterRepository, FilterRepository>();
builder.Services.AddScoped<IReservation, ReservationRepository>();
builder.Services.AddScoped<IPaymentGateway, PaymentProcessor>();
builder.Services.AddScoped<IUSer, UserRepository>();
builder.Services.AddScoped<ICartService, RegularUserCartService>();
builder.Services.AddScoped<ICartService, VIPUserCartService>();
builder.Services.AddSingleton<INotification>(sp => NotificationService.Instance);
builder.Services.AddScoped<WebApplication1.Interfaces.ISession, SessionRepository>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine("Connection String: " + connectionString);
var app = builder.Build();

// ��������� ��������� ��������� HTTP-��������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // ��������� ��������� ������
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
