using System;
public class SupportClass {
    public static void WriteStackTrace(System.Exception throwable, System.IO.TextWriter stream) {
	stream.Write(throwable.StackTrace);
	stream.Flush();
    }

    /*******************************/
    public class CalendarManager {
	public const int YEAR = 0;
	public const int MONTH = 1;
	public const int DATE = 2;
	public const int HOUR = 3;
	public const int MINUTE = 4;
	public const int SECOND = 5;
	public const int MILLISECOND = 6;

	static public CalendarHashTable manager = new CalendarHashTable();

    public class CalendarHashTable:System.Collections.Hashtable {
	    public System.DateTime GetDateTime(System.Globalization.Calendar calendar) {
		if (this[calendar] != null)
		    return ((CalendarProperties) this[calendar]).dateTime;
		else {
		    CalendarProperties tempProps = new CalendarProperties();
		    tempProps.dateTime = System.DateTime.Now;
		    this.Add(calendar, tempProps);
		    return this.GetDateTime(calendar);
		}
	    }
	    public void SetDateTime(System.Globalization.Calendar calendar, System.DateTime date) {
		if (this[calendar] != null) {
		    ((CalendarProperties) this[calendar]).dateTime = date;
		} else {
		    CalendarProperties tempProps = new CalendarProperties();
		    tempProps.dateTime = date;
		    this.Add(calendar, tempProps);
		}
	    }
	    public void Set(System.Globalization.Calendar calendar, int field, int fieldValue) {
		if (this[calendar] != null) {
		    System.DateTime tempDate = ((CalendarProperties) this[calendar]).dateTime;
		    switch (field) {
			case CalendarManager.DATE:
			    tempDate = tempDate.AddDays(fieldValue - tempDate.Year);
			    break;
			case CalendarManager.HOUR:
			    tempDate = tempDate.AddHours(fieldValue - tempDate.Hour);
			    break;
			case CalendarManager.MILLISECOND:
			    tempDate = tempDate.AddMilliseconds(fieldValue - tempDate.Millisecond);
			    break;
			case CalendarManager.MINUTE:
			    tempDate = tempDate.AddMinutes(fieldValue - tempDate.Minute);
			    break;
			case CalendarManager.MONTH:
			    //In java.util.Calendar, Month value is 0-based. e.g., 0 for January
			    tempDate = tempDate.AddMonths(fieldValue - (tempDate.Month + 1));
			    break;
			case CalendarManager.SECOND:
			    tempDate = tempDate.AddSeconds(fieldValue - tempDate.Second);
			    break;
			case CalendarManager.YEAR:
			    tempDate = tempDate.AddYears(fieldValue - tempDate.Year);
			    break;
			default:
			    break;
		    }
		    ((CalendarProperties) this[calendar]).dateTime = tempDate;
		} else {
		    CalendarProperties tempProps = new CalendarProperties();
		    tempProps.dateTime = System.DateTime.Now;
		    this.Add(calendar, tempProps);
		    this.Set(calendar, field, fieldValue);
		}
	    }

	    public void Set(System.Globalization.Calendar calendar, int year, int month, int day) {
		if (this[calendar] != null) {
		    this.Set(calendar, CalendarManager.YEAR, year);
		    this.Set(calendar, CalendarManager.MONTH, month);
		    this.Set(calendar, CalendarManager.DATE, day);
		} else {
		    CalendarProperties tempProps = new CalendarProperties();
		    //In java.util.Calendar, Month value is 0-based. e.g., 0 for January
		    tempProps.dateTime = new System.DateTime(year, month + 1, day);
		    this.Add(calendar, tempProps);
		}
	    }

	    public void Set(System.Globalization.Calendar calendar, int year, int month, int day, int hour, int minute) {
		if (this[calendar] != null) {
		    this.Set(calendar, CalendarManager.YEAR, year);
		    this.Set(calendar, CalendarManager.MONTH, month);
		    this.Set(calendar, CalendarManager.DATE, day);
		    this.Set(calendar, CalendarManager.HOUR, hour);
		    this.Set(calendar, CalendarManager.MINUTE, minute);
		} else {
		    CalendarProperties tempProps = new CalendarProperties();
		    //In java.util.Calendar, Month value is 0-based. e.g., 0 for January
		    tempProps.dateTime = new System.DateTime(year, month + 1, day, hour, minute, 0);
		    this.Add(calendar, tempProps);
		}
	    }

