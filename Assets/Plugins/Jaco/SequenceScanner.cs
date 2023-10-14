namespace Jaco
{
	//обеспечивает последовательную выборку из источника последовательностей (простого текста или спец. символов)
	public class SequenceScanner
	{
		static readonly char[] specialCharsStub = {};

		string source = string.Empty;
		char[] specialChars = specialCharsStub;
		char[] singleSpecialChar = new char[1] { default };
		int last = 0;
		int cur = 0;

		public string Source => source;

		int Length => last - cur + 1;

		public void Init (string source, char specialChar, int start = 0, int length = -1)
		{
			this.singleSpecialChar[0] = specialChar;
			Init(source, this.singleSpecialChar, start, length);
		}

		public void Init (string source, char[] specialChars, int start = 0, int length = -1)
		{
			this.source = source ?? string.Empty;
			this.specialChars = specialChars ?? specialCharsStub;
			this.cur = Clamp(start, 0, this.source.Length - 1);
			if (length <= -1) length = this.source.Length;
			this.last = Clamp(this.cur + length - 1, this.cur, this.cur + this.source.Length - 1);
		}

		public void Clear ()
		{
			source = string.Empty;
			specialChars = specialCharsStub;
			singleSpecialChar[0] = default;
			last = 0;
			cur = 0;
		}

		static int Clamp (int value, int min, int max)
		{
			if (min > max)
			{
				int tmp = min;
				min = max;
				max = tmp;
			}

			if (value < min) return min;
			else if (value > max) return max;
			else return value;
		}

		//если в источнике последовательностей больше нет - вернёт последовательность нулевой длины
		public Sequence GetNext ()
		{
			//достигнут конец источника, нет больше последовательностей для чтения
			if (cur > last) return new Sequence(last, 0);

			int s = source.IndexOfAny(specialChars, cur, Length);
			if (s != -1)//встречен спец. символ
			{
				int len = s - cur;
				if (len > 0)//есть текст до спец. символа - вернуть этот текст
				{
					Sequence result = new Sequence(cur, len);
					cur = s;//устанавливаем каретку на спец. символ
					return result;
				}
				else//нет текста - вернуть спец. символ
				{
					Sequence result = new Sequence(s, 1);
					cur = s + 1;//устанавливаем каретку на следующий за спец. символом символ
					return result;
				}
			}
			else//спец. символ не встречен
			{
				Sequence result = new Sequence(cur, Length);
				cur = last + 1;//устанавливаем за последний символ в источнике
				return result;
			}
		}
	}
}
