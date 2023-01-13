using System.Collections.Generic;

namespace Jaco.WordFormGeneration
{
	// Header pattern:
	// [ key0 = value0; key1 = value1; ... ; keyN = valueN ]
	// Keys/values can not be empty string

	public static class HeaderParser
	{
		public const char SEP = ';';
		public const char OPEN_BRACE = '[';
		public const char CLOSE_BRACE = ']';

		static readonly char[] braces = { OPEN_BRACE, CLOSE_BRACE };
		static readonly SequenceScanner scanner = new SequenceScanner();
		static readonly ArgParser argParser = new ArgParser();

		public static Dictionary<string, string> Parse (string header)
		{
			scanner.Init(header, braces );

			Sequence ob = scanner.GetNext();

			if (ob.length <= 0 || header[ob.start] != OPEN_BRACE)
			{
				scanner.Clear();
				return null;
			}

			Sequence text = scanner.GetNext();

			if (text.length <= 0 || header[text.start] == OPEN_BRACE || header[text.start] == CLOSE_BRACE)
			{
				scanner.Clear();
				return null;
			}

			Sequence cb = scanner.GetNext();

			if (cb.length <= 0 || header[cb.start] != CLOSE_BRACE)
			{
				scanner.Clear();
				return null;
			}

			Sequence empty = scanner.GetNext();

			if (empty.length > 0)
			{
				scanner.Clear();
				return null;
			}

			scanner.Init(header, SEP, text.start, text.length);

			bool expectSep = false;
			Dictionary<string, string> result = null;

			for (;;)
			{
				Sequence s = scanner.GetNext().Trim(header);
				if (s.length <= 0) break;//no more key-value pairs
				else if (header[s.start] == SEP)
				{
					if (expectSep)
					{
						expectSep = false;
						continue;
					}
					else//unexpected ';'
					{
						scanner.Clear();
						return null;
					}
				}
				else//sequence is a key-value pair
				{
					if (argParser.TryParse(s, header, out KeyValuePair<string, string> pair))
					{
						if (result == null)
						{
							result = new Dictionary<string, string>();
							result.Add(pair.Key, pair.Value);
						}
						else if (!result.TryAdd(pair.Key, pair.Value))//already added
						{
							scanner.Clear();
							return null;
						}
					}
					else//key-value pair parsing failed
					{
						scanner.Clear();
						return null;
					}
					expectSep = true;//next sequence must be s';'
				}
			}

			scanner.Clear();
			return result;
		}
	}
}
