using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jaco.XSV
{	
	public class Reader : IDisposable
	{
		public const int NO_MORE_CHARS = -1;//TextReader.Read() returns this value if no more characters are available

		CharReader reader;
		DataFormat format;
		CharBuffer buffer;//for value generating
		Queue<Element> elements;//for read values storing
		Action ReadToQueue;
		bool isDisposed;

		Reader (CharReader reader, DataFormat format)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));
			if (format == null) throw new ArgumentNullException(nameof(format));			

			if (format.format == FORMAT.TSV) ReadToQueue = ReadToQueue_TSV;
			else if (format.format == FORMAT.CSV) ReadToQueue = ReadToQueue_CSV;
			else throw new Exception("Undefined data format.");

			this.reader = reader;
			this.format = format;
			this.buffer = new CharBuffer();
			this.elements = new Queue<Element>();
			this.isDisposed = false;
		}

		public static Reader FromStream (Stream stream, DataFormat format)
		{
			CharReader cr = CharReader.FromStream(stream);
			return new Reader(cr, format);
		}

		public static Reader FromFile (string path, DataFormat format,
		  Encoding encoding, bool detectEncodingFromByteOrderMarks)
		{
			CharReader cr = CharReader.FromFile(path, encoding, detectEncodingFromByteOrderMarks);
			return new Reader(cr, format);
		}

		public static Reader FromFile (string path, DataFormat format)
		{
			return FromFile(path, format, Encoding.UTF8, true);
		}

		public static Reader FromString (string source, DataFormat format)
		{
			CharReader cr = CharReader.FromString(source);
			return new Reader(cr, format);
		}

		void ReadToQueue_TSV ()
		{
			for (;;)
			{
				int c = reader.Read();
				ELEMENT_TYPE t = GetType(c);
				if (t == ELEMENT_TYPE.VALUE) buffer.Add((char)c);
				else
				{
					elements.Enqueue(new Element(ELEMENT_TYPE.VALUE, buffer.GenString(null)));
					if (t != ELEMENT_TYPE.VALUE_DEL) elements.Enqueue(new Element(t));//do not add value delimiter
					return;
				}
			}
		}

		void ReadToQueue_CSV ()
		{
			bool isQuoted = false;//is read sequence quoted

			for (;;)
			{
				int c = reader.Read();

				if (isQuoted)//quoted read sequence
				{
					//c is the last character of a quoted sequence or the first character of a double quote
					if (c == format.escapeChar)
					{
						c = reader.Read();
						//the previous character was the last character of a quoted sequence
						if (c == format.valueDelimiter || c == format.recordDelimiter || c == NO_MORE_CHARS)
						{
							ELEMENT_TYPE t = GetType(c);
							elements.Enqueue(new Element(ELEMENT_TYPE.VALUE, buffer.GenString(Environment.NewLine)));
							if (t != ELEMENT_TYPE.VALUE_DEL)
							{
								//elements.Enqueue(new Element(t));//works with Google Docs csv-format only

								//crutch: works with both Google Docs & LibreOffice csv-formats
								//all records in LibreOffice csv-file end with record delimeter
								if (t == ELEMENT_TYPE.REC_DEL && reader.Peek() == NO_MORE_CHARS)
								  elements.Enqueue(new Element(ELEMENT_TYPE.TABLE_END));
								else elements.Enqueue(new Element(t));
							}
							isQuoted = false;
							return;
						}
						//the previous character was the first character of a double quote
						else if (c == format.escapeChar) buffer.Add(format.escapeChar);
					}
					else if (c == NO_MORE_CHARS)//data are invalid
					{
						buffer.Clear();
						throw new Exception("Unexpected end of data inside a quoted sequence.");
					}
					else buffer.Add((char)c);//the character is not special one
				}
				else
				{
					if (c == format.escapeChar) isQuoted = true;//c is the first character of a quoted sequence
					else
					{
						ELEMENT_TYPE t = GetType(c);
						if (t == ELEMENT_TYPE.VALUE) buffer.Add((char)c);//the character is not special one
						else
						{
							elements.Enqueue(new Element(ELEMENT_TYPE.VALUE, buffer.GenString(Environment.NewLine)));
							if (t != ELEMENT_TYPE.VALUE_DEL)
							{
								//elements.Enqueue(new Element(t));//works with Google Docs csv-format only

								//crutch: works with both Google Docs & LibreOffice csv-formats
								//all records in LibreOffice csv-file end with record delimeter
								if (t == ELEMENT_TYPE.REC_DEL && reader.Peek() == NO_MORE_CHARS)
								  elements.Enqueue(new Element(ELEMENT_TYPE.TABLE_END));
								else elements.Enqueue(new Element(t));
							}
							return;
						}
					}
				}
			}
		}

		public Element Read ()
		{
			if (elements.TryDequeue(out Element result)) return result;
			ReadToQueue();
			if (elements.TryDequeue(out result)) return result;
			//never if ReadToQueue() implemented correctly
			else
			  throw new Exception($"Unexpected empty queue '{nameof(elements)}' after '{nameof(ReadToQueue)}()' call.");
		}

		public Element Peek ()
		{
			if (elements.TryPeek(out Element result)) return result;
			ReadToQueue();
			if (elements.TryPeek(out result)) return result;
			//never if ReadToQueue() implemented correctly
			else
			  throw new Exception($"Unexpected empty queue '{nameof(elements)}' after '{nameof(ReadToQueue)}()' call.");
		}

		protected ELEMENT_TYPE GetType (int c)
		{
			if (c == format.valueDelimiter) return ELEMENT_TYPE.VALUE_DEL;
			else if (c == format.recordDelimiter) return ELEMENT_TYPE.REC_DEL;
			else if (c == NO_MORE_CHARS) return ELEMENT_TYPE.TABLE_END;
			else return ELEMENT_TYPE.VALUE;//any other is valid value character
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
			buffer?.Clear();//?
			elements?.Clear();//?
			reader = null;
			format = null;
			buffer = null;
			elements = null;
			isDisposed = true;
		}

		~Reader ()
		{
			Dispose(false);
		}
	}
}
