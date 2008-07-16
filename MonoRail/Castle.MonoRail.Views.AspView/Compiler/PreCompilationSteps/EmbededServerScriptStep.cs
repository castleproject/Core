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
	using System.Text.RegularExpressions;

	/// <summary>
	/// This step process &lt;script runat="server"&gt; tags, used to embed
	/// private methods right into the generated class.
	/// </summary>
	public class EmbededServerScriptStep : IPreCompilationStep 
	{
		#region IPreCompilationStep Members

		/// <summary>
		/// Would remove &lt;script runat="server"&gt; tags from the file's body,
		/// and add them to a list, later to be added to the generated class.
		/// </summary>
		/// <param name="file">The source file object to act upon.</param>
		void IPreCompilationStep.Process(SourceFile file) 
		{
			file.RenderBody = Internal.RegularExpressions.EmbededServerScriptBlock.Replace(
				file.RenderBody,
				delegate(Match match) 
				{
					string scriptcontents = match.Groups["content"].Value;
					file.EmbededScriptBlocks.Add(scriptcontents);
					return string.Empty;
				}
			);
			
		}

		#endregion
	}
}
