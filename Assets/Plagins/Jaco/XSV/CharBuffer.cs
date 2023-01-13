using System.Collections.Generic;
using System.Text;

namespace Jaco.XSV
{
	//helper class for generating values from read characters (todo:can be optimized)

	public class CharBuffer
	{
		StringBuilder builder = new StringBuilder();
		Queue<char> buffer = new Queue<char>();

		public bool IsEmpty => buffer.Count <= 0;

		public void Add (char c) => buffer.Enqueue(c);
		public void Clear () => buffer.Clear();

		public string GenString (string newLineReplacer)//null means do not replacing
		{
			string result = null;

			if (newLineReplacer == null) result = string.Concat(buffer);
			else
			{
				while (buffer.TryDequeue(out char c))
				{
					if (c == CharReader.NEL) builder.Append(newLineReplacer);//replace to newLineReplacer
					else builder.Append(c);//append as is
				}
				result = builder.ToString();
			}

			buffer.Clear();

			return result;
		}
	}
}
