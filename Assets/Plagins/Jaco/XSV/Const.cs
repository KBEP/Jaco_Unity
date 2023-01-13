namespace Jaco.XSV
{
	public static class Const
	{
		public const char NEL = '\u0085';//used as default record value
		public const char LF = '\u000A';
		public const char CR = '\u000D';
		public const char LS = '\u2028';
		public const char PS = '\u2029';

		public const char TSV_VALUE_DELIMITER = '\t';///used as default value
		public const char CSV_VALUE_DELIMITER = ',';///used as default value
		public const char RECORD_DELIMITER = NEL;
		public const char ESCAPE_CHAR = '\"';
	}
}