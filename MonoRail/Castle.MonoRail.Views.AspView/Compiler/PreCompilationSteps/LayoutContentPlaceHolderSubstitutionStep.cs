// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.AspView.Compiler.PreCompilationSteps
{
	using System;
	using System.Collections;
	using System.Text.RegularExpressions;

	public class LayoutContentPlaceHolderSubstitutionStep : IPreCompilationStep 
	{
		public static class ExceptionMessages
		{
			public const string IdAttributeNotFound = "asp:contentplaceholder tag should have 'id' attribute";
			public const string IdAttributeEmpty = "asp:contentplaceholder tag 'id' attribute should have non empty value";
			public const string ViewPropertyAllreadyRegisteredWithOtherTypeFormat = "there already exist a property named {0} which type is not string";
		}

		#region IPreCompilationStep Members

		public void Process(SourceFile file)
		{
			file.RenderBody = Internal.RegularExpressions.LayoutContentPlaceHolder.Replace(
				file.RenderBody,
				delegate(Match match) 
				{
					string parsedAttributes = match.Groups["attributes"].Value;
					IDictionary attributes = Utilities.GetAttributesDictionaryFrom(parsedAttributes);
					if(attributes.Contains("runat") && String.Equals("server",(attributes["runat"] as string), StringComparison.InvariantCultureIgnoreCase))
					{
						if (!attributes.Contains("id"))
							throw new AspViewException(ExceptionMessages.IdAttributeNotFound);
						if (String.IsNullOrEmpty((string)attributes["id"]))
							throw new AspViewException(ExceptionMessages.IdAttributeEmpty);

						string placeholderid = (string) attributes["id"];
						if(!file.Properties.ContainsKey(placeholderid))
						{
							// handle ViewContents special case
							if (placeholderid != "ViewContents")
								file.Properties.Add(placeholderid, new ViewProperty(placeholderid, "string", @""""""));
						}
						else if(!String.Equals(file.Properties[placeholderid].Type, "string", StringComparison.InvariantCultureIgnoreCase))
						{
							throw new AspViewException(String.Format(ExceptionMessages.ViewPropertyAllreadyRegisteredWithOtherTypeFormat, placeholderid));
						}
						return String.Format("<%={0}%>", placeholderid);
					}
					else
					{
						return match.Value;
					}
				});
		}

		#endregion
	}
}
