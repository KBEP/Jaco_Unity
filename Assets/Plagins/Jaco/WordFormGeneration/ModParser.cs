using System.Collections.Generic;

namespace Jaco.WordFormGeneration
{
	public static class ModParser
	{
		const char sep = ',';
		static readonly char[] sepArray = { sep };//for SequenceScanner.Init()
		static readonly SequenceScanner scanner = new SequenceScanner();
		static readonly Collector collector = new Collector();

		//returns collection of unique modifiers parsed from 'source' string
		public static List<string> ParseUnique (string source)
		{
			if (source == null) return null;
			scanner.Init(source, sepArray);

			for (;;)
			{
				Sequence s = scanner.GetNext();
				if (s.length <= 0) break;//no more values
				if (source[s.start] == sep && s.length == 1) continue;//it is a separator, skip it
				else collector.Add(s.Trim(source), source);//try add trimmed value			
			}

			List<string> result = collector.Generate();

			collector.Clear();
			scanner.Clear();

			return result;
		}

		class Collector
		{
			List<string> strings = new List<string>();
			
			public void Add (in Sequence sequence, string source)
			{
				foreach (var s in strings)
				  if (string.Compare(s, 0, source, sequence.start, sequence.length) == 0) return;
				
				strings.Add(sequence.GenString(source));
			}

			public List<string> Generate () => new List<string>(strings);

			public void Clear () => strings.Clear();
		}
	}
}
