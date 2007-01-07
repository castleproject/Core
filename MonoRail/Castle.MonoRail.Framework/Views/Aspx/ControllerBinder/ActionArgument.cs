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

#if (DOTNET2 && NET)

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.ComponentModel;
	using System.Text.RegularExpressions;

	[Serializable]
	[TypeConverter(typeof(ExpandableObjectConverter))]	
	public class ActionArgument : AbstractBindingComponent, ICloneable
	{
		private string name;
		private object value;
		private string expression;

		private static readonly Regex nameRegexRule =
			new Regex("^[A-za-z]([A-za-z0-9]*)$");

		[Category("Data"), DefaultValue("")]
		[Description("The name of the argument to pass to the controller action.")]
		public string Name
		{
			get { return name; }
			set { name = Trim(value); }
		}

		[DisplayName("Value")]
		[Category("Data"), DefaultValue((string) null)]
		[Description("The expression for the argument to pass to the controller action.")]
		public string Expression
		{
			get { return expression; }
			set{ expression = Trim(value); }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Value
		{
			get { return value; }
			set { this.value = value; }
		}

		protected override void Validate()
		{
			if (string.IsNullOrEmpty(name))
			{
				this["Name"] = "must be specified";
			}
			else if (!nameRegexRule.IsMatch(name))
			{
				this["Name"] = "must start with a letter and contain only letters and digits";	
			}

			if (string.IsNullOrEmpty(expression) && value == null)
			{
				this["Value"] = "must be specified";
			}
		}

		public object Clone()
		{
			ActionArgument actionArg = new ActionArgument();
			actionArg.name = name;
			actionArg.value = value;
			actionArg.expression = expression;
			return actionArg;
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(name))
			{
				return "Action Argument";
			}

			return name;
		}
	}
}

#endif
