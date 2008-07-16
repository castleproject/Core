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
	/// <summary>
	/// Strips any server side comment (&lt;%-- --%&gt;) found in the view template
	/// </summary>
	public class ServerSideCommentStripperStep : IPreCompilationStep 
	{
		#region IPreCompilationStep Members

		/// <summary>
		/// Performs server side comment stripping
		/// </summary>
		/// <param name="file">The source file object to act upon.</param>
		void IPreCompilationStep.Process(SourceFile file) 
		{
			file.RenderBody = Internal.RegularExpressions.ServerSideComment.Replace(
				file.RenderBody,
				string.Empty
			);
			
		}

		#endregion
	}
}
