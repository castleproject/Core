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

using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.Runtime;
using Commons.Collections;

namespace Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine
{
	using System;
	using System.Collections;
	using System.IO;
	using System.ComponentModel;

	using Castle.Components.Common.TemplateEngine;

	/// <summary>
	/// 
	/// </summary>
	public class NVelocityTemplateEngine : ITemplateEngine, ISupportInitialize
	{
		private String _templateDir;

		public NVelocityTemplateEngine(String templateDir)
		{
			_templateDir = templateDir;
		}

		public void BeginInit()
		{
			ExtendedProperties props = new ExtendedProperties();

			props.SetProperty(RuntimeConstants_Fields.RESOURCE_MANAGER_CLASS, "NVelocity.Runtime.Resource.ResourceManagerImpl\\,NVelocity");
			props.SetProperty(RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, _templateDir);

			Velocity.Init(props);
		}

		public void EndInit()
		{
		}

		/// <summary>
		/// Implementors should process the template with
		/// data from the context.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="templateName"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		public bool Process(IDictionary context, String templateName, TextWriter output)
		{
			Template template = RuntimeSingleton.getTemplate(templateName);

			template.Merge(CreateContext(context), output);

			return true;
		}

		private IContext CreateContext(IDictionary context)
		{
			return new VelocityContext( new Hashtable(context) );
		}
	}
}
