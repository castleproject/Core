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
	using System.Text;
	using System.Xml;
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Represents an evaluation context
	/// </summary>
	public interface ITemplateContext
	{
		/// <summary>
		/// Gets the template xml reader.
		/// </summary>
		/// <value>The reader.</value>
		XmlReader Reader { get; }

		/// <summary>
		/// Gets the service provider.
		/// </summary>
		/// <value>The service provider.</value>
		IServiceProvider ServiceProvider { get; }

		/// <summary>
		/// Gets the template engine.
		/// </summary>
		/// <value>The template engine.</value>
		ITemplateEngine Engine { get; }

		/// <summary>
		/// Gets the name of the root view.
		/// </summary>
		/// <value>The name of the root view.</value>
		String RootViewName { get; }

		/// <summary>
		/// Gets the script buffer.
		/// </summary>
		/// <value>The script.</value>
		StringBuilder Script { get; }

		/// <summary>
		/// Gets the indentation level.
		/// </summary>
		/// <value>The indentation.</value>
		int Indentation { get; }

		/// <summary>
		/// Gets the current element depth.
		/// </summary>
		/// <value>The current element depth.</value>
		int CurrentElementDepth { get; }

		void IncreaseIndentation(); 
		
		void DecreaseIndentation();

		void IncreaseDepth();

		void DecreaseDepth();

		void AppendIndented(string content);
		
		void AppendLineIndented(string content);
	}
}
