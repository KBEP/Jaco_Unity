using System;
using System.Collections.Generic;
using System.Text;

namespace Jaco
{
	public class Interpreter
	{
		TerminalProvider terminalProvider = new TerminalProvider();
		StringBuilder strBuilder = new StringBuilder();
		Stack<Terminal> stack = new Stack<Terminal>();
		List<string> argList = new List<string>();
		IVarProvider varProvider;

		public INTERPRETATION_RESULT TryInterpret (string source, IVarProvider varProvider, out string result)
		{
			this.varProvider = varProvider ?? DummyVarProvider.Instance;
			terminalProvider.Init(source, TerminalUtility.special);

			result = string.Empty;

			for (;;)
			{
				Terminal t = terminalProvider.GetNext();
				if (t.type == TERMINAL_TYPE.EMPTY)
				{
					while (stack.Count > 0)
					{
						t = stack.Pop();
						//на стэке д.б. остаться только терминалы текста или переменные, иначе ошибка синтаксиса
						if (t.type == TERMINAL_TYPE.TEXT)
						{
							ReadOnlySpan<char> text = t.source.AsSpan(t.sequence.start, t.sequence.length);
							strBuilder.Insert(0, text);
						}
						else if (t.type == TERMINAL_TYPE.VAR)
						{
							string varValue = Evaluate(t.GenString(), null);
							if (!string.IsNullOrEmpty(varValue)) strBuilder.Insert(0, varValue);
						}
						else
						{
							Clear();
							return INTERPRETATION_RESULT.ERR_EXPECT_TEXT_OR_VAR;
						}
					}
					result = strBuilder.ToString();
					Clear();
					return INTERPRETATION_RESULT.SUCCESS;
				}
				else if (t.type == TERMINAL_TYPE.CLOSE_BRACE)
				{
					for (;;)
					{
						if (!stack.TryPop(out Terminal tt))//пытаемся получить терминал
						{
							Clear();
							return INTERPRETATION_RESULT.ERR_NO_TERMINAL_AHEAD_CB;
						}
						if (tt.type == TERMINAL_TYPE.EMPTY)
						{
							Clear();
							return INTERPRETATION_RESULT.ERR_UNEXPECTED_EMPTY;
						}
						if (tt.type == TERMINAL_TYPE.OPEN_BRACE)
						{
							if (!stack.TryPop(out Terminal tVar))//пытаемся получить терминал
							{
								Clear();
								return INTERPRETATION_RESULT.ERR_NO_TERMINAL_AHEAD_OB;
							}
							argList.Insert(0, tVar.GenString());
							string s = Evaluate(argList);
							argList.Clear();//---???
							stack.Push(new Terminal(TERMINAL_TYPE.TEXT, s));
							break;
						}
						else if (tt.type == TERMINAL_TYPE.TEXT)
						{
							argList.Insert(0, tt.GenString());
						}
						else if (tt.type == TERMINAL_TYPE.VAR)
						{
							string v = Evaluate(tt.GenString(), null);//без аргументов
							argList.Insert(0, v);
						}
					}
				}
				else
				{
					stack.Push(t);
				}
			}
		}

		public string Interpret (string source, IVarProvider varProvider, string errResult = "") =>
		  TryInterpret(source, varProvider, out string result) == INTERPRETATION_RESULT.SUCCESS ? result : errResult;
		
		void Clear ()
		{
			stack.Clear();
			argList.Clear();
			strBuilder.Clear();
		}

		string Evaluate (List<string> args)
		{
			if (args == null || args.Count <= 0) return null;
			string varName = args[0];
			args.RemoveAt(0);
			return Evaluate (varName, args);
		}

		string Evaluate (string varName, List<string> args) =>
		  varProvider.TryGetValue(varName, out string result, args) ? result : null;

		class DummyVarProvider : IVarProvider
		{
			public static DummyVarProvider Instance => new DummyVarProvider();
			
			public bool Contains (string varName) => false;

			public bool TryGetValue (string varName, out string varValue, IReadOnlyList<string> args = null)
			{
				varValue = default;
				return false;
			}
		}
	}
}
