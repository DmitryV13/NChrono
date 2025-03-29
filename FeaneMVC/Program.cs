using FeaneMVC.Repository;
using FinalProject.DbModel;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Attributes;
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
builder.Services.AddScoped<IUSer, UserRepository>();
builder.Services.AddSingleton<INotification>(sp => NotificationService.Instance);
builder.Services.AddScoped<WebApplication1.Interfaces.ISession, SessionRepository>();

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
