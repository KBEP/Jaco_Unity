using System;
using System.Collections.Generic;
using System.Linq;

namespace Jaco.WordFormGeneration
{
	//generate multi-array keys from word form data collection
	public class KeyGenerator
	{
		MultiArrayKeysBuilder<string> keyBuilder;

		public IMultiArrayKeys<string> GenKeys (IEnumerable<WordFormData> wordFormData)
		{
			if (wordFormData == null) throw new ArgumentNullException(nameof(wordFormData));

			int dimCount = -1;//dimensions count
			foreach (var wfd in wordFormData)//just for get dimCount
			{
				dimCount = wfd.modLists.Count;
				break;
			}
			if (dimCount == -1) throw new ArgumentException($"'{nameof(wordFormData)}' can not be empty.");

			if (keyBuilder == null) keyBuilder = new MultiArrayKeysBuilder<string>(dimCount);
			else keyBuilder.Reset(dimCount);

			foreach (var wfd in wordFormData)
			{
				if (wfd.modLists.Count != dimCount) throw new Exception("All modifier list counts must be equal.");				
				int dim = 0;//dimension
				foreach (var modList in wfd.modLists)
				{
					foreach (var mod in modList) keyBuilder.AddKey(mod, dim);
					dim++;
				}
			}

			return keyBuilder.GetResult();
		}

		public IMultiArrayKeys<string> GenKeys (IEnumerable<RecordData> recordDatas, bool skipFirst)
		{
			if (recordDatas == null) throw new ArgumentNullException(nameof(recordDatas));

			using (var etor = recordDatas.GetEnumerator())
			{
				//skip first item because it is a header
				if (skipFirst && !etor.MoveNext()) throw new Exception($"No enough items in '{nameof(recordDatas)}'.");

				if (keyBuilder == null) keyBuilder = new MultiArrayKeysBuilder<string>(2);//2D-table
				else keyBuilder.Reset(2);

				if (!etor.MoveNext()) throw new Exception($"No enough items in '{nameof(recordDatas)}'.");
				if (etor.Current == null) throw new Exception($"Null item in '{nameof(recordDatas)}'.");

				//through the first record, skiping first empty value
				foreach (var v in etor.Current.values.Skip(1))
				{
					if (!keyBuilder.AddKey(v, 0))//0 means adding as row
					{
						string m = string.Format("Failed to add key '{0}' (line {1}).", v, etor.Current.line);
						throw new Exception(m);
					}
				}

				int rowsCount = 0;//count of rows read

				while (etor.MoveNext())
				{
					RecordData cur = etor.Current;

					if (cur == null) throw new Exception($"Null item in '{nameof(recordDatas)}'.");
					if (cur.values.Count < 1) throw new Exception(string.Format("No values (line {0}).", cur.line));

					string firstValue = cur.values[0];

					if (!keyBuilder.AddKey(firstValue, 1))//1 means adding as column
					{
						string m = string.Format("Failed to add key '{0}' (line {1}).", firstValue, etor.Current.line);
						throw new Exception(m);
					}

					rowsCount++;
				}

				//no rows read
				if (rowsCount <= 0) throw new Exception($"No enough items in '{nameof(recordDatas)}'.");

				return keyBuilder.GetResult();
			}
		}
	}
}
