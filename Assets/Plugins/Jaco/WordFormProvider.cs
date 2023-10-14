using System.Collections.Generic;

namespace Jaco
{
	public class WordFormProvider : IVarProvider
	{
		Dictionary<string, WordForm> vars = new Dictionary<string, WordForm>();
		List<string> modsForCall = new List<string>();

		public bool Add (WordForm wordForm) => wordForm != null && vars.TryAdd(wordForm.BaseForm, wordForm);

		//IVarProvider

		public bool TryGetValue (string varName, out string varValue, IReadOnlyList<string> mods = null)
		{
			if (varName == null || !vars.TryGetValue(varName, out WordForm wordForm))
			{
				varValue = default;
				return false;
			}

			if (mods != null && mods.Count > 0 && wordForm.numToMod != null)
			{
				//if numToMod method is specified and the modifier is a number
				//translate it to an according string modifier and add it to the list
				//otherwise just add the modifier to the list
				foreach (var m in mods) modsForCall.Add(int.TryParse(m, out int num) ? wordForm.numToMod(num) : m);
				
				varValue = wordForm.Get(modsForCall);
				modsForCall.Clear();
			}
			else varValue = wordForm.Get(mods);

			return true;
		}

		public bool Contains (string varName) => varName != null && vars.ContainsKey(varName);

		//

		public bool Remove (string varName) => varName != null && vars.Remove(varName);

		public bool Contains (IEnumerable<string> varNames)//does 'vars' contain any variable name from 'varNames'?
		{
			if (varNames == null) return false;
			foreach (var varName in varNames) if (Contains(varName)) return true;
			return false;
		}

		public void Clear () => vars.Clear();
	}
}
