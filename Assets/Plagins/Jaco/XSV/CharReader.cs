using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jaco.XSV
{
	public class CharReader : IDisposable
	{
		public const char NEL = Const.NEL;//const in current implementation
		
		TextReader reader;
		Queue<int> chars;
		bool isDisposed;
		
		CharReader (TextReader reader)
		{
			this.reader = reader;
			this.chars = new Queue<int>();
		}
		
		public static CharReader FromStream (Stream stream)
		{
			return new CharReader(new StreamReader(stream));
		}
		
		public static CharReader FromFile (string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
		{
			StreamReader sr = new StreamReader(path, encoding, detectEncodingFromByteOrderMarks);
			return new CharReader(sr);
		}

		public static CharReader FromFile (string path)
		{
			return FromFile(path, Encoding.UTF8, true);
		}

		public static CharReader FromString (string source)
		{
			source = source ?? string.Empty;
			StringReader sr = new StringReader(source);
			return new CharReader(sr);
		}

		// These sequences are translated to a NEL:
		// CR+LF
		// CR
		// LF
		// LS
		// PS

		void ReadToQueue ()
		{
			int c = reader.Read();
			if (c == Const.CR)
			{
				if (reader.Peek() == Const.LF) reader.Read();//skip LF
				chars.Enqueue(NEL);
			}
			else if (c == Const.LF || c == Const.LS || c == Const.PS) chars.Enqueue(NEL);
			else chars.Enqueue(c);//c is not new line char sequence
		}
		
		public int Read ()
		{
			if (chars.TryDequeue(out int result)) return result;
			ReadToQueue();
			if (chars.TryDequeue(out result)) return result;
			//never if ReadToQueue() implemented correctly
			else throw new Exception("Unexpected empty queue 'chars' after 'ReadToQueue()' call.");
		}

		public int Peek ()
		{
			if (chars.TryPeek(out int result)) return result;
			ReadToQueue();
			if (chars.TryPeek(out result)) return result;
			//never if ReadToQueue() implemented correctly
			else throw new Exception("Unexpected empty queue 'chars' after 'ReadToQueue()' call.");
		}

		//

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
			chars.Clear();//?
			chars = null;
			isDisposed = true;
		}

		~CharReader ()
		{
			Dispose(false);
		}
	}
}
