using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Jaco.XSV;

namespace Jaco.WordFormGeneration
{
	public class RecordBlockReader : IDisposable
	{
		RecordReader reader;
		bool isDisposed;

		RecordBlockReader (RecordReader reader)
		{
			this.reader = reader;
		}

		public static RecordBlockReader FromStream (Stream stream, DataFormat format)
		{
			RecordReader reader = RecordReader.FromStream(stream, format);
			return new RecordBlockReader(reader);
		}

		public static RecordBlockReader FromFile (string path, DataFormat format,
		  Encoding encoding, bool detectEncodingFromByteOrderMarks)
		{
			RecordReader reader
			  = RecordReader.FromFile(path, format, encoding, detectEncodingFromByteOrderMarks);
			return new RecordBlockReader(reader);
		}

		public static RecordBlockReader FromFile (string path, DataFormat format)
		{
			RecordReader reader = RecordReader.FromFile(path, format);
			return new RecordBlockReader(reader);
		}

		public static RecordBlockReader FromString (string source, DataFormat format)
		{
			RecordReader reader = RecordReader.FromString(source, format);
			return new RecordBlockReader(reader);
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

		public bool TryGetNext (out List<List<string>> recordBlock)
		{
			recordBlock = GetNext();
			return recordBlock != null;
		}

		public List<List<string>> GetNext ()
		{
			List<List<string>> result = null;//result record block
			int valueCount = -1;

			for (;;)
			{
				List<string> values = GetNextRecordValues(true);
				if (values == null)//нет больше записей
				{
					return result;
				}
				else if (values.Count <= 0)//пустая запись
				{
					if (result == null || result.Count <= 0)//ни одной непустой записи ещё не было прочитано
					{
						continue;
					}
					else
					{
						return result;
					}
				}
				else
				{
					if (valueCount != -1)//уже определено
					{
						if (valueCount != values.Count) throw new Exception("All record lengths must be equal.");
					}
					else
					{
						valueCount = values.Count;//устанавливаем ширину блока
					}
					if (result == null) result = new List<List<string>>();
					result.Add(values);
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

		~RecordBlockReader ()
		{
			Dispose(false);
		}
	}
}
