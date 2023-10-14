using System.Collections.Generic;

namespace Jaco.WordFormGeneration
{
	public class ArgParser
	{
		// Argument key-value pair pattern:
		// key = value
		// Key/value can not be empty string

		public const char DEFAULT_SEPARATOR = '=';

		readonly SequenceScanner scanner;
		readonly char separator;

		public ArgParser (char separator = DEFAULT_SEPARATOR)
		{
			this.scanner = new SequenceScanner();
			this.separator = separator;
		}

		public bool TryParse (in Sequence sequence, string source, out KeyValuePair<string, string> result)
		{
			scanner.Init(source, separator, sequence.start, sequence.length);

			Sequence key = scanner.GetNext().Trim(source);//expect key text

			if (key.length <= 0 || source[key.start] == separator)//empty/null source or unexpected '='
			{
				result = default;
				scanner.Clear();
				return false;
			}

			Sequence s = scanner.GetNext().Trim(source);//expect '='

			if (s.length <= 0 || source[s.start] != separator)//no more sequences or the sequence is not '='
			{
				result = default;
				scanner.Clear();
				return false;
			}

			Sequence value = scanner.GetNext().Trim(source);//expect value text

			if (value.length <= 0 || source[value.start] == separator)//no more sequences or unexpected '='
			{
				result = default;
				scanner.Clear();
				return false;
			}

			result = new KeyValuePair<string, string>(key.GenString(source), value.GenString(source));
			scanner.Clear();
			return true;
		}
	}
}
