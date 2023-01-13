using System;

namespace Jaco.XSV
{
	public class DataFormat
	{
		public static readonly DataFormat tsv
		  = new DataFormat(FORMAT.TSV, Const.TSV_VALUE_DELIMITER, default, default);

		public static readonly DataFormat csv
		  = new DataFormat(FORMAT.CSV, Const.CSV_VALUE_DELIMITER, Const.ESCAPE_CHAR, default);

		public readonly FORMAT format;//reading algorithm - tsv or csv
		public readonly char valueDelimiter;
		public readonly char recordDelimiter;
		public readonly char escapeChar;//value quoted char (for csv only)
		public readonly string newLineReplacer;//null means NEL (for csv only)

		public DataFormat (FORMAT format, char valueDelimiter, char escapeChar, string newLineReplacer)
		{
			if (valueDelimiter == escapeChar)
			  throw new Exception($"'{nameof(valueDelimiter)}' and '{nameof(escapeChar)}' can not be equal.");

			this.format = format;
			this.valueDelimiter = valueDelimiter;
			//constant in current implementation, see CharReader.ReadToQueue()
			this.recordDelimiter = Const.RECORD_DELIMITER;
			this.escapeChar = escapeChar;
			this.newLineReplacer = newLineReplacer;
		}
	}
}
