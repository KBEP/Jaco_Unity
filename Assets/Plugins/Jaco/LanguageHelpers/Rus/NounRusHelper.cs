namespace Jaco
{
	public static class NounRusHelper
	{
		static readonly string[] numbers = { "ед", "мн24", "мн05" };

		public static string NumToMod (int number)
		{
			return numbers[CalcPluralFormIdx(number)];
		}

		static int CalcPluralFormIdx (int value)
		{
			long m = value >= 0 ? value : -value;//make the value positive and use long type to avoid overflow
			  return (int) (m % 10 == 1 && m % 100 != 11 ? 0 : m % 10 >= 2 && m % 10 <= 4 && (m % 100 < 10 || m % 100 >= 20) ? 1 : 2);
		}
	}
}
