using PhonebookApp.Repositories;

const string VueClientCorsPolicy = "VueClient";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(VueClientCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseCors(VueClientCorsPolicy);

app.UseStaticFiles();

app.MapControllers();

app.MapFallback("/api/{**path}", () => Results.NotFound());
app.MapFallbackToFile("index.html");

app.Run();
