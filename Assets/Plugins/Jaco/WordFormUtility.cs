using System;
using System.Collections.Generic;
using Jaco.WordFormGeneration;
using Jaco.XSV;

namespace Jaco
{
	public static class WordFormUtility
	{
		//adds word forms from file 'path' to dictionary 'destination', returns count of added word forms
		public static int AddWordForms (string path, ref Dictionary<string, WordForm> destination)
		{
			int count = 0;

			using (var reader = WordFormBlockReader.FromFile(path, DataFormat.tsv))
			{
				if (destination == null) destination = new Dictionary<string, WordForm>();

				WordFormGenerator wfg = new WordFormGenerator();
				int version = 0;//tsv file format version

				//recordDatas is probably file header
				if (!reader.TryGetNext(out List<RecordData> fileHeaderData)
				  || fileHeaderData.Count != 1
				  || fileHeaderData[0].values.Count <= 0
				  || !WordFormHeader.TryParse(fileHeaderData[0].values[0], out WordFormHeader fileHeader)
				  || !fileHeader.values.TryGetValue("version", out string versionStr)
				  || !int.TryParse(versionStr, out version)) throw new Exception("Invalid file header.");

				while (reader.TryGetNext(out List<RecordData> recordDatas))
				{
					WordForm w = wfg.GenWordForm(recordDatas, version);
					if (!destination.TryAdd(w.BaseForm, w))
					  throw new Exception(string.Format("Word form {0} already defined.", w.BaseForm));
					count++;
				}
			}

			return count;		
		}

		//adds word forms from file 'path' to 'provider', returns count of added word forms
		public static int AddWordForms (string path, ref WordFormProvider provider, bool clear)
		{
			int count = 0;

			using (var reader = WordFormBlockReader.FromFile(path, DataFormat.tsv))
			{
				if (provider == null) provider = new WordFormProvider();
				else if (clear) provider.Clear();

				WordFormGenerator wfg = new WordFormGenerator();
				int version = 0;//tsv file format version

				//recordDatas is probably file header
				if (!reader.TryGetNext(out List<RecordData> fileHeaderData)
				  || fileHeaderData.Count != 1
				  || fileHeaderData[0].values.Count <= 0
				  || !WordFormHeader.TryParse(fileHeaderData[0].values[0], out WordFormHeader fileHeader)
				  || !fileHeader.values.TryGetValue("version", out string versionStr)
				  || !int.TryParse(versionStr, out version)) throw new Exception("Invalid file header.");

				while (reader.TryGetNext(out List<RecordData> recordDatas))
				{
					WordForm w = wfg.GenWordForm(recordDatas, version);
					if (!provider.Add(w))
					  throw new Exception(string.Format("Word form {0} already defined.", w.BaseForm));
					count++;
				}
			}

			return count;		
		}
	}
}
