namespace SecurePass.Models
{
	public class Password
	{
		public int Length { get; set; }
		public bool CapitalLetters { get; set; }
		public bool SmallLetters { get; set; }
		public bool Numbers { get; set; }
		public bool SpecialCharacters { get; set; }
	}
}
