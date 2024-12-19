using VIS_projekt.Services;
using VIS_projekt.Connect;
using VIS_projekt.Gateways;
using VIS_projekt.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionBuilder = DBConnector.GetBuilder();
builder.Services.AddSingleton<SqlConnectionStringBuilder>(connectionBuilder);

TrainingPlan.SetConnectionString(connectionBuilder.ConnectionString);

var paymentCsvPath = Path.Combine(builder.Environment.ContentRootPath, "Data", "Payments.csv");
builder.Services.AddSingleton(new CsvPaymentProvider(paymentCsvPath));

builder.Services.AddScoped<TrainingPlanService>();
builder.Services.AddSingleton<TrainerGateway>();
builder.Services.AddScoped<UserGateway>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<PaymentGateway>();

var app = builder.Build();

var rewriteOptions = new RewriteOptions()
    .AddRedirect("^login$", "login.html"); 

app.UseRewriter(rewriteOptions);

app.UseDefaultFiles(); 
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
