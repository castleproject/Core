//
// OptionDetails.cs
//
// Author: Rafael Teixeira (rafaelteixeirabr@hotmail.com)
//
// (C) 2002 Rafael Teixeira
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Mono.GetOptions
{
	using System;
	using System.Collections;
	using System.Reflection;

	public enum WhatToDoNext
	{
		AbandonProgram,
		GoAhead
	}

	internal enum OptionProcessingResult
	{
		NotThisOption,
		OptionAlone,
		OptionConsumedParameter
	}

	internal class OptionDetails : IComparable
	{
		public string ShortForm;
		public string LongForm;
		public string AlternateForm;
		public string ShortDescription;
		public bool NeedsParameter;
		public int MaxOccurs; // negative means there is no limit
		public int Occurs;
		public bool BooleanOption;
		public Options OptionBundle;
		public MemberInfo MemberInfo;
		public ArrayList Values;
		public Type ParameterType;
		public string paramName = null;
		public bool VBCStyleBoolean;
		public bool SecondLevelHelp;
		public bool Hidden;

		public OptionDetails NextAlternate = null;

		private string ExtractParamName(string shortDescription)
		{
			int whereBegins = shortDescription.IndexOf("{");
			if (whereBegins < 0)
				paramName = "PARAM";
			else
			{
				int whereEnds = shortDescription.IndexOf("}");
				if (whereEnds < whereBegins)
					whereEnds = shortDescription.Length + 1;

				paramName = shortDescription.Substring(whereBegins + 1, whereEnds - whereBegins - 1);
				shortDescription =
					shortDescription.Substring(0, whereBegins) +
					paramName +
					shortDescription.Substring(whereEnds + 1);
			}
			return shortDescription;
		}

		public string ParamName
		{
			get { return paramName; }
		}

		private bool verboseParsing
		{
			get { return OptionBundle.VerboseParsingOfOptions || OptionBundle.DebuggingOfOptions; }
		}

//		private bool debugOptions { get { return OptionBundle.DebuggingOfOptions; } }

		private OptionsParsingMode parsingMode
		{
			get { return OptionBundle.ParsingMode; }
		}

		private bool useGNUFormat
		{
			get { return (parsingMode & OptionsParsingMode.GNU_DoubleDash) == OptionsParsingMode.GNU_DoubleDash; }
		}

		private bool dontSplitOnCommas
		{
			get { return OptionBundle.DontSplitOnCommas; }
		}

		private string linuxLongPrefix
		{
			get { return (useGNUFormat ? "--" : "-"); }
		}

		public string DefaultForm
		{
			get
			{
				string shortPrefix = "-";
				string longPrefix = linuxLongPrefix;
				if (parsingMode == OptionsParsingMode.Windows)
				{
					shortPrefix = "/";
					longPrefix = "/";
				}
				if (ShortForm != string.Empty)
					return shortPrefix + ShortForm;
				else
					return longPrefix + LongForm;
			}
		}

		private string optionHelp = null;

		public override string ToString()
		{
			if (optionHelp == null)
			{
				string shortPrefix;
				string longPrefix;
				bool hasLongForm = (LongForm != null && LongForm != string.Empty);
				if (OptionBundle.ParsingMode == OptionsParsingMode.Windows)
				{
					shortPrefix = "/";
					longPrefix = "/";
				}
				else
				{
					shortPrefix = "-";
					longPrefix = linuxLongPrefix;
				}
				optionHelp = "  ";
				optionHelp += (ShortForm != string.Empty) ? shortPrefix + ShortForm + " " : "   ";
				optionHelp += hasLongForm ? longPrefix + LongForm : "";
				if (NeedsParameter)
				{
					if (hasLongForm)
						optionHelp += ":";
					optionHelp += ParamName;
				}
				else if (BooleanOption && VBCStyleBoolean)
				{
					optionHelp += "[+|-]";
				}
				optionHelp += "\t";
				if (AlternateForm != string.Empty && AlternateForm != null)
					optionHelp += "Also " + shortPrefix + AlternateForm + (NeedsParameter ? (":" + ParamName) : "") + ". ";
				optionHelp += ShortDescription;
			}
			return optionHelp;
		}

		private static Type TypeOfMember(MemberInfo memberInfo)
		{
			if ((memberInfo.MemberType == MemberTypes.Field && memberInfo is FieldInfo))
				return ((FieldInfo) memberInfo).FieldType;

			if ((memberInfo.MemberType == MemberTypes.Property && memberInfo is PropertyInfo))
				return ((PropertyInfo) memberInfo).PropertyType;

			if ((memberInfo.MemberType == MemberTypes.Method && memberInfo is MethodInfo))
			{
				if (((MethodInfo) memberInfo).ReturnType.FullName != typeof(WhatToDoNext).FullName)
					throw new NotSupportedException("Option method must return '" + typeof(WhatToDoNext).FullName + "'");

				ParameterInfo[] parameters = ((MethodInfo) memberInfo).GetParameters();
				if ((parameters == null) || (parameters.Length == 0))
					return null;
				else
					return parameters[0].ParameterType;
			}

			throw new NotSupportedException("'" + memberInfo.MemberType + "' memberType is not supported");
		}

		public OptionDetails(MemberInfo memberInfo, OptionAttribute option, Options optionBundle)
		{
			ShortForm = ("" + option.ShortForm).Trim();
			if (option.LongForm == null)
				LongForm = string.Empty;
			else
				LongForm = (option.LongForm == string.Empty) ? memberInfo.Name : option.LongForm;
			AlternateForm = option.AlternateForm;
			ShortDescription = ExtractParamName(option.ShortDescription);
			Occurs = 0;
			OptionBundle = optionBundle;
			BooleanOption = false;
			MemberInfo = memberInfo;
			NeedsParameter = false;
			Values = null;
			MaxOccurs = 1;
			VBCStyleBoolean = option.VBCStyleBoolean;
			SecondLevelHelp = option.SecondLevelHelp;
			Hidden = false; // TODO: check other attributes

			ParameterType = TypeOfMember(memberInfo);

			if (ParameterType != null)
			{
				if (ParameterType.FullName != "System.Boolean")
				{
					if (LongForm.IndexOf(':') >= 0)
						throw new InvalidOperationException(
							"Options with an embedded colon (':') in their visible name must be boolean!!! [" +
							MemberInfo.ToString() + " isn't]");

					NeedsParameter = true;

					if (option.MaxOccurs != 1)
					{
						if (ParameterType.IsArray)
						{
							Values = new ArrayList();
							MaxOccurs = option.MaxOccurs;
						}
						else
						{
							if (MemberInfo is MethodInfo || MemberInfo is PropertyInfo)
								MaxOccurs = option.MaxOccurs;
							else
								throw new InvalidOperationException("MaxOccurs set to non default value (" + option.MaxOccurs + ") for a [" +
								                                    MemberInfo.ToString() + "] option");
						}
					}
				}
				else
				{
					BooleanOption = true;
					if (option.MaxOccurs != 1)
					{
						if (MemberInfo is MethodInfo || MemberInfo is PropertyInfo)
							MaxOccurs = option.MaxOccurs;
						else
							throw new InvalidOperationException("MaxOccurs set to non default value (" + option.MaxOccurs + ") for a [" +
							                                    MemberInfo.ToString() + "] option");
					}
				}
			}
		}

		internal string Key
		{
			get
			{
				if (useGNUFormat)
				{
					string ShortID = ShortForm.ToUpper();
					if (ShortID == string.Empty)
						ShortID = "ZZ";
					return ShortID + " " + LongForm;
				}
				else
					return LongForm + " " + ShortForm;
			}
		}

		int IComparable.CompareTo(object other)
		{
			return Key.CompareTo(((OptionDetails) other).Key);
		}

		public void TransferValues()
		{
			if (Values != null)
			{
				if (MemberInfo is FieldInfo)
				{
					((FieldInfo) MemberInfo).SetValue(OptionBundle, Values.ToArray(ParameterType.GetElementType()));
					return;
				}

				if (MemberInfo is PropertyInfo)
				{
					((PropertyInfo) MemberInfo).SetValue(OptionBundle, Values.ToArray(ParameterType.GetElementType()), null);
					return;
				}

				if (
					(WhatToDoNext)
					((MethodInfo) MemberInfo).Invoke(OptionBundle, new object[] {Values.ToArray(ParameterType.GetElementType())}) ==
					WhatToDoNext.AbandonProgram)
					Environment.Exit(1);
			}
		}

		private int HowManyBeforeExceedingMaxOccurs(int howMany)
		{
			if (MaxOccurs > 0 && (Occurs + howMany) > MaxOccurs)
			{
				Console.Error.WriteLine("Option " + LongForm + " can be used at most " + MaxOccurs + " times. Ignoring extras...");
				howMany = MaxOccurs - Occurs;
			}
			Occurs += howMany;
			return howMany;
		}

		private bool AddingOneMoreExceedsMaxOccurs
		{
			get { return HowManyBeforeExceedingMaxOccurs(1) < 1; }
		}

		private void DoIt(bool setValue)
		{
			if (AddingOneMoreExceedsMaxOccurs)
				return;

			if (verboseParsing)
				Console.WriteLine("<{0}> set to [{1}]", LongForm, setValue);

			if (MemberInfo is FieldInfo)
			{
				((FieldInfo) MemberInfo).SetValue(OptionBundle, setValue);
				return;
			}
			if (MemberInfo is PropertyInfo)
			{
				((PropertyInfo) MemberInfo).SetValue(OptionBundle, setValue, null);
				return;
			}
			if ((WhatToDoNext) ((MethodInfo) MemberInfo).Invoke(OptionBundle, null) == WhatToDoNext.AbandonProgram)
				Environment.Exit(1);
		}

		private void DoIt(string parameterValue)
		{
			if (parameterValue == null)
				parameterValue = "";

			string[] parameterValues;

			if (dontSplitOnCommas || MaxOccurs == 1)
				parameterValues = new string[] {parameterValue};
			else
				parameterValues = parameterValue.Split(',');

			int waitingToBeProcessed = HowManyBeforeExceedingMaxOccurs(parameterValues.Length);

			foreach(string parameter in parameterValues)
			{
				if (waitingToBeProcessed-- <= 0)
					break;

				object convertedParameter = null;

				if (verboseParsing)
					Console.WriteLine("<" + LongForm + "> set to [" + parameter + "]");

				if (Values != null && parameter != null)
				{
					try
					{
						convertedParameter = Convert.ChangeType(parameter, ParameterType.GetElementType());
					}
					catch(Exception)
					{
						Console.WriteLine(
							String.Format("The value '{0}' is not convertible to the appropriate type '{1}' for the {2} option", parameter,
							              ParameterType.GetElementType().Name, DefaultForm));
					}
					Values.Add(convertedParameter);
					continue;
				}

				if (parameter != null)
				{
					try
					{
						convertedParameter = Convert.ChangeType(parameter, ParameterType);
					}
					catch(Exception)
					{
						Console.WriteLine(
							String.Format("The value '{0}' is not convertible to the appropriate type '{1}' for the {2} option", parameter,
							              ParameterType.Name, DefaultForm));
						continue;
					}
				}

				if (MemberInfo is FieldInfo)
				{
					((FieldInfo) MemberInfo).SetValue(OptionBundle, convertedParameter);
					continue;
				}

				if (MemberInfo is PropertyInfo)
				{
					((PropertyInfo) MemberInfo).SetValue(OptionBundle, convertedParameter, null);
					continue;
				}

				if ((WhatToDoNext) ((MethodInfo) MemberInfo).Invoke(OptionBundle, new object[] {convertedParameter}) ==
				    WhatToDoNext.AbandonProgram)
					Environment.Exit(1);
			}
		}

		private bool IsThisOption(string arg)
		{
			if (arg != null && arg != string.Empty)
			{
				arg = arg.TrimStart('-', '/');
				if (VBCStyleBoolean)
					arg = arg.TrimEnd('-', '+');
				return (arg == ShortForm || arg == LongForm || arg == AlternateForm);
			}
			return false;
		}

		public static void LinkAlternatesInsideList(ArrayList list)
		{
			Hashtable baseForms = new Hashtable(list.Count);
			foreach(OptionDetails option in list)
			{
				if (option.LongForm != null && option.LongForm.Trim().Length > 0)
				{
					string[] parts = option.LongForm.Split(':');
					if (parts.Length < 2)
					{
						baseForms.Add(option.LongForm, option);
					}
					else
					{
						OptionDetails baseForm = (OptionDetails) baseForms[parts[0]];
						if (baseForm != null)
						{
							// simple linked list
							option.NextAlternate = baseForm.NextAlternate;
							baseForm.NextAlternate = option;
						}
					}
				}
			}
		}

		private bool IsAlternate(string compoundArg)
		{
			OptionDetails next = NextAlternate;
			while(next != null)
			{
				if (next.IsThisOption(compoundArg))
					return true;
				next = next.NextAlternate;
			}
			return false;
		}

		public OptionProcessingResult ProcessArgument(string arg, string nextArg)
		{
			if (IsAlternate(arg + ":" + nextArg))
				return OptionProcessingResult.NotThisOption;

			if (IsThisOption(arg))
			{
				if (!NeedsParameter)
				{
					if (VBCStyleBoolean && arg.EndsWith("-"))
						DoIt(false);
					else
						DoIt(true);
					return OptionProcessingResult.OptionAlone;
				}
				else
				{
					DoIt(nextArg);
					return OptionProcessingResult.OptionConsumedParameter;
				}
			}

			if (IsThisOption(arg + ":" + nextArg))
			{
				DoIt(true);
				return OptionProcessingResult.OptionConsumedParameter;
			}

			return OptionProcessingResult.NotThisOption;
		}
	}
}