//Реализация по Варианту №4: Text text ~var_name~{~arg1~|any text here|~arg2~} text text text.
namespace Jaco
{
	public class TerminalProvider
	{
		SequenceScanner scanner;

		public TerminalProvider ()
		{
			scanner = new SequenceScanner();
		}

		public TERM_GEN_RESULT TryGetNext (out Terminal terminal)
		{
			Sequence s = scanner.GetNext();
			if (s.length <= 0)
			{
				terminal = Terminal.Empty;
				return TERM_GEN_RESULT.SUCCESS;
			}
			if (TerminalUtility.IsVarMark(scanner.Source, s))//возможно имя переменной
			{
				int varStart = s.start + 1;//+ 1 for skip head var mark
				s = scanner.GetNext();//var name
				if (!TerminalUtility.IsValidVarName_NoMark(scanner.Source, s))//error
				{
					terminal = Terminal.Empty;
					return TERM_GEN_RESULT.ERR_EXPECT_VAR_NAME;
				}
				int varLength = s.length;
				s = scanner.GetNext();//tail var mark
				if (!TerminalUtility.IsVarMark(scanner.Source, s))//error
				{
					terminal = Terminal.Empty;
					return TERM_GEN_RESULT.ERR_EXPECT_CLOSING_VAR_MARK;
				}
				terminal = new Terminal(TERMINAL_TYPE.VAR, new Sequence(varStart, varLength), scanner.Source);
				return TERM_GEN_RESULT.SUCCESS;
			}
			if (!TerminalUtility.TryGetSequenceType(scanner.Source, s, out SEQUENCE_TYPE sType))//error
			{
				terminal = Terminal.Empty;
				return TERM_GEN_RESULT.ERR_UNDETERMINED_SEQ_TYPE;
			}
			if (!TryGetTerminalType(sType, out TERMINAL_TYPE tType))//error
			{
				terminal = Terminal.Empty;
				return TERM_GEN_RESULT.ERR_SEQ_TYPE_TRANSLATION_FAIL;
			}
			terminal = new Terminal(tType, s, scanner.Source);
			return TERM_GEN_RESULT.SUCCESS;
		}

		public Terminal GetNext ()
		{
			TryGetNext(out Terminal t);
			return t;
		}

		public void Init (string source, char[] specialChars)
		{
			scanner.Init(source, specialChars);
		}

		bool TryGetTerminalType (SEQUENCE_TYPE type, out TERMINAL_TYPE result)
		{
			switch (type)
			{
				case SEQUENCE_TYPE.TEXT        : result = TERMINAL_TYPE.TEXT;        return true;
				case SEQUENCE_TYPE.OPEN_BRACE  : result = TERMINAL_TYPE.OPEN_BRACE;  return true;
				case SEQUENCE_TYPE.ARG_SEP     : result = TERMINAL_TYPE.ARG_SEP;     return true;
				case SEQUENCE_TYPE.CLOSE_BRACE : result = TERMINAL_TYPE.CLOSE_BRACE; return true;
				default                        : result = default(TERMINAL_TYPE);    return false;
			}
		}
	}
}
