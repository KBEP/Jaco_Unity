namespace Jaco.XSV
{
	public readonly struct Element
	{
		public readonly ELEMENT_TYPE type;
		public readonly string value;//not used if type is not VALUE

		public Element (ELEMENT_TYPE type, string value = null)
		{
			this.type = type;
			this.value = value;
		}

		public override string ToString () =>
		  type == ELEMENT_TYPE.VALUE
		  ? string.Format("(VALUE:{0})", value)
		  : string.Format("({0})", type);
	}
}
