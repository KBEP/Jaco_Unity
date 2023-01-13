namespace Jaco.WordGeneration
{
	readonly struct Terminal
	{
		public readonly Sequence sequence;
		public readonly TERMINAL_TYPE type;

		public Terminal (TERMINAL_TYPE type, in Sequence sequence)
		{
			this.type = type;
			this.sequence = sequence;
		}
	}
}
