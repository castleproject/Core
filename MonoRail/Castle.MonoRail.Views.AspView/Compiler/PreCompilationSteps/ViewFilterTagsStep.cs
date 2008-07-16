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
    using System.Text.RegularExpressions;

	public class ViewFilterTagsStep : IPreCompilationStep
	{
		public void Process(SourceFile file)
		{
			file.RenderBody = Internal.RegularExpressions.ViewFiltersTags.Replace(file.RenderBody, HandleViewFilterTag);
		}

		private string HandleViewFilterTag(Match match)
		{
			string filterName = match.Groups["filterName"].Value;
			if (!filterName.EndsWith("ViewFilter", StringComparison.CurrentCultureIgnoreCase))
				filterName += "ViewFilter";
			string content = match.Groups["content"].Value;
			content = Internal.RegularExpressions.ViewFiltersTags.Replace(content, HandleViewFilterTag);
			string openTag = FilterCanBeBoundEarly(filterName)
			                 	?
			                 		GetEarlyBoundViewFilterOpenStatement(filterName)
			                 	:
			                 		GetLateBoundViewFilterOpenStatement(filterName);
			return string.Format("<% {0} %>{1}<% EndFiltering(); %>", openTag, content);
		}

		private bool FilterCanBeBoundEarly(string filterName)
		{
			Type t = Type.GetType(GetAssemblyQualifiedViewFilterName(filterName), false, true);
			return t != null;
		}

		protected string GetEarlyBoundViewFilterOpenStatement(string filterName)
		{
			return string.Format("StartFiltering(new {0}());",
				GetAssemblyQualifiedViewFilterName(filterName));
		}	

		protected static string GetLateBoundViewFilterOpenStatement(string filterName)
		{
			return string.Format("StartFiltering(\"{0}\");", filterName);
		}

		protected virtual string GetAssemblyQualifiedViewFilterName(string filterName)
		{
			return "Castle.MonoRail.Views.AspView.ViewFilters." + filterName;
		}

	}
}
