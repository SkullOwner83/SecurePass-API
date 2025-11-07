namespace SecurePass.Models
{
	public class Diagnostic
	{
		public string Strength { get; set; } = string.Empty;
		public double Score { get; set; } = 0;
		public int Characters { get; set; } = 0;
		public double Entropy { get; set; } = 0;
		public List<string> Suggestions { get; set; } = [];
	}
}
