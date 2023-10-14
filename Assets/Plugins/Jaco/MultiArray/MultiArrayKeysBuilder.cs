using System.Collections.Generic;

namespace Jaco
{
	public class MultiArrayKeysBuilder<K>
	{
		List<int> dimKeyCount;
		SortedList<K, KeyAddress> addr;

		public bool HasResult => addr != null && addr.Count > 0 && dimKeyCount != null && dimKeyCount.Count > 0;

		public MultiArrayKeysBuilder (int dimCount)
		{
			Reset(dimCount);
		}

		public bool AddKeyToNewDimension (K key)
		{
			dimKeyCount.Add(0);//add new dimension
			bool wasAdded = AddKey(key, dimKeyCount.Count - 1);//to last added dimension
			if (!wasAdded) dimKeyCount.RemoveAt(dimKeyCount.Count - 1);//remove last added dimension
			return wasAdded;
		}

		public bool AddKey (K key, int dim)
		{
			if (key == null) return false;

			//clamp
			if (dim < 0) dim = 0;
			else if (dim >= dimKeyCount.Count) dim = dimKeyCount.Count - 1;

			if (!addr.TryAdd(key, new KeyAddress(dim, dimKeyCount[dim]))) return false;
			dimKeyCount[dim]++;

			return true;
		}

		public void Reset (int dimCount)
		{
			if (dimKeyCount == null) dimKeyCount = new List<int>(dimCount);
			else dimKeyCount.Clear();

			//add dimesions
			while (--dimCount >= 0) dimKeyCount.Add(0);//fill

			if (addr == null) addr = new SortedList<K, KeyAddress>();
			else addr.Clear();
		}

		public IMultiArrayKeys<K> GetResult ()
		{
			return new MAKeys<K>(addr, dimKeyCount);
		}

		class MAKeys<T> : IMultiArrayKeys<T>
		{
			public int Size { get; private set; }
			public int Rank { get; private set; }

			public KeyAddress this[T key] => key != null && addr.TryGetValue(key, out KeyAddress result)
			  ? result
			  : KeyAddress.Invalid;

			readonly int[] dimKeyCount;
			readonly SortedList<T, KeyAddress> addr;

			//no args check, trust outer class
			public MAKeys (SortedList<T, KeyAddress> addr, List<int> dimKeyCount)
			{
				this.addr = new SortedList<T, KeyAddress>(addr);//copy, replase to Clone() when it will be implemented--!!!
				this.dimKeyCount = dimKeyCount.ToArray();//copy
				if (dimKeyCount.Count > 0)
				{
					Rank = dimKeyCount.Count;
					Size = 1;
					for (int i = 0; i < dimKeyCount.Count; ++i) Size *= dimKeyCount[i];
				}
				else
				{
					Rank = 1;
					Size = 0;
				}
			}

			public int GetKeyCount (int dimIdx) => dimKeyCount[dimIdx];
		}
	}
}
