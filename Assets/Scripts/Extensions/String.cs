namespace Extensions
{
	public static class String
	{
		/// <param name="seconds">Seconds to convert</param>
		/// <returns>Time formatted as "MM:SS".</returns>
		public static string TimeToMMSS(int seconds) => $"{seconds / 60:00}:{seconds % 60:00}";
	}
}
