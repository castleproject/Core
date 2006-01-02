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

namespace Castle.MonoRail.Generator.Generators
{
	using System;
	using System.IO;
	using System.Text;
	using System.Collections;

	using Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine;

	/// <summary>
	/// Base Generator only set up the template engine
	/// </summary>
	public abstract class AbstractGenerator
	{
		private static readonly String ResourcePrefix = "Castle.MonoRail.Generator.templates.";

		protected NVelocityTemplateEngine engine;
				
		public AbstractGenerator()
		{
			engine = new NVelocityTemplateEngine();

			engine.AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().FullName;
			
			engine.BeginInit();
			engine.EndInit();
		}

		protected void WriteTemplateFile(String filename, Hashtable ctx, String templateName)
		{
			using (StreamWriter swriter = new StreamWriter(filename, false, Encoding.Default))
			{
				engine.Process(ctx, ResourcePrefix + templateName, swriter);
			}
		}
	}
}