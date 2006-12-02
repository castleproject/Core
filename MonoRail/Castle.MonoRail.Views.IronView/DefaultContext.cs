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

	class DefaultContext : ITemplateContext
	{
		private readonly string rootViewName;
		private readonly XmlReader reader;
		private readonly IServiceProvider serviceProvider;
		private int currentElementDepth;
		private int indentation;
		private StringBuilder script = new StringBuilder();

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultContext"/> class.
		/// </summary>
		/// <param name="rootViewName">Name of the root view.</param>
		/// <param name="reader">The reader.</param>
		/// <param name="serviceProvider">The service provider.</param>
		public DefaultContext(String rootViewName, XmlReader reader, 
		                      IServiceProvider serviceProvider)
		{
			this.rootViewName = rootViewName;
			this.reader = reader;
			this.serviceProvider = serviceProvider;
		}

		public XmlReader Reader
		{
			get { return reader; }
		}

		public IServiceProvider ServiceProvider
		{
			get { return serviceProvider; }
		}

		public string RootViewName
		{
			get { return rootViewName; }
		}

		public StringBuilder Script
		{
			get { return script; }
		}

		public int Indentation
		{
			get { return indentation; }
		}

		public int CurrentElementDepth
		{
			get { return currentElementDepth; }
		}

		public void IncreaseIndentation()
		{
			indentation++;
		}

		public void DecreaseIndentation()
		{
			indentation--;
		}

		public void IncreaseDepth()
		{
			currentElementDepth++;
		}

		public void DecreaseDepth()
		{
			currentElementDepth--;
		}

		public void AppendIndented(string content)
		{
			Indent();

			script.Append(content);
		}

		public void AppendLineIndented(string content)
		{
			Indent();

			script.AppendLine(content);
		}

		private void Indent()
		{
			for(int i = 0; i < indentation; i++)
			{
				script.Append('\t');
			}
		}
	}
}