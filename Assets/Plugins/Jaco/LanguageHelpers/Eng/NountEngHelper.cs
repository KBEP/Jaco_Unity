namespace Jaco
{
	public static class NounEngHelper
	{
		static readonly string[] numbers = { "sn", "pl" };//singular and plural
		
		public static string NumToMod (int number) => numbers[CalcPluralFormIdx(number)];
		
		static int CalcPluralFormIdx (int value) => value == 1 || value == -1 ? 0 : 1;
	}
}
