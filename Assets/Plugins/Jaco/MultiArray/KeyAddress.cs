namespace Jaco
{
	public readonly struct KeyAddress
	{
		public static readonly KeyAddress Invalid = new KeyAddress(-1, -1);

		public readonly int dim;
		public readonly int idx;

		public KeyAddress (int dim, int idx)
		{
			this.dim = dim;
			this.idx = idx;
		}

		public override string ToString () => string.Format("(dim:{0}, idx:{1})", dim, idx);

		public override int GetHashCode () => System.HashCode.Combine<int, int>(dim, idx);

		public override bool Equals (object obj) => obj is KeyAddress k && Equals(k);

		public bool Equals (KeyAddress k) => dim == k.dim && idx == k.idx;

		public static bool operator == (KeyAddress k1, KeyAddress k2) =>  k1.Equals(k2);

		public static bool operator != (KeyAddress k1, KeyAddress k2) => !k1.Equals(k2);
	}
}
