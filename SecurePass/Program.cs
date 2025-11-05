using SecurePass.Models;
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

app.MapPost("/password", (Password password) => {
	StringBuilder sb = new StringBuilder();
	Random random = new Random();
	string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
	char[] stringChars = new char[password.Length];

	for(int i = 0; i < stringChars.Length; i++)
	{
		stringChars[i] = chars[random.Next(chars.Length)];
	}

	string generatedPassword = new string(stringChars);
	return Results.Ok(generatedPassword);
});

app.Run();
