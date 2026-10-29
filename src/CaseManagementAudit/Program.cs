using CaseManagementAudit.Data;
using CaseManagementAudit.Data.Interceptors;
using CaseManagementAudit.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Case Management & Audit API",
        Version = "v1",
        Description = "Enterprise EF Core Reference: AsSplitQuery, Interceptors, Bulk Mutations, and Fluent API."
    });
});

builder.Services.AddSingleton<AuditSaveChangesInterceptor>();

builder.Services.AddDbContext<CaseDbContext>((serviceProvider, options) =>
{
    var interceptor = serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>();
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                           ?? "Data Source=casemanagement.db";

    options.UseSqlite(connectionString)
           .AddInterceptors(interceptor)
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CaseDbContext>();
    await DbInitializer.InitializeAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Case Management API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.MapCaseEndpoints();

app.Run();