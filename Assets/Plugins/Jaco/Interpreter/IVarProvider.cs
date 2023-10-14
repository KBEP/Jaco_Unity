using System.Collections.Generic;

namespace Jaco
{
	public interface IVarProvider
	{
		bool TryGetValue (string varName, out string varValue, IReadOnlyList<string> args = null);

		bool Contains (string varName);
	}
}
