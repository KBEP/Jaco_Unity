namespace Jaco.WordGeneration
{
	enum OP_GEN_RESULT
	{
		NONE,
		ERR_TXT_TXT,//Ошибка сканнера: Прочитан текст после текста.
		ERR_UNK_TERM,//Ошибка: Неизвестный тип терминала.
		ERR_OP_OP,//Ошибка: Прочитан оператор после оператора.
		SUCCESS
	}
}
