using System;
using System.IO;
using System.Text;

namespace Jaco.XSV
{
	public class RecordReader : IDisposable
	{
		Reader reader;
		RecordEnumerator recordEnumerator;
		bool isDisposed;

		public FakeRecordCollection Records => new FakeRecordCollection(ref recordEnumerator);

		RecordReader (Reader reader)
		{
			this.reader = reader;
			this.recordEnumerator = new RecordEnumerator(this.reader);
		}

		public static RecordReader FromStream (Stream stream, DataFormat format)
		{
			Reader reader = Reader.FromStream(stream, format);
			return new RecordReader(reader);
		}

		public static RecordReader FromFile (string path, DataFormat format,
		  Encoding encoding, bool detectEncodingFromByteOrderMarks)
		{
			Reader reader = Reader.FromFile(path, format, encoding, detectEncodingFromByteOrderMarks);
			return new RecordReader(reader);
		}

		public static RecordReader FromFile (string path, DataFormat format)
		{
			Reader reader = Reader.FromFile(path, format);
			return new RecordReader(reader);
		}

		public static RecordReader FromString (string source, DataFormat format)
		{
			Reader reader = Reader.FromString(source, format);
			return new RecordReader(reader);
		}

		public bool TryReadOne (out Record record)
		{
			bool hasRecord = recordEnumerator.MoveNext();
			record = hasRecord ? recordEnumerator.Current : Record.Invalid;
			return hasRecord;
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

		~RecordReader ()
		{
			Dispose(false);
		}
	}
}
