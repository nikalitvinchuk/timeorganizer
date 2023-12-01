@* public class GregorianCalendar : System.Globalization.Calendar

using System;
using System.Globalization;

public class SamplesGregorianCalendar
{

	public static void Main()
	{

		// Creates strings with punctuation and without.
		String strADPunc = "A.D.";
		String strADNoPunc = "AD";
		String strCEPunc = "C.E.";
		String strCENoPunc = "CE";

		// Calls DTFI.GetEra for each culture that uses GregorianCalendar as the default calendar.
		Console.WriteLine("            ----- AD -----  ----- CE -----");
		Console.WriteLine("CULTURE     PUNC   NO PUNC  PUNC   NO PUNC  CALENDAR");
		foreach (CultureInfo myCI in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
		{
			Console.Write("{0,-12}", myCI);
			Console.Write("{0,-7}{1,-9}", myCI.DateTimeFormat.GetEra(strADPunc), myCI.DateTimeFormat.GetEra(strADNoPunc));
			Console.Write("{0,-7}{1,-9}", myCI.DateTimeFormat.GetEra(strCEPunc), myCI.DateTimeFormat.GetEra(strCENoPunc));
			Console.Write("{0}", myCI.Calendar);
			Console.WriteLine();
		}
	}
}
*@