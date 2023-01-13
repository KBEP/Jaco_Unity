using System.Collections.Generic;

namespace Jaco
{
	// Интерфейс Rank-размерного массива с доступом к значению TValue по ключам TKey.
	public interface IMultiArray<TKey, TValue>
	{
		int Rank { get; }
		int Size { get; }
		TValue Get (IEnumerable<TKey> keys);
		void Set (IEnumerable<TKey> keys, TValue value);
	}
}
