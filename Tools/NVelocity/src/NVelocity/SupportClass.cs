namespace NVelocity
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.OleDb;
	using System.Globalization;
	using System.IO;
	using System.Reflection;

	public class SupportClass
	{
		public static sbyte[] ToSByteArray(byte[] byteArray)
		{
			sbyte[] sbyteArray = new sbyte[byteArray.Length];
			for(int index = 0; index < byteArray.Length; index++)
				sbyteArray[index] = (sbyte) byteArray[index];
			return sbyteArray;
		}

		/*******************************/

		public static byte[] ToByteArray(sbyte[] sbyteArray)
		{
			byte[] byteArray = new byte[sbyteArray.Length];
			for(int index = 0; index < sbyteArray.Length; index++)
				byteArray[index] = (byte) sbyteArray[index];
			return byteArray;
		}

		/*******************************/

		public static Object PutElement(Hashtable hashTable, Object key, Object newValue)
		{
			Object element = hashTable[key];
			hashTable[key] = newValue;
			return element;
		}

		/*******************************/

		public class TransactionManager
		{
			public static ConnectionHashTable manager = new ConnectionHashTable();

			public class ConnectionHashTable : Hashtable
			{
				public OleDbCommand CreateStatement(OleDbConnection connection)
				{
					OleDbCommand command = connection.CreateCommand();
					OleDbTransaction transaction;
					if (this[connection] != null)
					{
						ConnectionProperties Properties = ((ConnectionProperties) this[connection]);
						transaction = Properties.Transaction;
						command.Transaction = transaction;
					}
					else
					{
						ConnectionProperties TempProp = new ConnectionProperties();
						TempProp.AutoCommit = false;
						TempProp.TransactionLevel = 0;
						command.Transaction = TempProp.Transaction;
						Add(connection, TempProp);
					}
					return command;
				}


				public void Commit(OleDbConnection connection)
				{
					if (this[connection] != null && ((ConnectionProperties) this[connection]).AutoCommit)
					{
						OleDbTransaction transaction = ((ConnectionProperties) this[connection]).Transaction;
						transaction.Commit();
					}
				}

				public void RollBack(OleDbConnection connection)
				{
					if (this[connection] != null && ((ConnectionProperties) this[connection]).AutoCommit)
					{
						OleDbTransaction transaction = ((ConnectionProperties) this[connection]).Transaction;
						transaction.Rollback();
					}
				}

				public void SetAutoCommit(OleDbConnection connection, bool boolean)
				{
					if (this[connection] != null)
					{
						ConnectionProperties Properties = ((ConnectionProperties) this[connection]);
						Properties.AutoCommit = boolean;
						if (!boolean)
						{
							OleDbTransaction transaction = Properties.Transaction;
							if (Properties.TransactionLevel == 0)
							{
								transaction = connection.BeginTransaction();
							}
							else
							{
								transaction = connection.BeginTransaction(Properties.TransactionLevel);
							}
						}
					}
					else
					{
						ConnectionProperties TempProp = new ConnectionProperties();
						TempProp.AutoCommit = boolean;
						TempProp.TransactionLevel = 0;
						if (boolean)
							TempProp.Transaction = connection.BeginTransaction();
						Add(connection, TempProp);
					}
				}

				public OleDbCommand PrepareStatement(OleDbConnection connection, string sql)
				{
					OleDbCommand command = this.CreateStatement(connection);
					command.CommandText = sql;
					return command;
				}

				public OleDbCommand PrepareCall(OleDbConnection connection, string sql)
				{
					OleDbCommand command = this.CreateStatement(connection);
					command.CommandText = sql;
					return command;
				}

				public void SetTransactionIsolation(OleDbConnection connection, int level)
				{
					if (this[connection] != null)
					{
						ConnectionProperties Properties = ((ConnectionProperties) this[connection]);
						Properties.TransactionLevel = (IsolationLevel) level;
					}
					else
					{
						ConnectionProperties TempProp = new ConnectionProperties();
						TempProp.AutoCommit = false;
						TempProp.TransactionLevel = (IsolationLevel) level;
						Add(connection, TempProp);
					}

				}

				public int GetTransactionIsolation(OleDbConnection connection)
				{
					if (this[connection] != null)
					{
						ConnectionProperties Properties = ((ConnectionProperties) this[connection]);
						if (Properties.TransactionLevel != 0)
							return (int) Properties.TransactionLevel;
						else
							return 2;
					}
					else
						return 2;
				}

				public bool GetAutoCommit(OleDbConnection connection)
				{
					if (this[connection] != null)
					{
						return ((ConnectionProperties) this[connection]).AutoCommit;
					}
					else
						return false;
				}

				private class ConnectionProperties
				{
					public bool AutoCommit;
					public OleDbTransaction Transaction;
					public IsolationLevel TransactionLevel;
				}

			}
		}


		/*******************************/

		public class Tokenizer
		{
			private ArrayList elements;
			private string source;
			//The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character
			private string delimiters = " \t\n\r";

			public Tokenizer(string source)
			{
				this.elements = new ArrayList();
				this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
				this.RemoveEmptyStrings();
				this.source = source;
			}

			public Tokenizer(string source, string delimiters)
			{
				this.elements = new ArrayList();
				this.delimiters = delimiters;
				this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
				this.RemoveEmptyStrings();
				this.source = source;
			}

			public int Count
			{
				get { return (this.elements.Count); }
			}

			public bool HasMoreTokens()
			{
				return (this.elements.Count > 0);
			}

			public string NextToken()
			{
				string result;
				if (source == "")
					throw new System.Exception();
				else
				{
					this.elements = new ArrayList();
					this.elements.AddRange(this.source.Split(delimiters.ToCharArray()));
					RemoveEmptyStrings();
					result = (string) this.elements[0];
					this.elements.RemoveAt(0);
					this.source = this.source.Replace(result, "");
					this.source = this.source.TrimStart(this.delimiters.ToCharArray());
					return result;
				}
			}

			public string NextToken(string delimiters)
			{
				this.delimiters = delimiters;
				return NextToken();
			}

			private void RemoveEmptyStrings()
			{
				//VJ++ does not treat empty strings as tokens
				for(int index = 0; index < this.elements.Count; index++)
					if ((string) this.elements[index] == "")
					{
						this.elements.RemoveAt(index);
						index--;
					}
			}
		}

		/*******************************/

		public static long FileLength(FileInfo file)
		{
			if (Directory.Exists(file.FullName))
				return 0;
			else
				return file.Length;
		}

		/*******************************/

		public static void WriteStackTrace(System.Exception throwable, TextWriter stream)
		{
			stream.Write(throwable.StackTrace);
			stream.Flush();
		}


		public class TextNumberFormat
		{
			// Declaration of fields
			private NumberFormatInfo numberFormat;

			private enum formatTypes
			{
				General,
				Number,
				Currency,
				Percent
			} ;

			private int numberFormatType;
			private bool groupingActivated;
			private string separator;
			private int digits;

			// CONSTRUCTORS
			public TextNumberFormat()
			{
				this.numberFormat = new NumberFormatInfo();
				this.numberFormatType = (int) formatTypes.General;
				this.groupingActivated = true;
				this.separator = this.GetSeparator((int) formatTypes.General);
				this.digits = 3;
			}

			private TextNumberFormat(formatTypes theType, int digits)
			{
				this.numberFormat = NumberFormatInfo.CurrentInfo;
				this.numberFormatType = (int) theType;
				this.groupingActivated = true;
				this.separator = this.GetSeparator((int) theType);
				this.digits = digits;
			}

			private TextNumberFormat(formatTypes theType, CultureInfo cultureNumberFormat, int digits)
			{
				this.numberFormat = cultureNumberFormat.NumberFormat;
				this.numberFormatType = (int) theType;
				this.groupingActivated = true;
				this.separator = this.GetSeparator((int) theType);
				this.digits = digits;
			}

			public static TextNumberFormat getTextNumberInstance()
			{
				TextNumberFormat instance = new TextNumberFormat(formatTypes.Number, 3);
				return instance;
			}

			public static TextNumberFormat getTextNumberCurrencyInstance()
			{
				TextNumberFormat instance = new TextNumberFormat(formatTypes.Currency, 3);
				return instance;
			}

			public static TextNumberFormat getTextNumberPercentInstance()
			{
				TextNumberFormat instance = new TextNumberFormat(formatTypes.Percent, 3);
				return instance;
			}

			public static TextNumberFormat getTextNumberInstance(CultureInfo culture)
			{
				TextNumberFormat instance = new TextNumberFormat(formatTypes.Number, culture, 3);
				return instance;
			}

			public static TextNumberFormat getTextNumberCurrencyInstance(CultureInfo culture)
			{
				TextNumberFormat instance = new TextNumberFormat(formatTypes.Currency, culture, 3);
				return instance;
			}

			public static TextNumberFormat getTextNumberPercentInstance(CultureInfo culture)
			{
				TextNumberFormat instance = new TextNumberFormat(formatTypes.Percent, culture, 3);
				return instance;
			}

			public Object Clone()
			{
				return (Object) this;
			}

			public override bool Equals(Object textNumberObject)
			{
				return Object.Equals((Object) this, textNumberObject);
			}

			public string FormatDouble(double number)
			{
				if (this.groupingActivated)
				{
					return number.ToString(this.GetCurrentFormatString() + this.digits, this.numberFormat);
				}
				else
				{
					return (number.ToString(this.GetCurrentFormatString() + this.digits, this.numberFormat)).Replace(this.separator, "");
				}
			}

			public string FormatLong(long number)
			{
				if (this.groupingActivated)
				{
					return number.ToString(this.GetCurrentFormatString() + this.digits, this.numberFormat);
				}
				else
				{
					return (number.ToString(this.GetCurrentFormatString() + this.digits, this.numberFormat)).Replace(this.separator, "");
				}
			}

			public static CultureInfo[] GetAvailableCultures()
			{
				return CultureInfo.GetCultures(CultureTypes.AllCultures);
			}

			public override int GetHashCode()
			{
				return this.GetHashCode();
			}

			private string GetCurrentFormatString()
			{
				string currentFormatString = "n"; //Default value
				switch(this.numberFormatType)
				{
					case (int) formatTypes.Currency:
						currentFormatString = "c";
						break;

					case (int) formatTypes.General:
						currentFormatString = "n" + this.numberFormat.NumberDecimalDigits;
						break;

					case (int) formatTypes.Number:
						currentFormatString = "n" + this.numberFormat.NumberDecimalDigits;
						break;

					case (int) formatTypes.Percent:
						currentFormatString = "p";
						break;
				}
				return currentFormatString;
			}

			private string GetSeparator(int numberFormatType)
			{
				string separatorItem = " "; //Default Separator

				switch(numberFormatType)
				{
					case (int) formatTypes.Currency:
						separatorItem = this.numberFormat.CurrencyGroupSeparator;
						break;

					case (int) formatTypes.General:
						separatorItem = this.numberFormat.NumberGroupSeparator;
						break;

					case (int) formatTypes.Number:
						separatorItem = this.numberFormat.NumberGroupSeparator;
						break;

					case (int) formatTypes.Percent:
						separatorItem = this.numberFormat.PercentGroupSeparator;
						break;
				}
				return separatorItem;
			}

			public bool GroupingUsed
			{
				get { return (this.groupingActivated); }
				set { this.groupingActivated = value; }
			}

			public int Digits
			{
				get { return this.digits; }
				set { this.digits = value; }
			}
		}

		/*******************************/

		public class DateTimeFormatManager
		{
			public static DateTimeFormatHashTable manager = new DateTimeFormatHashTable();

			public class DateTimeFormatHashTable : Hashtable
			{
				public void SetDateFormatPattern(DateTimeFormatInfo format, String newPattern)
				{
					if (this[format] != null)
						((DateTimeFormatProperties) this[format]).DateFormatPattern = newPattern;
					else
					{
						DateTimeFormatProperties tempProps = new DateTimeFormatProperties();
						tempProps.DateFormatPattern = newPattern;
						Add(format, tempProps);
					}
				}

				public string GetDateFormatPattern(DateTimeFormatInfo format)
				{
					if (this[format] == null)
						return "d-MMM-yy";
					else
						return ((DateTimeFormatProperties) this[format]).DateFormatPattern;
				}

				public void SetTimeFormatPattern(DateTimeFormatInfo format, String newPattern)
				{
					if (this[format] != null)
						((DateTimeFormatProperties) this[format]).TimeFormatPattern = newPattern;
					else
					{
						DateTimeFormatProperties tempProps = new DateTimeFormatProperties();
						tempProps.TimeFormatPattern = newPattern;
						Add(format, tempProps);
					}
				}

				public string GetTimeFormatPattern(DateTimeFormatInfo format)
				{
					if (this[format] == null)
						return "h:mm:ss tt";
					else
						return ((DateTimeFormatProperties) this[format]).TimeFormatPattern;
				}

				private class DateTimeFormatProperties
				{
					public string DateFormatPattern = "d-MMM-yy";
					public string TimeFormatPattern = "h:mm:ss tt";
				}
			}
		}

		/*******************************/

		public static string FormatDateTime(DateTimeFormatInfo format, DateTime date)
		{
			string timePattern = DateTimeFormatManager.manager.GetTimeFormatPattern(format);
			string datePattern = DateTimeFormatManager.manager.GetDateFormatPattern(format);
			return date.ToString(datePattern + " " + timePattern, format);
		}

		/*******************************/

		public static DateTimeFormatInfo GetDateTimeFormatInstance(int dateStyle, int timeStyle, CultureInfo culture)
		{
			DateTimeFormatInfo format = culture.DateTimeFormat;

			switch(timeStyle)
			{
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

			switch(dateStyle)
			{
				case -1:
					DateTimeFormatManager.manager.SetDateFormatPattern(format, "");
					break;

				case 0:
					DateTimeFormatManager.manager.SetDateFormatPattern(format, "dddd, MMMM dd%, yyy");
					break;

				case 1:
					DateTimeFormatManager.manager.SetDateFormatPattern(format, "MMMM dd%, yyy");
					break;

				case 2:
					DateTimeFormatManager.manager.SetDateFormatPattern(format, "d-MMM-yy");
					break;

				case 3:
					DateTimeFormatManager.manager.SetDateFormatPattern(format, "M/dd/yy");
					break;
			}

			return format;
		}

		/*******************************/

		/// <summary>
		/// Creates an instance of a received Type
		/// </summary>
		/// <param name="classType">The Type of the new class instance to return</param>
		/// <returns>An Object containing the new instance</returns>
		public static Object CreateNewInstance(Type classType)
		{
			ConstructorInfo[] constructors = classType.GetConstructors();

			if (constructors.Length == 0)
				return null;

			ParameterInfo[] firstConstructor = constructors[0].GetParameters();
			int countParams = firstConstructor.Length;

			Type[] constructor = new Type[countParams];
			for(int i = 0; i < countParams; i++)
				constructor[i] = firstConstructor[i].ParameterType;

			return classType.GetConstructor(constructor).Invoke(new Object[] {});
		}

	}
}