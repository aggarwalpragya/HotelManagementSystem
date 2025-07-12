using System.Text;
using APIHotelManagement.Interfaces;
using APIHotelManagement.Models;
using APIHotelManagement.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()                               // Log to console
    .CreateLogger();

// Replace default .NET Core logging with Serilog
builder.Host.UseSerilog();

// front connection
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")  // Angular frontend URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
            //.AllowCredentials();
        });
});

// JWT Authentication
builder.Services.AddHttpClient("JWTMicroservice", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Microservices:JWTMicroservice"]);
});



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration["JWT:SecretKey"]
        ))
    };
});



builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddDbContext<HotelManagementDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register your repositories and other services
builder.Services.AddScoped<IStaff, StaffRepository>();
builder.Services.AddScoped<IRoom, RoomRepository>();
builder.Services.AddScoped<IInventory, InventoryRepository>();
builder.Services.AddScoped<IDepartment, DepartmentRepository>();
builder.Services.AddScoped<IGuest, GuestRepository>();
builder.Services.AddScoped<IReservation, ReservationRepository>();
builder.Services.AddScoped<IService, ServiceRepository>();
builder.Services.AddScoped<IRoomType, RoomTypeRepository>();
builder.Services.AddScoped<IPayment, PaymentRepository>();
builder.Services.AddScoped<IReservationService, ReservationServiceRepository>();
builder.Services.AddScoped<IUser, UserRepository>();
builder.Services.AddScoped<IBill, BillRepository>();
builder.Services.AddScoped<IReport, ReportRepository>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Main API V1");
        c.SwaggerEndpoint("https://localhost:7009/swagger/v1/swagger.json", "Authentication API"); // External API
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");


app.UseAuthentication(); // Add this before Authorization
app.UseAuthorization();
app.MapControllers();

app.Run();
