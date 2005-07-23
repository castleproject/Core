// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Common.TemplateEngine
{
	using System;
	using System.Collections;
	using System.IO;

	/// <summary>
	/// Abstracts the underlying template engine being
	/// used.
	/// </summary>
	public interface ITemplateEngine
	{
		/// <summary>
		/// Implementors should process the template with
		/// data from the context.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="templateName"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		bool Process(IDictionary context, String templateName, TextWriter output);

		/// <summary>
		/// Implementors should return <c>true</c> only if the 
		/// specified template exists and can be used
		/// </summary>
		/// <param name="templateName"></param>
		/// <returns></returns>
		bool HasTemplate(String templateName);
	}
}
