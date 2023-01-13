using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jaco.WordFormGeneration;

namespace Jaco
{
	public class WordFormHeader
	{
		public static readonly WordFormHeader Empty = new WordFormHeader(null);

		public readonly ReadOnlyDictionary<string, string> values;

		public bool IsEmpty => values.Count <= 0;

		//WARNING: Ctr does not create copy of 'values' dictionary, it can still be changed from the outside!
		public WordFormHeader (Dictionary<string, string> values)
		{
			if (values == null) values = new Dictionary<string, string>();
			this.values = new ReadOnlyDictionary<string, string>(values);
		}

		//returns default if parsing failed
		public static bool TryParse (string headerString, out WordFormHeader result)
		{
			Dictionary<string, string> values = HeaderParser.Parse(headerString);
			if (values != null)
			{
				result = new WordFormHeader(values);
				return true;
			}
			else
			{
				result = default;
				return false;
			}
		}
	}
}
