using AutoMapper;
using CargoManagement.Models;
using CargoManagement.Models.Shared;
using CargoManagement.Repository;
using CargoManagement.Services.Abstractions;
using CargoManagement.Services.BackgroundTask;
using CargoManagement.Services.Implementations;
using MailService.WebApi.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;
using WhatsappBusiness.CloudApi.Configurations;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration.GetSection("CMSConfig").Get<CMSConfig>();

//Authentication scheme starts
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config.TokenIssuer,
            ValidAudience = config.TokenIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.TokenKey))
        };
    });
//Authentication scheme ends 
//Configuring Dependancy injection Starts
builder.Services.AddHttpContextAccessor();//For injecting httpContext

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHubSerice, HubService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IJourneyService, JourneyService>();
builder.Services.AddScoped<IJourneyItemsService, JourneyItemsService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<IDropdownService, DropdownService>();
builder.Services.AddScoped<IBoxTypesService, BoxTypesService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IExpenseTypeService, ExpenseTypeService>();
builder.Services.AddScoped<IWhatsAppBusinessClient, WhatsappService>();
builder.Services.AddScoped<ILogService, LoggerService>();
builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();
//builder.Services.AddHostedService<EmailNotificationTask>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings")); // For injecting Mail
builder.Services.Configure<CMSConfig>(builder.Configuration.GetSection("CMSConfig"));// For injecting cofig
builder.Services.Configure<WhatsAppBusinessCloudApiConfig>(builder.Configuration.GetSection("WhatsAppBusinessCloudApiConfiguration"));// For injecting cofig

//My SQL Configuartion
builder.Services.AddDbContextPool<cmspartialdeliveryContext>(options =>
{
    var connetionString = builder.Configuration.GetConnectionString("Default");
    options.UseMySql(connetionString, ServerVersion.AutoDetect(connetionString));
});

//Cors Starts
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

// Auto Mapper Configurations Starts
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
// Auto Mapper Configurations Ends here

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, config.ProfileFolder)),
    RequestPath = "/" + config.ProfileFolderAlias
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, config.BookingDocsFolder)),
    RequestPath = "/" + config.BookingDocsFolderAlias
});
app.UseCors("MyPolicy");

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
