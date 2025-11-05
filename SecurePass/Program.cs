using SecurePass.Models;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Hello World");

app.MapPost("/generate", (Generator generator) => {
	Random random = new Random();
	char[] result = new char[generator.Length];
	string chars = string.Empty;

	if (generator.Length < 0 || generator.Length > 100) {
		return Results.BadRequest("Specified length out of range.");
	}

	if (generator.Upper) chars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	if (generator.Lower) chars += "abcdefghijklmnopqrstuvwxyz";
	if (generator.Numbers) chars += "0123456789";
	if (generator.Symbols) chars += "!@#$%^&*()-_=+[]{};:,.<>/?|\\`~";

	if (string.IsNullOrEmpty(chars)) {
		return Results.BadRequest("No character groups gave been provided to generate the password.");
	}

	for(int i = 0; i < result.Length; i++)
	{
		result[i] = chars[random.Next(chars.Length)];
	}

	string generatedPassword = new string(result);
	return Results.Ok(generatedPassword);
});