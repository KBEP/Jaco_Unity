namespace Jaco.WordGeneration
{
	readonly struct Op
	{
		public readonly Sequence value;
		public readonly OP_TYPE type;

		public Op (OP_TYPE type, in Sequence value)
		{
			this.type = type;
			this.value = value;
		}
	}
}
