using SecurePass.Models;

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

	if (generator.Length < 0 || generator.Length > 100)
		return Results.BadRequest("Specified length out of range.");

	if (generator.Upper) chars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	if (generator.Lower) chars += "abcdefghijklmnopqrstuvwxyz";
	if (generator.Numbers) chars += "0123456789";
	if (generator.Symbols) chars += "!@#$%^&*()-_=+[]{};:,.<>/?|\\`~";

	if (string.IsNullOrEmpty(chars))
		return Results.BadRequest("No character groups gave been provided to generate the password.");

	for(int i = 0; i < result.Length; i++)
	{
		result[i] = chars[random.Next(chars.Length)];
	}

	string generatedPassword = new string(result);
	return Results.Ok(generatedPassword);
});

app.MapPost("/validate", (string password) =>
{
    if (string.IsNullOrEmpty(password))
        return Results.BadRequest("Password cannot be empty.");

    var evaluator = new Zxcvbn.Zxcvbn();
    var result = evaluator.EvaluatePassword(password);

    var diagnostic = new Diagnostic
    {
        Characters = password.Length,
        Entropy = result.Entropy
    };

    double diversity = 0;
    if (password.Any(char.IsLower)) diversity += 0.25;
    if (password.Any(char.IsUpper)) diversity += 0.25;
    if (password.Any(char.IsDigit)) diversity += 0.25;
    if (password.Any(c => char.IsSymbol(c) || char.IsPunctuation(c))) diversity += 0.25;

    // --- Normalize entropy to 0–1 range ---
    double normalizedEntropy = Math.Min(result.Entropy / 128.0, 1.0);

    // --- Combine entropy + diversity + length into a single score ---
    double lengthFactor = Math.Min(password.Length / 20.0, 1.0);
    double combinedScore = (normalizedEntropy * 0.6) + (diversity * 0.3) + (lengthFactor * 0.1);
    double percentage = Math.Round(combinedScore * 100, 2);
    diagnostic.Score = percentage;

    // --- Strength classification based on percentage ---
    diagnostic.Strength = percentage switch
    {
        < 20 => "Very weak",
        < 40 => "Weak",
        < 60 => "Medium",
        < 80 => "Strong",
        _ => "Very strong"
    };


    if (!password.Any(char.IsUpper)) diagnostic.Suggestions.Add("Add at least one uppercase letter.");
    if (!password.Any(char.IsLower)) diagnostic.Suggestions.Add("Add at least one lowercase letter.");
    if (!password.Any(char.IsDigit)) diagnostic.Suggestions.Add("Add at least one number.");
    if (!password.Any(c => char.IsSymbol(c) || char.IsPunctuation(c))) diagnostic.Suggestions.Add("Add at least one symbol or special character.");
    if (!password.Contains(' ')) diagnostic.Suggestions.Add("Add at least one space or separator to improve unpredictability.");
    if (password.Length < 12) diagnostic.Suggestions.Add("Increase the password length to at least 12 characters.");
    if (result.Entropy < 36) diagnostic.Suggestions.Add("Avoid common words or predictable patterns.");

    return Results.Ok(diagnostic);
});

app.Run();