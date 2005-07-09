using System;
public class AppSupportClass {

    public class TextNumberFormat {
	// Declaration of fields
	private System.Globalization.NumberFormatInfo numberFormat;
	private enum formatTypes { General, Number, Currency, Percent };
	private int numberFormatType;
	private bool groupingActivated;
	private string separator;
	private int digits;

	// CONSTRUCTORS
	public TextNumberFormat() {
	    this.numberFormat      = new System.Globalization.NumberFormatInfo();
	    this.numberFormatType  = (int)TextNumberFormat.formatTypes.General;
	    this.groupingActivated = true;
	    this.separator = this.GetSeparator( (int)TextNumberFormat.formatTypes.General );
	    this.digits = 3;
	}

	private TextNumberFormat(TextNumberFormat.formatTypes theType, int digits) {
	    this.numberFormat      = System.Globalization.NumberFormatInfo.CurrentInfo;
	    this.numberFormatType  = (int)theType;
	    this.groupingActivated = true;
	    this.separator = this.GetSeparator( (int)theType );
	    this.digits    = digits;
	}

	private TextNumberFormat(TextNumberFormat.formatTypes theType, System.Globalization.CultureInfo cultureNumberFormat, int digits) {
	    this.numberFormat      = cultureNumberFormat.NumberFormat;
	    this.numberFormatType  = (int)theType;
	    this.groupingActivated = true;
	    this.separator = this.GetSeparator( (int)theType );
	    this.digits    = digits;
	}

	public static TextNumberFormat getTextNumberInstance() {
	    TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Number, 3);
	    return instance;
	}

