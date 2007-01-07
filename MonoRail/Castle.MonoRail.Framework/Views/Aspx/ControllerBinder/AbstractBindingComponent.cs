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
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Text;

	[Serializable]
	public abstract class AbstractBindingComponent : IDataErrorInfo
	{
		private IDictionary<string, string> propertyErrors;

		public bool IsValid()
		{
			propertyErrors = null;
			Validate();
			return propertyErrors == null || propertyErrors.Count == 0;
		}

		protected abstract void Validate();

		#region IDataErrorInfo

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Error
		{
			get
			{
				if (propertyErrors != null)
				{
					StringBuilder errorMessage = new StringBuilder();

					foreach(KeyValuePair<string, string> error in propertyErrors)
					{
						errorMessage.AppendFormat("'{0}' {1}{2}", error.Key, error.Value,
						                          Environment.NewLine);
					}

					return errorMessage.ToString();
				}

				return string.Empty;
			}
		}

		public string this[string propertyName]
		{
			get
			{
				if (propertyErrors != null &&
				    propertyErrors.ContainsKey(propertyName))
				{
					return propertyErrors[propertyName];
				}

				return string.Empty;
			}

			protected set
			{
				if (propertyErrors == null)
				{
					propertyErrors = new Dictionary<string, string>();
				}

				propertyErrors[propertyName] = value;
			}
		}

		#endregion

		protected string Trim(string value)
		{
			return (value != null) ? value.Trim() : "";
		}
	}
}

#endif
