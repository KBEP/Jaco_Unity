using System;
using System.Collections.Generic;
using System.Linq;

namespace Jaco.WordFormGeneration
{
	public class WordFormGenerator
	{
		KeyGenerator keyGen = new KeyGenerator();
		//stores all added word forms to avoid duplicates when adding new one
		HashSet<string> addedForms = new HashSet<string>();
		List<string> mods = new List<string>();
		Combiner<string> combiner;

		//generates a word form
		//argument 'version' is ignored now
		public WordForm GenWordForm (List<RecordData> recordDatas, int version = 0)
		{
			if (recordDatas == null) throw new ArgumentNullException(nameof(recordDatas));

			if (recordDatas.Count < 1) throw new ArgumentException($"'{nameof(recordDatas)}' item count must be > 1.");

			RecordData headerData = recordDatas[0];//the first item can be a header
			if (headerData == null)
			  throw new ArgumentException($"Null item is not allowed in '{nameof(recordDatas)}'.");

			//returns null if the first record is not a header
			bool hasHeader = WordFormHeader.TryParse(headerData.values[0], out WordFormHeader header);
			if (!hasHeader) header = WordFormHeader.Empty;

			using (var etor = recordDatas.GetEnumerator())
			{
				if (hasHeader && !etor.MoveNext()) throw new Exception($"No enough items in '{nameof(recordDatas)}'.");
				if (!etor.MoveNext()) throw new Exception($"No enough items in '{nameof(recordDatas)}'.");

				RecordData first = etor.Current;

				if (first == null) throw new Exception($"Null item in '{nameof(recordDatas)}'.");
				if (first.values.Count < 1) throw new Exception(string.Format("No values (line {0}).", first.line));

				IMultiArrayKeys<string> keys = null;
				MultiArray<string, string> arr = null;

				addedForms.Clear();

				if (first.values[0].All(char.IsWhiteSpace))//table format #1
				{
					//create multy-array keys
					keys = keyGen.GenKeys(recordDatas, hasHeader);

					//create and fill a multi-array
					arr = new MultiArray<string, string>(keys);

					List<string> rowMods = GenRowMods(first, hasHeader);
					List<string> columnMods = GenColumnMods(recordDatas, hasHeader);

					int rowIdx = hasHeader ? 2 : 1;

					foreach (var c in columnMods)
					{
						int columnIdx = 1;
						foreach (var r in rowMods)
						{
							mods.Clear();
							mods.Add(r);
							mods.Add(c);

							string w = recordDatas[rowIdx].values[columnIdx];
							if (addedForms.TryGetValue(w, out string same)) w = same;
							else addedForms.Add(w);
							arr.Set(mods, w);
							columnIdx++;
						}
						rowIdx++;
					}
				}
				else//table format #2
				{
					List<WordFormData> wfds = RecordBlockToWordFormData(recordDatas, hasHeader);

					//create multy-array keys
					keys = keyGen.GenKeys(wfds);

					//create and fill a multi-array
					arr = new MultiArray<string, string>(keys);

					mods.Clear();

					foreach (var wfd in wfds)
					{
						if (combiner != null) combiner.Reset(wfd.modLists);
						else combiner = new Combiner<string>(wfd.modLists);

						while (combiner.TryGetCombination(ref mods))
						{
							string w = arr.Get(mods);
							if (w != null)
							{
								string mods_list = string.Join(',', mods);
								string m = string.Format("Value defined twice (keys: {0}).", mods_list);
								throw new Exception(m);
							}
							w = wfd.word;
							if (addedForms.TryGetValue(w, out string same)) w = same;
							else addedForms.Add(w);
							arr.Set(mods, w);
						}
					}

					int udefinedCount = arr.Replace(null, string.Empty);//count of undefined forms

					if (udefinedCount != 0)
					{
						string m = string.Format("Some forms are not defined (count: {0}).", udefinedCount);
						throw new Exception(m);
					}
				}

				return new WordForm(header, arr);
			}
		}

		List<WordFormData> RecordBlockToWordFormData (List<RecordData> recordDatas, bool skipFirst)
		{
			if (recordDatas == null) throw new ArgumentNullException(nameof(recordDatas));
			if (recordDatas.Count < 1) throw new ArgumentException($"'{nameof(recordDatas)}' item count must be > 1.");

			List<WordFormData> result = new List<WordFormData>();

			//expected count of values in each record (except header record)
			//-1 means the value is not defined yet
			int valueCount = -1;

			string baseForm = null;//for WordFormData ctors

			foreach (var data in recordDatas.Skip(skipFirst ? 1 : 0))//skip header record if it exists
			{
				if (data == null) throw new ArgumentException($"Null item is not allowed in '{nameof(recordDatas)}'.");
				if (valueCount != -1 && valueCount != data.values.Count)
				{
					string m = string.Format("Count of values in the records must be the same (line {0}).", data.line);
					throw new ArgumentException(m);
				}
				else//not defined yet
				{
					valueCount = data.values.Count;
				}
				WordFormData wfd = new WordFormData(data.values, baseForm);
				if (baseForm == null) baseForm = wfd.word;//first added wfd.word will be a base form other wfds.
				result.Add(wfd);
			}

			return result;
		}

		List<string> GenRowMods (RecordData recordData, bool skipFirst)
		{
			if (recordData == null) throw new ArgumentNullException(nameof(recordData));

			List<string> result = new List<string>();

			foreach (var v in recordData.values.Skip(skipFirst ? 1 : 0))
			{
				if (result.Contains(v)) throw new Exception(string.Format("Mods must be unique '{0}'.", v));
				result.Add(v);
			}

			return result;
		}

		List<string> GenColumnMods (List<RecordData> recordDatas, bool skipFirst)
		{
			if (recordDatas == null) throw new ArgumentNullException(nameof(recordDatas));

			List<string> result = new List<string>();

			foreach (var rd in recordDatas.Skip(skipFirst ? 2 : 1))
			{
				if (rd == null) throw new Exception($"Null item in '{nameof(recordDatas)}'.");
				if (rd.values.Count < 1) throw new Exception(string.Format("No values (line {0}).", rd.line));
				string mod = rd.values[0];
				if (result.Contains(mod)) throw new Exception(string.Format("Mods must be unique '{0}'.", mod));
				result.Add(mod);
			}

			return result;
		}
	}
}
