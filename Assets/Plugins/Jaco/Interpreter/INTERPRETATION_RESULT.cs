namespace Jaco
{
	public enum INTERPRETATION_RESULT
	{
		NONE,
		ERR_EXPECT_TEXT_OR_VAR,//интерпретатор ожидал на стэке терминал типа TEXT или VAR
		ERR_NO_TERMINAL_AHEAD_CB,//на стэке нет терминала перед закрывающей скобкой
		ERR_NO_TERMINAL_AHEAD_OB,//на стэке нет терминала перед открывающей скобкой
		ERR_UNEXPECTED_EMPTY,//неожиданный пустой терминал (он может встречатся только в конце выражения)
		SUCCESS
	}
}
