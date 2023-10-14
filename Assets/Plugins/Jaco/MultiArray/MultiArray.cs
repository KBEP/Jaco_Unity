using System.Collections.Generic;
using System;

namespace Jaco
{
	// Особенности работы методов Get и Set:
	//  - Если в аргументе не найдены ключ для соответствующей размерности, то используется первый ключ в этой размерности.
	//  - Если для размерности найдено два и более подходящих ключа, то используется последний найденный.
	public class MultiArray<K, V> : IMultiArray<K, V>
	{
		V[] values;
		IMultiArrayKeys<K> keys;
		int[] indices;

		public int Rank => keys.Rank;
		public int Size => keys.Size;

		public MultiArray (IMultiArrayKeys<K> keys)
		{
			if (keys == null) throw new ArgumentNullException(nameof(keys));
			this.keys = keys;
			this.values = new V[this.keys.Size];
			this.indices = new int[this.keys.Rank];
		}

		public V Get (IEnumerable<K> keys)
		{
			int idx = GenFlatIdx(keys);
			return values[idx];
		}

		public void Set (IEnumerable<K> keys, V value)
		{
			int idx = GenFlatIdx(keys);
			values[idx] = value;
		}

		public V GetFirst () => values[0];

		public V SetFirst (V value) => values[0] = value;

		public int Replace (V oldValue, V newValue)
		{
			int count = 0;
			for (int i = 0; i < values.Length; ++i)
			{
				if (values[i].Equals(oldValue))
				{
					values[i] = newValue;
					++count;
				}
			}
			return count;
		}

		int GenFlatIdx (IEnumerable<K> keys)
		{
			if (keys == null) return 0;
			SetIndicesToDefault();
			foreach (var k in keys)
			{
				KeyAddress a = this.keys[k];
				if (!a.Equals(KeyAddress.Invalid)) indices[a.dim] = a.idx;
			}
			int result = indices[0];
			for (int i = 1; i < indices.Length; ++i) result = result * this.keys.GetKeyCount(i) + indices[i];
			return result;
		}

		void SetIndicesToDefault ()
		{
			for (int i = 0; i < indices.Length; ++i) indices[i] = 0;
		}
	}
}
