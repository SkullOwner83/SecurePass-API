namespace SecurePass.Models
{
	public class Generator
	{
		public int Length { get; set; }
		public bool Upper { get; set; }
		public bool Lower { get; set; }
		public bool Numbers { get; set; }
		public bool Symbols { get; set; }
	}
}
