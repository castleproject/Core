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

namespace Castle.MonoRail.Views.AspView.Internal
{
	using System.Text.RegularExpressions;

	public static class RegularExpressions
	{
		public static readonly Regex PageDirective = new Regex(
@"<%@\s*Page\s+Language\s*=\s*""c#""(?:\s+Inherits\s*=\s*""(?<base>[\w.]+)(?:<(?<view>[\w.<>]+)>)?\s*"")?.*%>\s*\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		public static readonly Regex MasterPageDirective = new Regex(
@"<%@\s*Master\s+Language\s*=\s*""c#""(?:\s+Inherits\s*=\s*""(?<base>[\w.]+)(?:<(?<view>[\w.<>]+)>)?\s*"")?.*%>\s*\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		public static readonly Regex ImportDirective = new Regex(
@"<%@\s*Import\s+Namespace\s*=\s*""(?<namespace>[\w.]+)""\s*%>\s*\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public static readonly Regex PropertiesSection = new Regex(
@"<aspview:properties>(?:\s*<%)?(?<properties>.*)(?:%\s*>)?</aspview:properties>\s*\n", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

		public static readonly Regex PropertiesServerScriptSection = new Regex(
@"<script\s+runat\s*=\s*\""server\""\s+type=""aspview/properties""\s*>(?<properties>((?!</script>).)*)</script>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

		public static readonly Regex SubViewTags = new Regex(
@"<subView:(?<viewName>[\w\.]+)" + attributesBlock + @">\s*</subView:\k<viewName>>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public static readonly Regex Attributes = new Regex(
@"\s*(?<name>\w+)\s*=\s*""(?<value>(\s*<%=\s*[\w\.\(\)\[\]""]+\s*%>\s*)|([^""]*))""\s*");

		public static readonly Regex ViewFiltersTags = new Regex("<filter:(?<filterName>\\w+)>(?<content>.*)</filter:\\k<filterName>>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

		public static readonly Regex ViewComponentTags = new Regex(@"
<component:(?<componentName>\w[\w.]*)" + attributesBlock + @">					# Match first opeing delimiter
  (?<content>
    (?:
        <component:\k<componentName> (?<LEVEL>)								# On opening delimiter push level
      | 
        </component:\k<componentName>> (?<-LEVEL>)								# On closing delimiter pop level
      |
        (?! <component:\k<componentName> | </component:\k<componentName>> ) .  # Match any char unless the opening   
    )*																			# or closing delimiters are in the lookahead string
    (?(LEVEL)(?!))																# If level exists then fail
.*?  )
</component:\k<componentName>>													# Match first closing delimiter
", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

		public static readonly Regex ViewComponentSectionTags = new Regex(
@"<section:(?<sectionName>\w+)>(?<content>.*)</section:\k<sectionName>>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

		public static readonly Regex Script = new Regex(@"
(?<markup>((?!<%).)*)
(?<script><%(?<statement>((?!<%).)*)%>)?", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

		private const string attributesBlock =
@"(?<attributes>(\s*\w+=""[<][%]=\s*[\w\.\(\)\[\]""]+\s*[%][>]""|\s*\w+=""[^""]*""|\s*)*)";


		public static readonly Regex EmbededServerScriptBlock = new Regex(
@"<script\s+runat\s*=\s*\""server\""\s*>(?<content>((?!</script>).)*)</script>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

		public static readonly Regex InlineOutputDirective = new Regex(
@"\${(?<content>[\s\w\.\(\)\[\]""]+)}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public static readonly Regex ServerSideComment = new Regex(
@"<%--(?<content>((?!--%>).)*)--%>", RegexOptions.Compiled | RegexOptions.Singleline);

		public static readonly Regex LayoutContentPlaceHolder = new Regex(@"<asp:contentplaceholder(\s)*" + attributesBlock + @"(/>|>(\s)*</asp:contentplaceholder>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public static readonly Regex ContentTag = new Regex(@"<asp:content(\s)+" + attributesBlock + @"(\s)*>(?<content>((?!</asp:content>)(\s|.))*)</asp:content>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
	}

}
