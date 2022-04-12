
using Microsoft.AspNetCore.Mvc;

var apiVersion = new ApiVersion(1, 0);
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("secrets/appsettings.secrets.json", optional: true);

builder.Configuration.AddUserSecrets<Program>(optional: true);

builder.Services.AddHealthChecks();
builder.Services.AddControllers().AddDapr();
