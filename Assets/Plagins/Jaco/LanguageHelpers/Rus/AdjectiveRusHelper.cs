//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

namespace Jaco
{
	public static class AdjectiveRusHelper
	{
		static readonly string[] numbers = { "ед", "мн" };
		
		public static string NumToMod (int number) => numbers[CalcPluralFormIdx(number)];

		static int CalcPluralFormIdx (int number) => number == 1 || number == -1 ? 0 : 1;
	}
}