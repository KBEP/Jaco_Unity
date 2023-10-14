namespace Jaco.WordGeneration
{
	class OpProvider
	{
		SequenceScanner scanner = new SequenceScanner();

		public void Init (string source, char[] specialChars)
		{
			this.scanner.Init(source, specialChars);
		}

		public OP_GEN_RESULT TryGetNext (out Op op)
		{
			Terminal t = GetNextTerminal();
			if (t.type == TERMINAL_TYPE.EMPTY)
			{
				op = new Op(OP_TYPE.RETURN, t.sequence);
				return OP_GEN_RESULT.SUCCESS;
			}
			else if (t.type == TERMINAL_TYPE.TEXT)
			{
				Terminal opr = GetNextTerminal();
				if (opr.type == TERMINAL_TYPE.EMPTY)
				{
					op = new Op(OP_TYPE.RETURN, t.sequence);
					return OP_GEN_RESULT.SUCCESS;
				}
				else if (opr.type == TERMINAL_TYPE.OP_ADD)
				{
					op = new Op(OP_TYPE.ADD_HEAD, t.sequence);
					return OP_GEN_RESULT.SUCCESS;
				}
				else if (opr.type == TERMINAL_TYPE.OP_REPLACE)
				{
					op = new Op(OP_TYPE.REPLACE_HEAD, t.sequence);
					return OP_GEN_RESULT.SUCCESS;
				}
				else if (opr.type == TERMINAL_TYPE.TEXT)
				{
					//Debug.Log("Ошибка сканнера: Прочитан текст после текста.");
					op = new Op(OP_TYPE.RETURN, Sequence.Empty);
					return OP_GEN_RESULT.ERR_TXT_TXT;
				}
				else
				{
					//Debug.Log("Ошибка: Неизвестный тип терминала.");
					op = new Op(OP_TYPE.RETURN, Sequence.Empty);
					return OP_GEN_RESULT.ERR_UNK_TERM;
				}
			}
			else//terminal is op
			{
				Terminal txt = GetNextTerminal();
				if (txt.type == TERMINAL_TYPE.TEXT)
				{
					if (t.type == TERMINAL_TYPE.OP_ADD)
					{
						op = new Op(OP_TYPE.ADD_TAIL, txt.sequence);
						return OP_GEN_RESULT.SUCCESS;
					}
					else if (t.type == TERMINAL_TYPE.OP_REPLACE)
					{
						op = new Op(OP_TYPE.REPLACE_TAIL, txt.sequence);
						return OP_GEN_RESULT.SUCCESS;
					}
					else
					{
						//Debug.Log("Ошибка: Неизвестный тип терминала.");
						op = new Op(OP_TYPE.RETURN, Sequence.Empty);
						return OP_GEN_RESULT.ERR_UNK_TERM;
					}
				}
				else
				{
					//Debug.Log("Ошибка: Прочитан оператор после оператора.");
					op = new Op(OP_TYPE.RETURN, Sequence.Empty);
					return OP_GEN_RESULT.ERR_OP_OP;
				}
			}
		}

		Terminal GetNextTerminal ()
		{
			Sequence s = scanner.GetNext();
			if (s.length <= 0) return new Terminal(TERMINAL_TYPE.EMPTY, s);
			if (s.length == 1)
			{
				if (scanner.Source[s.start] == WordGenerator.OP_ADD)
				  return new Terminal(TERMINAL_TYPE.OP_ADD, s);
				  
				if (scanner.Source[s.start] == WordGenerator.OP_REPLACE)
				  return new Terminal(TERMINAL_TYPE.OP_REPLACE, s);
			}
			return new Terminal(TERMINAL_TYPE.TEXT, s);
		}
	}
}