using System.Collections.Generic;

namespace Jaco
{
	public class VarProvider : IVarProvider
	{
		Dictionary<string, DDynVar> vars = new Dictionary<string, DDynVar>();

		public bool Add (string varName, DDynVar value)
		  => TerminalUtility.IsValidVarName_NoMark(varName) && value != null && vars.TryAdd(varName, value);

		public bool Remove (string varName) => varName != null && vars.Remove(varName);

		public bool TryGetValue (string varName, out string varValue, IReadOnlyList<string> args = null)
		{
			if (varName != null && vars.TryGetValue(varName, out DDynVar method) && method != null)
			{
				varValue = method(args);
				return true;
			}
			else
			{
				varValue = default;
				return false;
			}
		}

		public bool Contains (string varName) => varName != null && vars.ContainsKey(varName);

		public void Clear () => vars.Clear();
	}
}
