using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Jaco.WordFormGeneration
{
	public class RecordData
	{
		public readonly int line;
		public readonly ReadOnlyCollection<string> values;

		public RecordData (int line, IEnumerable<string> values)
		{			
			this.line = line;

			List<string> list = new List<string>();
			if (values != null) foreach (var v in values) if (v != null) list.Add(v);
			this.values = list.AsReadOnly();
		}
	}
}
