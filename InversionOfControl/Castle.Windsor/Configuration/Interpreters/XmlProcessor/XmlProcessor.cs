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

namespace Castle.Windsor.Configuration.Interpreters.XmlProcessor
{
	using System;
	using System.Xml;

	using Castle.MicroKernel.SubSystems.Resource;
	using Castle.Model.Resource;

	using ElementProcessors;

	/// <summary>
	/// Pendent
	/// </summary>
	public class XmlProcessor
	{
		private IXmlProcessorEngine engine;

		public XmlProcessor(IResourceSubSystem resourceSubSystem)
		{
			engine = new DefaultXmlProcessorEngine(resourceSubSystem);
			RegisterProcessors();
		}

		public XmlProcessor()
		{
			engine = new DefaultXmlProcessorEngine();
			RegisterProcessors();
		}

		protected virtual void RegisterProcessors()
		{
			AddElementProcessor(typeof(IfElementProcessor));
			AddElementProcessor(typeof(DefineElementProcessor));
			AddElementProcessor(typeof(UndefElementProcessor));
			AddElementProcessor(typeof(ChooseElementProcessor));
			AddElementProcessor(typeof(PropertiesElementProcessor));
			AddElementProcessor(typeof(AttributesElementProcessor));
			AddElementProcessor(typeof(IncludeElementProcessor));
			AddElementProcessor(typeof(IfProcessingInstructionProcessor));
			AddElementProcessor(typeof(DefinedProcessingInstructionProcessor));
			AddElementProcessor(typeof(UndefProcessingInstructionProcessor));
			AddElementProcessor(typeof(DefaultTextNodeProcessor));
		}

		protected void AddElementProcessor(Type t)
		{
			engine.AddNodeProcessor(t);
		}

		public XmlNode Process(XmlNode node)
		{
			if (node.NodeType == XmlNodeType.Document)
			{
				node = (node as XmlDocument).DocumentElement;
			}

			engine.DispatchProcessAll(new DefaultXmlProcessorNodeList(node));

			return node;
		}

		public XmlNode Process(IResource resource)
		{
			using(resource)
			{
				XmlDocument doc = new XmlDocument();

				doc.Load(resource.GetStreamReader());

				engine.PushResource(resource);

				XmlNode element = Process(doc.DocumentElement);

				engine.PopResource();

				return element;
			}
		}
	}
}