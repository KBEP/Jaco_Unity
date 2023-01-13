using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jaco.WordGeneration;

namespace Jaco.WordFormGeneration
{
	public class WordFormData
	{
		public readonly ReadOnlyCollection<ReadOnlyCollection<string>> modLists;//modifiers
		public readonly string word;

		//pass null as 'baseForm' if it is not specified
		public WordFormData (IReadOnlyList<string> record, string baseForm)
		{
			if (record == null) throw new ArgumentNullException(nameof(record));
			if (record.Count < 2) throw new ArgumentException("Count of record's values must be 2 or greater.");

			//generate modifiers

			int modListCount = record.Count - 1;
			List<ReadOnlyCollection<string>> tmpModLists = new List<ReadOnlyCollection<string>>(modListCount);

			for (int i = 0; i < record.Count - 1; ++i)//except last, it is a word form, not list of modifiers
			{
				ReadOnlyCollection<string> m = ModParser.ParseUnique(record[i])?.AsReadOnly();//a modifiers list
				if (m == null || m.Count <= 0)
				  throw new Exception(string.Format("Can not generate modifiers list from {0}.", record[i]));
				tmpModLists.Add(m);
			}

			this.modLists = tmpModLists.AsReadOnly();

			//set a word form

			if (baseForm != null)//base form specified - generate word form based on it
			{
				string pattern = record[record.Count - 1];//pattern or value
				GEN_RESULT genResult = WordGenerator.GenForm(baseForm, pattern, out string strResult);
				if (genResult == GEN_RESULT.SUCCESS) this.word = strResult;
				else
				{
					string m = string.Format("Failed to generate word from ({0}/{1}).", baseForm, pattern);
					throw new Exception(m);
				}
			}
			else this.word = record[record.Count - 1];//set value as is

			if (this.word == null) this.word = string.Empty;
		} 
	}
}
