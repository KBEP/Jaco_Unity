using System;
using System.Collections;
using System.Collections.Generic;

namespace Jaco.XSV
{
	public struct ValueEnumerator : IEnumerator<string>
	{
		Reader reader;
		string current;

		public string Current => current;

		public ValueEnumerator (Reader reader)
		{
			this.reader = reader;
			this.current = default;
		}

		string ReadNextValue ()
		{
			for (;;)
			{
				Element e = reader.Peek();
				if (e.type == ELEMENT_TYPE.VALUE)
				{
					return reader.Read().value;
				}
				else if (e.type == ELEMENT_TYPE.REC_DEL || e.type == ELEMENT_TYPE.TABLE_END)
				{
					return null;//no more values
				}
			}
		}

		//

		object IEnumerator.Current => Current;

		void IEnumerator.Reset ()
		{
			throw new NotSupportedException("Reset()");
		}

		bool IEnumerator.MoveNext ()
		{
			current = ReadNextValue();
			return current != null;
		}

		void IDisposable.Dispose () {}
	}
}
