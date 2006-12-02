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

namespace Castle.MonoRail.Views.IronView
{
	using System;
	using System.Xml;

	/// <summary>
	/// Allows implementation of custom processor
	/// to be invoked by the parser
	/// </summary>
	public interface IElementProcessor
	{
		/// <summary>
		/// Implementors should return true if they 
		/// can handle the Processing instruction
		/// </summary>
		/// <param name="name">instruction's name</param>
		/// <param name="value">instruction's value</param>
		/// <returns><c>true</c> if the processor handles it</returns>
		bool CanHandlePI(String name, String value);

		/// <summary>
		/// Implementors should return true if they 
		/// can handle the element. 
		/// </summary>
		/// <param name="parser">parser instance</param>
		/// <param name="reader">XmlReader instance</param>
		/// <returns><c>true</c> if the processor handles it</returns>
		bool CanHandleElement(ITemplateParser parser, XmlReader reader);

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="context"></param>
		void ProcessPI(ITemplateParser parser, ITemplateContext context);

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="context"></param>
		void ProcessElement(ITemplateParser parser, ITemplateContext context);
	}
}
