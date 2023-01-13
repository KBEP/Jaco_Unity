using System.Text;

namespace Jaco.WordGeneration
{
	public static class WordGenerator
	{
		public const char OP_ADD = '+';
		public const char OP_REPLACE = '~';

		const int MAX_OP = 3;//max opertors (for combination Head-Tail-End)

		static readonly char[] opChars = { OP_ADD, OP_REPLACE };

		static OpProvider opProvider = new OpProvider();
		static StringBuilder sb = new StringBuilder();

		public static GEN_RESULT GenForm (string baseForm, string patternOrValue, out string result)
		{
			result = string.Empty;
			if (string.IsNullOrEmpty(patternOrValue)) return GEN_RESULT.INVALID_PATTERN_OR_VALUE;
			opProvider.Init(patternOrValue, opChars);
			EXPECT exp = EXPECT.ANY;
			bool addBase = true;
			if (baseForm == null) baseForm = string.Empty;
			int baseStart = 0;
			int baseEnd = baseForm.Length - 1;
			int cmdCount = 0;
			while (++cmdCount <= MAX_OP)//limit
			{
				if (opProvider.TryGetNext(out Op op) != OP_GEN_RESULT.SUCCESS)
				{
					sb.Clear();
					return GEN_RESULT.SYNTAX_ERROR;
				}
				if (!IsExpected(op, exp))
				{
					sb.Clear();
					return GEN_RESULT.UNEXPECTED_OPERATOR;
				}
				if (op.type == OP_TYPE.RETURN)
				{
					if (addBase) sb.AppendFromTo(baseForm, baseStart, baseEnd);
					result = sb.ToStringAndClear();
					return GEN_RESULT.SUCCESS;
				}
				else if (op.type == OP_TYPE.ADD_HEAD)
				{
					sb.Append(patternOrValue, op.value.start, op.value.length);
				}
				else if (op.type == OP_TYPE.REPLACE_HEAD)
				{
					baseStart = baseForm.IndexOf(patternOrValue[op.value.End]);
					if (baseStart == -1)
					{
						sb.Clear();
						return GEN_RESULT.NO_HEAD_FOUND;
					}
					sb.Append(patternOrValue, op.value.start, op.value.length - 1);
				}
				else if (op.type == OP_TYPE.ADD_TAIL)
				{
					sb.AppendFromTo(baseForm, baseStart, baseEnd);
					addBase = false;
					sb.Append(patternOrValue, op.value.start, op.value.length);
				}
				else if (op.type == OP_TYPE.REPLACE_TAIL)
				{
					baseEnd = baseForm.LastIndexOf(patternOrValue[op.value.start]);//?
					if (baseEnd == -1)
					{
						sb.Clear();
						return GEN_RESULT.NO_TAIL_FOUND;
					}
					if (baseStart > baseEnd)
					{
						sb.Clear();
						return GEN_RESULT.IDX_OVERLAP;
					}
					sb.AppendFromTo(baseForm, baseStart, baseEnd);
					addBase = false;
					sb.Append(patternOrValue, op.value.start + 1, op.value.length - 1);
				}
				else
				{
					sb.Clear();
					return GEN_RESULT.UNKNOWN_OPERATOR;
				}
				exp = ExpectAfter(op);
			}
			sb.Clear();
			return GEN_RESULT.OPERATOR_OVERLIMIT;
		}

		public static string GenForm (string baseForm, string patternOrValue, string errResult = "")
		  => GenForm(baseForm, patternOrValue, out string result) == GEN_RESULT.SUCCESS ? result : errResult;

		static bool IsExpected (in Op op, EXPECT exp)
		{
			switch (exp)
			{
				case EXPECT.ANY:
					return true;
				
				case EXPECT.END_OR_TAIL:
					return op.type == OP_TYPE.RETURN || op.type == OP_TYPE.REPLACE_TAIL || op.type == OP_TYPE.ADD_TAIL;
				
				case EXPECT.END:
					return op.type == OP_TYPE.RETURN;
				
				case EXPECT.NOTHING:
				default:
					return false;
			}
		}

		static EXPECT ExpectAfter (in Op op)
		{
			switch (op.type)
			{
				case OP_TYPE.RETURN       : return EXPECT.NOTHING;
				case OP_TYPE.REPLACE_TAIL :
				case OP_TYPE.ADD_TAIL     : return EXPECT.END;
				case OP_TYPE.REPLACE_HEAD :
				case OP_TYPE.ADD_HEAD     : return EXPECT.END_OR_TAIL;
				default                   : return EXPECT.NOTHING;
			}
		}

		static string ToStringAndClear (this StringBuilder sb)
		{
			string result = sb.ToString();
			sb.Clear();
			return result;
		}

		static StringBuilder AppendFromTo (this StringBuilder sb, string value, int startIndex, int endIndex)
		{
			return sb.Append(value, startIndex, endIndex - startIndex + 1);
		}
	}
}
