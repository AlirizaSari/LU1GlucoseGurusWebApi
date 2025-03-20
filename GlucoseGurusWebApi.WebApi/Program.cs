using GlucoseGurusWebApi.WebApi.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
    .AddBearerToken();

builder.Services
    .AddIdentityApiEndpoints<IdentityUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 10;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddRoles<IdentityRole>()
    .AddDapperStores(options =>
    {
        options.ConnectionString = builder.Configuration["SqlConnectionString"];
    });

builder.Services.Configure<BearerTokenOptions>(options =>
{
    options.BearerTokenExpiration = TimeSpan.FromMinutes(60);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireClaim("admin");
    });
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<RouteOptions>(o => o.LowercaseUrls = true);

var sqlConnectionString = builder.Configuration["SqlConnectionString"];
var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);

builder.Services.AddTransient<IDoctorRepository, SqlDoctorRepository>(provider => new SqlDoctorRepository(sqlConnectionString));
builder.Services.AddTransient<ITrajectRepository, SqlTrajectRepository>(provider => new SqlTrajectRepository(sqlConnectionString));
builder.Services.AddTransient<IPatientRepository, SqlPatientRepository>(provider => new SqlPatientRepository(sqlConnectionString));
builder.Services.AddTransient<IParentGuardianRepository, SqlParentGuardianRepository>(provider => new SqlParentGuardianRepository(sqlConnectionString));
builder.Services.AddTransient<ICareMomentRepository, SqlCareMomentRepository>(provider => new SqlCareMomentRepository(sqlConnectionString));
builder.Services.AddTransient<INoteRepository, SqlNoteRepository>(provider => new SqlNoteRepository(sqlConnectionString));

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAuthenticationService, AspNetIdentityAuthenticationService>();

var app = builder.Build();

app.MapGet("/", () => $"The API is up 🚀\nConnection string found: {(sqlConnectionStringFound ? "✅" : "❌")}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/account")
    .MapIdentityApi<IdentityUser>();

app.MapPost("/account/logout", async (SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();
})
.RequireAuthorization();

app.MapControllers()
    .RequireAuthorization();

app.Run();
