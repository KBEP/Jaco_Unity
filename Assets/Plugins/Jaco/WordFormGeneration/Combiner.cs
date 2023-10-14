using System;
using System.Collections.Generic;

namespace Jaco.WordFormGeneration
{
	public class Combiner<T>
	{
		IReadOnlyCollection<IReadOnlyList<T>> sets;
		int max;//max combination count
		int next;//next combination index

		//expects 'sets' not to be changed from the outside, otherwise the TryGetCombination() will not work correct
		public Combiner (IReadOnlyCollection<IReadOnlyList<T>> sets)
		{
			Reset(sets);
		}

		public static bool IsSetsValid (IReadOnlyCollection<IReadOnlyList<T>> sets)
		{
			if (sets == null) return false;
			foreach (var s in sets) if (s == null || s.Count <= 0) return false;
			return true;
		}

		//remove null/empty lists
		public static bool TryMakeSetsValid (ref List<IReadOnlyList<T>> sets)
		{
			if (sets == null) return false;
			sets.RemoveAll(s => s == null || s.Count <= 0);
			return sets.Count > 0;
		}

		public void Reset (IReadOnlyCollection<IReadOnlyList<T>> sets)//use new sets
		{
			if (!IsSetsValid(sets))
			  throw new ArgumentException($"Argument '{nameof(sets)}' is invalid (null or has null/empty values).");

			this.sets = sets;

			this.max = 1;
			foreach (var s in sets) this.max *= s.Count;//calculate max combination count

			Reset();
		}

		public void Reset ()//start from first combination
		{
			next = 0;
		}

		public bool TryGetCombination (ref List<T> result)
		{
			if (next >= max)//no more combination
			{
				if (result != null) result.Clear();
				return false;
			}

			if (result == null) result = new List<T>();
			else result.Clear();

			int div = max;//divider
			int n = next;

			foreach (var s in sets)
			{
				div /= s.Count;
				int j = n / div;
				n -= j * div;
				result.Add(s[j]);
			}

			next++;

			return true;
		}
	}
}
