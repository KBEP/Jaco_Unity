namespace Jaco
{
	public readonly struct Sequence//close equivalent of ReadOnlySpan<char>
	{
		public readonly int start;
		public readonly int length;

		public int End => start + length - 1;

		public static Sequence Empty => new Sequence(0, 0);

		public Sequence (int start, int length)
		{
			this.start = start >= 0 ? start : 0;
			this.length = length >= 0 ? length : 0;
		}

		public string GenString (string source) =>
		  source != null && length + start <= source.Length
		  ? source.Substring(start, length)
		  : null;

		public Sequence Trim (string source)
		{
			if (source == null) new Sequence(this.start, 0);

			int start = this.start;
			int end = this.End;

			while (start <= end && char.IsWhiteSpace(source[start])) start++;
			while (end > start && char.IsWhiteSpace(source[end])) end--;

			return new Sequence(start, end - start + 1);
		}

		public bool Equals (Sequence s) => start == s.start && length == s.length;

		public override bool Equals (object o) => o is Sequence s && Equals(s);

		public override int GetHashCode () => start ^ length;

		public override string ToString () => string.Format("({0},{1})", start, length);

		public static bool operator == (Sequence s1, Sequence s2) =>  s1.Equals(s2);

		public static bool operator != (Sequence s1, Sequence s2) => !s1.Equals(s2);
	}
}
