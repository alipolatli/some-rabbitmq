using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQAddWatermarksToImages.BackgroundServices;
using RabbitMQAddWatermarksToImages.Models;
using RabbitMQAddWatermarksToImages.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(optionsAction => optionsAction.UseInMemoryDatabase(databaseName: "WatermarkDb"));
builder.Services.AddSingleton<IRabbitMQClientService, RabbitMQClientService>();
builder.Services.AddSingleton<IConnectionFactory>(serviceProvider => new ConnectionFactory { Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")) });
builder.Services.AddSingleton(typeof(RabbitMQPublisher));
builder.Services.AddSingleton(typeof(RabbitMQSubscriber));
builder.Services.AddHostedService<BSAddWatermarksToImages>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
