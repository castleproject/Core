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
	/// A unit that processes an AspView SourceFile, to make it ready for compilation
	/// </summary>
	public interface IPreCompilationStep
	{
		/// <summary>
		/// Acts upon a source file
		/// </summary>
		/// <param name="file">The source file to process</param>
		void Process(SourceFile file);
	}
}
