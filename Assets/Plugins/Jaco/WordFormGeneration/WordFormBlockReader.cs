using Jaco.XSV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jaco.WordFormGeneration
{
	public class WordFormBlockReader : IDisposable
	{
		RecordReader reader;
		int line;
		bool isDisposed;

		WordFormBlockReader (RecordReader reader)
		{
			this.reader = reader;
			this.line = 0;
			this.isDisposed = false;
		}

		public static WordFormBlockReader FromStream (Stream stream, DataFormat format)
		{
			RecordReader reader = RecordReader.FromStream(stream, format);
			return new WordFormBlockReader(reader);
		}

		public static WordFormBlockReader FromFile (string path, DataFormat format,
		  Encoding encoding, bool detectEncodingFromByteOrderMarks)
		{
			RecordReader reader
			  = RecordReader.FromFile(path, format, encoding, detectEncodingFromByteOrderMarks);
			return new WordFormBlockReader(reader);
		}

		public static WordFormBlockReader FromFile (string path, DataFormat format)
		{
			RecordReader reader = RecordReader.FromFile(path, format);
			return new WordFormBlockReader(reader);
		}

		public static WordFormBlockReader FromString (string source, DataFormat format)
		{
			RecordReader reader = RecordReader.FromString(source, format);
			return new WordFormBlockReader(reader);
		}

		public bool TryGetNext (out List<RecordData> recordDatas)
		{
			recordDatas = GetNext();
			return recordDatas != null;
		}

		void TrimEnd (List<string> list)//удаляет пустые значения в конце списка
		{
			if (list == null) return;

			for (int i = list.Count - 1; i >= 0; --i)
			{
				string s = list[i];
				if (s == string.Empty || s == null) list.RemoveAt(i);
				else return;
			}
		}

		RecordData GetNextRecordData (bool trimEnd)
		{
			List<string> recordValues = GetNextRecordValues(trimEnd);
			if (recordValues == null) return null;
			else return new RecordData(++line, recordValues);
		}

		List<string> GetNextRecordValues (bool trimEnd)
		{
			if (reader.TryReadOne(out Record record))
			{
				var result = new List<string>(record);
				if (trimEnd) TrimEnd(result);
				return result;
			}
			else return null;
		}

		List<RecordData> GetNext ()//returns next block of records
		{
			List<RecordData> result = null;//result block

			for (;;)
			{
				RecordData recordData = GetNextRecordData(true);
				if (recordData == null) return result;//нет больше записей
				else if (recordData.values.Count <= 0)//пустая запись
				{
					if (result == null || result.Count <= 0) continue;//ни одной непустой записи ещё не было прочитано
					else return result;
				}
				else
				{
					if (result == null) result = new List<RecordData>();
					result.Add(recordData);
				}
			}
		}

		public void Dispose ()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose (bool doDisposing)
		{
			if(isDisposed) return;
			if(doDisposing) reader?.Dispose();
			reader = null;
			isDisposed = true;
		}

		~WordFormBlockReader ()
		{
			Dispose(false);
		}
	}
}
