namespace Jaco
{
	public static class TerminalUtility
	{
		public const char VAR_MARK = '~';

		public static readonly char[] special = { '{', '|', '}', VAR_MARK };
		public static readonly SEQUENCE_TYPE[] type =
		{
			SEQUENCE_TYPE.OPEN_BRACE,
			SEQUENCE_TYPE.ARG_SEP,
			SEQUENCE_TYPE.CLOSE_BRACE,
			SEQUENCE_TYPE.VAR_MARK
		};

		//без учёта маркера VAR_MARK
		public static bool IsValidVarName_NoMark (string source, in Sequence s)
		{
			if (source == null) return false;
			for (int i = 0; i < special.Length; ++i) if (source.IndexOf(special[i], s.start, s.length) != -1) return false;
			return true;
		}

		//не нуль, не нулевой длины, не содержит спец. символов
		public static bool IsValidVarName_NoMark (string name)
		{
			if (string.IsNullOrEmpty(name)) return false;
			for (int i = 0; i < special.Length; ++i) if (name.Contains(special[i])) return false;
			return true;
		}

		//имя должно начинаться и заканчиваться c VAR_MARK, не содержать специальных символов и содержать хотя бы один иной символ
		//добавить невозможность содержания пробельных символов-------------------------------------------------------------!!!
		public static bool IsValidVarName (string name)
		{
			if (name == null || !name.StartsWith(VAR_MARK) || !name.EndsWith(VAR_MARK)) return false;
			int count = special.Length - 1;//to skip VAR_MARK
			for (int i = 0; i < count; ++i) if (name.Contains(special[i])) return false;
			return true;
		}

		public static bool IsVarMark (string source, in Sequence s) =>
		  source != null && s.length == 1 && s.start < source.Length && source[s.start] == VAR_MARK;

		public static bool TryGetSequenceType (string source, in Sequence s, out SEQUENCE_TYPE result)
		{
			if (source == null || s.start >= source.Length)
			{
				result = default(SEQUENCE_TYPE);
				return false;
			}

			int idx = System.Array.IndexOf<char>(special, source[s.start]);

			if (idx != -1)//содержит спец. символ
			{
				if (s.length == 1)//последовательность - один из спец. символов
				{
					result = type[idx];
					return true;
				}
				else//последовательность со спец. символом не может содержать др. символы
				{
					result = default(SEQUENCE_TYPE);
					return false;
				}
			}

			result = SEQUENCE_TYPE.TEXT;
			return true;
		}
	}
}
