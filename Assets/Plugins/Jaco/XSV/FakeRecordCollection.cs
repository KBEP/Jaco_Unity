using System.Collections;
using System.Collections.Generic;

namespace Jaco.XSV
{
	public struct FakeRecordCollection : IEnumerable<Record>
	{
		RecordEnumerator enumerator;

		public FakeRecordCollection (ref RecordEnumerator enumerator)
		{
			this.enumerator = enumerator;
		}

		public IEnumerator<Record> GetEnumerator ()
		{
			return enumerator;
		}

		//

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}
	}
}