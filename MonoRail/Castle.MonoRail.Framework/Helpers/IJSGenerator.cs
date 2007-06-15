// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
	using System.Text;

	public interface IJSGenerator
	{
		/// <summary>
		/// TODO: Implement and document this one
		/// <para>
		/// Top, After, Before, Bottom
		/// </para>
		/// </summary>
		/// <param name="position"></param>
		/// <param name="id"></param>
		/// <param name="renderOptions"></param>
		void InsertHtml(string position, string id, object renderOptions);

		/// <summary>
		/// TODO: Implement and document this one
		/// </summary>
		/// <param name="id"></param>
		/// <param name="renderOptions"></param>
		void ReplaceHtml(String id, object renderOptions);

		/// <summary>
		/// TODO: Implement and document this one
		/// </summary>
		/// <param name="id"></param>
		/// <param name="renderOptions"></param>
		void Replace(String id, object renderOptions);

		/// <summary>
		/// TODO: Implement and document this one
		/// </summary>
		/// <param name="ids"></param>
		void Show(params string[] ids);

		/// <summary>
		/// TODO: Implement and document this one
		/// </summary>
		/// <param name="ids"></param>
		void Hide(params string[] ids);

		/// <summary>
		/// TODO: Implement and document this one
		/// </summary>
		/// <param name="ids"></param>
		void Toggle(params string[] ids);

		/// <summary>
		/// TODO: Implement and document this one
		/// </summary>
		/// <param name="ids"></param>
		void Remove(params string[] ids);

		/// <summary>
		/// TODO: Implement and document this one
		/// </summary>
		/// <param name="message"></param>
		void Alert(object message);

		/// <summary>
		/// TODO: Implement and document this one
		/// </summary>
		/// <param name="url"></param>
		void RedirectTo(object url);

		/// <summary>
		/// Re-apply Behaviour css' rules.
		/// </summary>
		void ReApply();

		/// <summary>
		/// TODO: Implement and document this one
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="element">The element.</param>
		/// <param name="options">The options.</param>
		void VisualEffect(String name, String element, IDictionary options);

		/// <summary>
		/// TODO: Implement and document this one
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="options">The options.</param>
		void VisualEffectDropOut(String element, IDictionary options);

		/// <summary>
		/// TODO: Implement and document this one
		/// </summary>
		/// <param name="variable"></param>
		/// <param name="expression"></param>
		void Assign(String variable, String expression);


		/// <summary>
		/// Declares the specified variable as null.
		/// </summary>
		/// <param name="variable">The variable name.</param>
		void Declare(String variable);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="function"></param>
		/// <param name="args"></param>
		void Call(object function, params object[] args);

		/// <summary>
		/// Renders the specified render options.
		/// <para>
		/// If the renderOptions is a string, the content is escaped and quoted.
		/// </para>
		/// <para>
		/// If the renderOptions is a dictionary, we extract the key <c>partial</c>
		/// and evaluate the template it points to. The content is escaped and quoted.
		/// </para>
		/// </summary>
		/// <param name="renderOptions">The render options.</param>
		/// <returns></returns>
		object Render(object renderOptions);

		/// <summary>
		/// Writes the content specified to the generator instance
		/// </summary>
		/// <param name="content">The content.</param>
		void Write(String content);

		/// <summary>
		/// Writes the content specified to the generator instance
		/// </summary>
		/// <param name="content">The content.</param>
		void AppendLine(String content);

		string ToString();

		StringBuilder Lines { get; }

		IJSCollectionGenerator CreateCollectionGenerator(string root);

		IJSElementGenerator CreateElementGenerator(string root);
	}
}