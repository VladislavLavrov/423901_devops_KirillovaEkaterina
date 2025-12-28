using Microsoft.Data.Sqlite;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/login.html";
        options.AccessDeniedPath = "/login.html";
        options.Events.OnRedirectToLogin = ctx =>
        {
            if (ctx.Request.Path.StartsWithSegments("/api"))
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
            ctx.Response.Redirect(ctx.RedirectUri);
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<Func<SqliteConnection>>(_ =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return () => new SqliteConnection(connectionString);
});

var app = builder.Build();

// Инициализация базы данных SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString))
{
    // Создание директории для БД, если путь указан с директорией
    var dbPath = connectionString.Replace("Data Source=", "").Trim();
    if (dbPath.Contains('/') || dbPath.Contains('\\'))
    {
        var directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
    
    using (var conn = new SqliteConnection(connectionString))
    {
        conn.Open();
        
        // Создание таблицы users
        conn.Execute(@"
            CREATE TABLE IF NOT EXISTS users (
                username TEXT PRIMARY KEY,
                password_hash TEXT NOT NULL,
                role TEXT NOT NULL
            )");
        
        // Добавление тестовых пользователей
        conn.Execute(@"
            INSERT OR IGNORE INTO users (username, password_hash, role) 
            VALUES ('admin', 'admin', 'admin');
            
            INSERT OR IGNORE INTO users (username, password_hash, role) 
            VALUES ('accountant', 'accountant', 'accountant');
            
            INSERT OR IGNORE INTO users (username, password_hash, role) 
            VALUES ('buh', '123', 'accountant');
        ");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapGet("/", () => Results.Redirect("login.html"));
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();


app.Run();
