using System;
using System.Collections.Generic;

namespace Jaco
{
	public class WordForm
	{		
		public readonly WordFormHeader header;
		//used for translation from a number to a grammatical number from modifier, the field can be null
		public readonly Func<int, string> numToMod;

		readonly MultiArray<string, string> forms;

		public string BaseForm => forms.Size >= 1 ? forms.GetFirst() : string.Empty;

		public WordForm (WordFormHeader header, MultiArray<string, string> forms)
		{
			if (header == null) throw new ArgumentNullException(nameof(header));
			if (forms == null) throw new ArgumentNullException(nameof(forms));
			if (forms.Size <= 0) throw new ArgumentException($"Empty '{nameof(forms)}'.");

			this.header = header;
			this.forms = forms;

			if (this.header.values.TryGetValue("num_to_mod", out string methodName))
			{
				this.numToMod = ModHelper.GetNumToModMethod(methodName);
				if (this.numToMod == null)
				  throw new ArgumentException(string.Format("Method '{0}' does not exist.", methodName));
			}
			else this.numToMod = null;
		}

		public string Get (IReadOnlyList<string> mods) => forms.Get(mods);

		public override string ToString ()
		  => string.Format("(base:{0}, dims:{1}, keys:{2})", BaseForm, forms.Rank, forms.Size);
	}
}
