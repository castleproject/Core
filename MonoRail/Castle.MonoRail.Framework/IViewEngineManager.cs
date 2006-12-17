// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.IO;
	
	/// <summary>
	/// Sits between the controller and the view engines (multiples)
	/// to decide which view engine should render a specific content
	/// </summary>
	public interface IViewEngineManager
	{
		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <returns><c>true</c> if it exists</returns>
		bool HasTemplate(String templateName);

		/// <summary>
		/// Processes the view - using the templateName 
		/// to obtain the correct template,
		/// and using the context to output the result.
		/// </summary>
		void Process(IRailsEngineContext context, Controller controller, String templateName);

		///<summary>
		/// Processes the view - using the templateName 
		/// to obtain the correct template
		/// and writes the results to the System.TextWriter. 
		/// No layout is applied!
		/// </summary>
		void Process(TextWriter output, IRailsEngineContext context, Controller controller, String templateName);

		void ProcessPartial(TextWriter output, IRailsEngineContext context, Controller controller, String partialName);

		/// <summary>
		/// Wraps the specified content in the layout using 
		/// the context to output the result.
		/// </summary>
		void ProcessContents(IRailsEngineContext context, Controller controller, String contents);
	}
}
