using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Jaco
{
	public static class ModHelper
	{
		static readonly ReadOnlyDictionary<string, Func<int, string>> numToModMethods;

		static ModHelper ()
		{
			Dictionary<string, Func<int, string>> tmp = new Dictionary<string, Func<int, string>>();

			//rus
			tmp.Add("noun_rus", NounRusHelper.NumToMod);
			tmp.Add("adjective_rus", AdjectiveRusHelper.NumToMod);

			tmp.Add("noun_eng", NounEngHelper.NumToMod);

			//eng
			numToModMethods = new ReadOnlyDictionary<string, Func<int, string>>(tmp);
		}

		public static Func<int, string> GetNumToModMethod (string name)
		{
			return name != null && numToModMethods.TryGetValue(name, out Func<int, string> method) ? method : null;
		}
	}
}