	public static TextNumberFormat getTextNumberCurrencyInstance() {
	    TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Currency, 3);
	    return instance;
	}

	public static TextNumberFormat getTextNumberPercentInstance() {
	    TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Percent, 3);
	    return instance;
	}

	public static TextNumberFormat getTextNumberInstance(System.Globalization.CultureInfo culture) {
	    TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Number, culture, 3);
	    return instance;
	}

	public static TextNumberFormat getTextNumberCurrencyInstance(System.Globalization.CultureInfo culture) {
	    TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Currency, culture, 3);
	    return instance;
	}

	public static TextNumberFormat getTextNumberPercentInstance(System.Globalization.CultureInfo culture) {
	    TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Percent, culture, 3);
	    return instance;
	}

	public System.Object Clone() {
	    return (System.Object)this;
	}

	public override bool Equals(System.Object textNumberObject) {
	    return System.Object.Equals((System.Object)this, textNumberObject);
	}

	public string FormatDouble(double number) {
	    if (this.groupingActivated) {
		return number.ToString(this.GetCurrentFormatString() + this.digits , this.numberFormat);
	    } else {
		return (number.ToString(this.GetCurrentFormatString() + this.digits , this.numberFormat)).Replace(this.separator,"");
	    }
	}

	public string FormatLong(long number) {
	    if (this.groupingActivated) {
		return number.ToString(this.GetCurrentFormatString() + this.digits , this.numberFormat);
	    } else {
		return (number.ToString(this.GetCurrentFormatString() + this.digits , this.numberFormat)).Replace(this.separator,"");
	    }
	}

	public static System.Globalization.CultureInfo[] GetAvailableCultures() {
	    return System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures);
	}

	public override int GetHashCode() {
	    return this.GetHashCode();
	}

	private string GetCurrentFormatString() {
	    string currentFormatString = "n";  //Default value
	    switch (this.numberFormatType) {
		case (int)TextNumberFormat.formatTypes.Currency:
		    currentFormatString = "c";
		    break;

		case (int)TextNumberFormat.formatTypes.General:
		    currentFormatString = "n" + this.numberFormat.NumberDecimalDigits;
		    break;

		case (int)TextNumberFormat.formatTypes.Number:
		    currentFormatString = "n" + this.numberFormat.NumberDecimalDigits;
		    break;

		case (int)TextNumberFormat.formatTypes.Percent:
		    currentFormatString = "p";
		    break;
	    }
	    return currentFormatString;
	}

	private string GetSeparator(int numberFormatType) {
	    string separatorItem = " ";  //Default Separator

	    switch (numberFormatType) {
		case (int)TextNumberFormat.formatTypes.Currency:
		    separatorItem = this.numberFormat.CurrencyGroupSeparator;
		    break;

		case (int)TextNumberFormat.formatTypes.General:
		    separatorItem = this.numberFormat.NumberGroupSeparator;
		    break;

		case (int)TextNumberFormat.formatTypes.Number:
		    separatorItem = this.numberFormat.NumberGroupSeparator;
		    break;

		case (int)TextNumberFormat.formatTypes.Percent:
		    separatorItem = this.numberFormat.PercentGroupSeparator;
		    break;
	    }
	    return separatorItem;
	}

	public bool GroupingUsed {
	    get {
		return (this.groupingActivated);
	    }
	    set {
		this.groupingActivated = value;
	    }
	}

	public int Digits {
	    get {
		return this.digits;
	    }
	    set {
		this.digits = value;
	    }
	}
    }

    /*******************************/
    public class DateTimeFormatManager {
	static public DateTimeFormatHashTable manager = new DateTimeFormatHashTable();

    public class DateTimeFormatHashTable :System.Collections.Hashtable {
	    public void SetDateFormatPattern(System.Globalization.DateTimeFormatInfo format, System.String newPattern) {
		if (this[format] != null)
		    ((DateTimeFormatProperties) this[format]).DateFormatPattern = newPattern;
		else {
		    DateTimeFormatProperties tempProps = new DateTimeFormatProperties();
		    tempProps.DateFormatPattern  = newPattern;
		    Add(format, tempProps);
		}
	    }

	    public string GetDateFormatPattern(System.Globalization.DateTimeFormatInfo format) {
		if (this[format] == null)
		    return "d-MMM-yy";
		else
		    return ((DateTimeFormatProperties) this[format]).DateFormatPattern;
	    }

	    public void SetTimeFormatPattern(System.Globalization.DateTimeFormatInfo format, System.String newPattern) {
		if (this[format] != null)
		    ((DateTimeFormatProperties) this[format]).TimeFormatPattern = newPattern;
		else {
		    DateTimeFormatProperties tempProps = new DateTimeFormatProperties();
		    tempProps.TimeFormatPattern  = newPattern;
		    Add(format, tempProps);
		}
	    }

	    public string GetTimeFormatPattern(System.Globalization.DateTimeFormatInfo format) {
		if (this[format] == null)
		    return "h:mm:ss tt";
		else
		    return ((DateTimeFormatProperties) this[format]).TimeFormatPattern;
	    }

	    class DateTimeFormatProperties {
		public string DateFormatPattern = "d-MMM-yy";
		public string TimeFormatPattern = "h:mm:ss tt";
	    }
	}
    }

    /*******************************/
    public static string FormatDateTime(System.Globalization.DateTimeFormatInfo format, System.DateTime date) {
	string timePattern = DateTimeFormatManager.manager.GetTimeFormatPattern(format);
	string datePattern = DateTimeFormatManager.manager.GetDateFormatPattern(format);
	return date.ToString(datePattern + " " + timePattern, format);
    }

    /*******************************/
    public static System.Globalization.DateTimeFormatInfo GetDateTimeFormatInstance(int dateStyle, int timeStyle, System.Globalization.CultureInfo culture) {
	System.Globalization.DateTimeFormatInfo format = culture.DateTimeFormat;

	switch (timeStyle) {
	    case -1:
		DateTimeFormatManager.manager.SetTimeFormatPattern(format, "");
		break;

	    case 0:
		DateTimeFormatManager.manager.SetTimeFormatPattern(format, "h:mm:ss 'o clock' tt zzz");
		break;

	    case 1:
		DateTimeFormatManager.manager.SetTimeFormatPattern(format, "h:mm:ss tt zzz");
		break;

	    case 2:
		DateTimeFormatManager.manager.SetTimeFormatPattern(format, "h:mm:ss tt");
		break;

	    case 3:
		DateTimeFormatManager.manager.SetTimeFormatPattern(format, "h:mm tt");
		break;
	}

	switch (dateStyle) {
	    case -1:
		DateTimeFormatManager.manager.SetDateFormatPattern(format, "");
		break;

	    case 0:
		DateTimeFormatManager.manager.SetDateFormatPattern(format, "dddd, MMMM dd%, yyy");
		break;

	    case 1:
		DateTimeFormatManager.manager.SetDateFormatPattern(format, "MMMM dd%, yyy" );
		break;

	    case 2:
		DateTimeFormatManager.manager.SetDateFormatPattern(format, "d-MMM-yy" );
		break;

	    case 3:
		DateTimeFormatManager.manager.SetDateFormatPattern(format, "M/dd/yy");
		break;
	}

	return format;
    }

}
