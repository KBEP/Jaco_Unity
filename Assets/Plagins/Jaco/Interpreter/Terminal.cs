namespace Jaco
{
	public readonly struct Terminal
	{
		public static readonly Terminal Empty = new Terminal(TERMINAL_TYPE.EMPTY, string.Empty);

		public readonly TERMINAL_TYPE type;
		public readonly Sequence sequence;
		public readonly string source;

		public Terminal (TERMINAL_TYPE type, Sequence sequence, string source)
		{
			this.type = type;
			this.source = source ?? string.Empty;
			int start = Clamp(sequence.start, 0, this.source.Length - 1);
			int length = Clamp(sequence.length, 0, this.source.Length - start);
			this.sequence = new Sequence(start, length);
		}

		public Terminal (TERMINAL_TYPE type, string source)
		{
			this.type = type;
			this.source = source ?? string.Empty;
			this.sequence = new Sequence(0, this.source.Length);
		}

		public string GenString ()
		{
			return source.Substring(sequence.start, sequence.length);
		}

		static int Clamp (int value, int min, int max)
		{
			if (max < min)// swap
			{
				min = min ^ max;
				max = max ^ min;
				min = min ^ max;
			}
			return min <= value ? value <= max ? value : max : min;
		}
	}
}
