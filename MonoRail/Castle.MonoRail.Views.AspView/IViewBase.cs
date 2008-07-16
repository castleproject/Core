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

namespace Castle.MonoRail.Views.AspView
{
	using System.Collections;
	using System.IO;
	using Framework;

	public interface IViewBase
	{
		/// <summary>
		/// Gets the output writer to which the view is rendered.
		/// </summary>
		TextWriter OutputWriter { get; }

		/// <summary>
		/// Processes the view.
		/// Will first render a ContentView if present (on layouts), to the ViewContents property.
		/// </summary>
		void Process();

		/// <summary>
		/// (For layouts) - gets the ContentView's contents.
		/// </summary>
		string ViewContents { get; }

		/// <summary>
		/// Gets the properties container. Based on current property containers that was sent from the controller, such us PropertyBag, Flash, etc.
		/// </summary>
		IDictionary Properties { get; }

		/// <summary>
		/// Gets the current Rails context.
		/// </summary>
		IEngineContext Context { get; }

		/// <summary>
		/// Gets the controller which have asked for this view to be rendered.
		/// </summary>
		IController Controller { get; }

		/// <summary>
		/// (For subviews) - gets a reference to the view's parent view.
		/// </summary>
		IViewBase ParentView { get; }

		/// <summary>
		/// Would initialize a view instance, prepearing it to be processed.
		/// </summary>
		/// <param name="viewEngine">The view engine.</param>
		/// <param name="output">The writer to which the view would be rendered.</param>
		/// <param name="context">The rails engine content.</param>
		/// <param name="controller">The controller which have asked for this view to be rendered.</param>
		/// <param name="controllerContext">The controller's context.</param>
		void Initialize(AspViewEngine viewEngine, TextWriter output, IEngineContext context,
						IController controller, IControllerContext controllerContext);

		/// <summary>
		/// When overriden in a concrete view class, renders the view content to the output writer.
		/// </summary>
		void Render();

	}
}