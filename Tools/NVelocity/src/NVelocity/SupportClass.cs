// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
					object o = this[connection];
					if (o == null)
					{
						ConnectionProperties TempProp = new ConnectionProperties();
						TempProp.AutoCommit = boolean;
						TempProp.TransactionLevel = 0;
						if (boolean)
						{
							TempProp.Transaction = connection.BeginTransaction();
						}
						Add(connection, TempProp);
					}
					else
					{
						ConnectionProperties Properties = ((ConnectionProperties) o);
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
				}

				public OleDbCommand PrepareStatement(OleDbConnection connection, string sql)
				{
					OleDbCommand command = CreateStatement(connection);
					command.CommandText = sql;
					return command;
				}

				public OleDbCommand PrepareCall(OleDbConnection connection, string sql)
				{
					OleDbCommand command = CreateStatement(connection);
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
					if (this[connection] == null)
					{
						return false;
					}
					else
					{
						return ((ConnectionProperties) this[connection]).AutoCommit;
					}
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
				elements = new ArrayList();
				elements.AddRange(source.Split(delimiters.ToCharArray()));
				RemoveEmptyStrings();
				this.source = source;
			}

			public Tokenizer(string source, string delimiters)
			{
				elements = new ArrayList();
				this.delimiters = delimiters;
				elements.AddRange(source.Split(this.delimiters.ToCharArray()));
				RemoveEmptyStrings();
				this.source = source;
			}

			public int Count
			{
				get { return (elements.Count); }
			}

			public bool HasMoreTokens()
			{
				return (elements.Count > 0);
			}

			public string NextToken()
			{
				string result;
				if (source == string.Empty)
					throw new System.Exception();
				else
				{
					elements = new ArrayList();
					elements.AddRange(source.Split(delimiters.ToCharArray()));
					RemoveEmptyStrings();
					result = (string) elements[0];
					elements.RemoveAt(0);
					source = source.Replace(result, string.Empty);
					source = source.TrimStart(delimiters.ToCharArray());
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
				for(int index = 0; index < elements.Count; index++)
					if ((string) elements[index] == string.Empty)
					{
						elements.RemoveAt(index);
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
				numberFormat = new NumberFormatInfo();
				numberFormatType = (int) formatTypes.General;
				groupingActivated = true;
				separator = GetSeparator((int) formatTypes.General);
				digits = 3;
			}

			private TextNumberFormat(formatTypes theType, int digits)
			{
				numberFormat = NumberFormatInfo.CurrentInfo;
				numberFormatType = (int) theType;
				groupingActivated = true;
				separator = GetSeparator((int) theType);
				this.digits = digits;
			}

			private TextNumberFormat(formatTypes theType, CultureInfo cultureNumberFormat, int digits)
			{
				numberFormat = cultureNumberFormat.NumberFormat;
				numberFormatType = (int) theType;
				groupingActivated = true;
				separator = GetSeparator((int) theType);
				this.digits = digits;
			}

			public static TextNumberFormat GetTextNumberInstance()
			{
				TextNumberFormat instance = new TextNumberFormat(formatTypes.Number, 3);
				return instance;
			}

			public static TextNumberFormat GetTextNumberCurrencyInstance()
			{
				TextNumberFormat instance = new TextNumberFormat(formatTypes.Currency, 3);
				return instance;
			}

			public static TextNumberFormat GetTextNumberPercentInstance()
			{
				TextNumberFormat instance = new TextNumberFormat(formatTypes.Percent, 3);
				return instance;
			}

			public static TextNumberFormat GetTextNumberInstance(CultureInfo culture)
			{
				TextNumberFormat instance = new TextNumberFormat(formatTypes.Number, culture, 3);
				return instance;
			}

			public static TextNumberFormat GetTextNumberCurrencyInstance(CultureInfo culture)
			{
				TextNumberFormat instance = new TextNumberFormat(formatTypes.Currency, culture, 3);
				return instance;
			}

			public static TextNumberFormat GetTextNumberPercentInstance(CultureInfo culture)
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
				return Equals((Object) this, textNumberObject);
			}

			public string FormatDouble(double number)
			{
				if (groupingActivated)
				{
					return number.ToString(GetCurrentFormatString() + digits, numberFormat);
				}
				else
				{
					return (number.ToString(GetCurrentFormatString() + digits, numberFormat)).Replace(separator, string.Empty);
				}
			}

			public string FormatLong(long number)
			{
				if (groupingActivated)
				{
					return number.ToString(GetCurrentFormatString() + digits, numberFormat);
				}
				else
				{
					return (number.ToString(GetCurrentFormatString() + digits, numberFormat)).Replace(separator, string.Empty);
				}
			}

			public static CultureInfo[] GetAvailableCultures()
			{
				return CultureInfo.GetCultures(CultureTypes.AllCultures);
			}

			public override int GetHashCode()
			{
				return GetHashCode();
			}

			private string GetCurrentFormatString()
			{
				string currentFormatString = "n"; //Default value
				switch(numberFormatType)
				{
					case (int) formatTypes.Currency:
						currentFormatString = "c";
						break;

					case (int) formatTypes.General:
						currentFormatString = string.Format("n{0}", numberFormat.NumberDecimalDigits);
						break;

					case (int) formatTypes.Number:
						currentFormatString = string.Format("n{0}", numberFormat.NumberDecimalDigits);
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
						separatorItem = numberFormat.CurrencyGroupSeparator;
						break;

					case (int) formatTypes.General:
						separatorItem = numberFormat.NumberGroupSeparator;
						break;

					case (int) formatTypes.Number:
						separatorItem = numberFormat.NumberGroupSeparator;
						break;

					case (int) formatTypes.Percent:
						separatorItem = numberFormat.PercentGroupSeparator;
						break;
				}
				return separatorItem;
			}

			public bool GroupingUsed
			{
				get { return (groupingActivated); }
				set { groupingActivated = value; }
			}

			public int Digits
			{
				get { return digits; }
				set { digits = value; }
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
			return date.ToString(string.Format("{0} {1}", datePattern, timePattern), format);
		}

		/*******************************/

		public static DateTimeFormatInfo GetDateTimeFormatInstance(int dateStyle, int timeStyle, CultureInfo culture)
		{
			DateTimeFormatInfo format = culture.DateTimeFormat;

			switch(timeStyle)
			{
				case -1:
					DateTimeFormatManager.manager.SetTimeFormatPattern(format, string.Empty);
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
					DateTimeFormatManager.manager.SetDateFormatPattern(format, string.Empty);
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