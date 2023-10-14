using System.Collections;
using System.Collections.Generic;

namespace Jaco.XSV
{
	public readonly struct Record : IEnumerable<string>
	{
		public static readonly Record Invalid = new Record(null);

		readonly Reader reader;

		public bool IsValid => reader != null;

		public bool HasValues => reader.Peek().type == ELEMENT_TYPE.VALUE;

		public Record (Reader reader)
		{
			this.reader = reader;
		}

		public IEnumerator<string> GetEnumerator ()
		{
			return new ValueEnumerator(reader);
		}

		//

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}
	}
}
