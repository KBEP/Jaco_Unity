using System;
using System.Collections;
using System.Collections.Generic;

namespace Jaco.XSV
{
	public struct RecordEnumerator : IEnumerator<Record>
	{
		Reader reader;
		Record current;

		public Record Current => current;

		public RecordEnumerator (Reader reader)
		{
			this.reader = reader;
			this.current = Record.Invalid;
		}

		Record MoveToNextRecordStart ()
		{
			for (;;)
			{
				Element e = reader.Read();
				if (e.type == ELEMENT_TYPE.REC_DEL)
				{
					return new Record(reader);
				}
				else if (e.type == ELEMENT_TYPE.TABLE_END)
				{
					return Record.Invalid;
				}
			}
		}

		//

		object IEnumerator.Current => Current;

		void IEnumerator.Reset ()
		{
			throw new NotSupportedException("Reset()");
		}

		public bool MoveNext ()
		{
			current = current.IsValid ? MoveToNextRecordStart() : new Record(reader);
			return current.IsValid;
		}

		void IDisposable.Dispose () {}
	}
}
