using UnityEngine;
using Jaco;

public sealed class Tester : MonoBehaviour
{
	[Header("***PRESS T TO TEST***")]
	
	//обычно вместо 5 и 4 вставляется динамическая переменная
	//переменные день и час заданы в файле counting_form_rus.tsv
	//в фигурных скобках передаются модификаторы:
	//числа автоматически переведутся в модификаторы числа,
	//дат и имн - это модификаторы падежа
	//таким образом переводчик может указать как правильно должно меняться слово
	public string source = "К 5 ~день~{5|дат} добавить 4 ~час~{4|имн}.";

	//здесь финальный текст, каким он будет в диалоге персонажа или на кнопке
	public string result;

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			//путь до файла с данными словоформ
			string path = Application.dataPath + "/Resources/counting_form_rus.tsv";
			//из этого провайдера берём слова в нужной форме
			WordFormProvider wordFormProvider = new WordFormProvider();
			//добавляем в провайдер данные из файла
			WordFormUtility.AddWordForms(path, ref wordFormProvider, true);

			result = new Interpreter().Interpret(source, wordFormProvider, source);
		}
	}
}
