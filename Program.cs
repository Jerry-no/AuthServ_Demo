using AuthService.Application;
using AuthService.Application.Mapper;
using AuthService.Common.Abstractions;
using AuthService.Common.Exceptions;
using AuthService.Common.Filters;
using AuthService.Common.Middlewares;
using AuthService.Common.Validation;
using AuthService.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
var config = new TypeAdapterConfig();
config.Scan(typeof(ApplicationAssemblyMarker).Assembly);

builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddDbContext<AuthDbContext>();

builder.Services.AddApplication(builder.Configuration);

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ECMAnnDb"));
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<ResponseWrapperFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<LoggingEnricherMiddleware>();
builder.Services.AddScoped<IUserMapper, UserMapper>();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// Pending security
// app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();