using SecurePass.Models;
using System.Runtime.InteropServices;
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

app.MapPost("/generate", (Password password) => {
	StringBuilder sb = new StringBuilder();
	Random random = new Random();
	char[] result = new char[password.Length];
	string chars = string.Empty;

	if (password.CapitalLetters) chars += string.Concat(Enumerable.Range('A', 26).Select(c => (char)c));
	if (password.SmallLetters) chars += string.Concat(Enumerable.Range('a', 26).Select(c => (char)c));
	if (password.Numbers) chars += string.Concat(Enumerable.Range('0', 10).Select(c => (char)c));
	if (password.SpecialCharacters) chars += string.Concat(
		Enumerable.Range(33, 94).Select(i => (char)i).Where(c => !char.IsLetterOrDigit(c)
	));

	for(int i = 0; i < result.Length; i++)
	{
		result[i] = chars[random.Next(chars.Length)];
	}

	string generatedPassword = new string(result);
	return Results.Ok(generatedPassword);
});

app.Run();