	    public void Set(System.Globalization.Calendar calendar, int year, int month, int day, int hour, int minute, int second) {
		if (this[calendar] != null) {
		    this.Set(calendar, CalendarManager.YEAR, year);
		    this.Set(calendar, CalendarManager.MONTH, month);
		    this.Set(calendar, CalendarManager.DATE, day);
		    this.Set(calendar, CalendarManager.HOUR, hour);
		    this.Set(calendar, CalendarManager.MINUTE, minute);
		    this.Set(calendar, CalendarManager.SECOND, second);
		} else {
		    CalendarProperties tempProps = new CalendarProperties();
		    //In java.util.Calendar, Month value is 0-based. e.g., 0 for January
		    tempProps.dateTime = new System.DateTime(year, month + 1, day, hour, minute, second);
		    this.Add(calendar, tempProps);
		}
	    }

	    public int  Get(System.Globalization.Calendar calendar, int field) {
		if (this[calendar] != null) {
		    switch (field) {
			case CalendarManager.DATE:
			    return ((CalendarProperties) this[calendar]).dateTime.Day;
			case CalendarManager.HOUR:
			    return ((CalendarProperties) this[calendar]).dateTime.Hour;
			case CalendarManager.MILLISECOND:
			    return ((CalendarProperties) this[calendar]).dateTime.Millisecond;
			case CalendarManager.MINUTE:
			    return ((CalendarProperties) this[calendar]).dateTime.Minute;
			case CalendarManager.MONTH:
			    //In java.util.Calendar, Month value is 0-based. e.g., 0 for January
			    return ((CalendarProperties) this[calendar]).dateTime.Month - 1;
			case CalendarManager.SECOND:
			    return ((CalendarProperties) this[calendar]).dateTime.Second;
			case CalendarManager.YEAR:
			    return ((CalendarProperties) this[calendar]).dateTime.Year;
			default:
			    return 0;
		    }
		} else {
		    CalendarProperties tempProps = new CalendarProperties();
		    tempProps.dateTime = System.DateTime.Now;
		    this.Add(calendar, tempProps);
		    return this.Get(calendar, field);
		}
	    }

	    public void SetTimeInMilliseconds(System.Globalization.Calendar calendar, long milliseconds) {
		if (this[calendar] != null) {
		    ((CalendarProperties) this[calendar]).dateTime = new System.DateTime(milliseconds);
		} else {
		    CalendarProperties tempProps = new CalendarProperties();
		    tempProps.dateTime = new System.DateTime(System.TimeSpan.TicksPerMillisecond * milliseconds);
		    this.Add(calendar, tempProps);
		}
	    }

	    public System.DayOfWeek GetFirstDayOfWeek(System.Globalization.Calendar calendar) {
		if (this[calendar] != null && ((CalendarProperties)this[calendar]).dateTimeFormat != null) {
		    return ((CalendarProperties) this[calendar]).dateTimeFormat.FirstDayOfWeek;
		} else {
		    CalendarProperties tempProps = new CalendarProperties();
		    tempProps.dateTimeFormat = System.Globalization.DateTimeFormatInfo.CurrentInfo;
		    this.Add(calendar, tempProps);
		    return this.GetFirstDayOfWeek(calendar);
		}
	    }

	    public void SetFirstDayOfWeek(System.Globalization.Calendar calendar, System.DayOfWeek firstDayOfWeek) {
		if (this[calendar] != null && ((CalendarProperties)this[calendar]).dateTimeFormat != null) {
		    ((CalendarProperties) this[calendar]).dateTimeFormat.FirstDayOfWeek = firstDayOfWeek;
		} else {
		    CalendarProperties tempProps = new CalendarProperties();
		    tempProps.dateTimeFormat = System.Globalization.DateTimeFormatInfo.CurrentInfo;
		    this.Add(calendar, tempProps);
		    this.SetFirstDayOfWeek(calendar, firstDayOfWeek);
		}
	    }

	    public void Clear(System.Globalization.Calendar calendar) {
		if (this[calendar] != null)
		    this.Remove(calendar);
	    }

	    public void Clear(System.Globalization.Calendar calendar, int field) {
		if (this[calendar] != null)
		    this.Remove(calendar);
		else
		    this.Set(calendar, field, 0);
	    }

	    class CalendarProperties {
		public System.DateTime dateTime;
		public System.Globalization.DateTimeFormatInfo dateTimeFormat;
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

    /*******************************/
    static public System.Random Random = new System.Random();

}
