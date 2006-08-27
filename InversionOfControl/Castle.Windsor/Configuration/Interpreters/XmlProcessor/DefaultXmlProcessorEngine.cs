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

namespace Castle.Windsor.Configuration.Interpreters.XmlProcessor
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Text.RegularExpressions;
	using System.Xml;

	using Castle.MicroKernel.SubSystems.Resource;
	using Castle.Core.Resource;

	using ElementProcessors;

	public class DefaultXmlProcessorEngine : IXmlProcessorEngine
	{
		private readonly Regex flagPattern = new Regex(@"^(\w|_)+$");
		private readonly IDictionary properties = new HybridDictionary();
		private readonly IDictionary flags = new HybridDictionary();
		private readonly Stack resourceStack = new Stack();
		private readonly Hashtable nodeProcessors = new Hashtable();
		private readonly IXmlNodeProcessor defaultElementProcessor;

		private IResourceSubSystem resourseSubSystem;

		public DefaultXmlProcessorEngine() : this(new DefaultResourceSubSystem())
		{
		}

		public DefaultXmlProcessorEngine(IResourceSubSystem resourceSubSystem)
		{
			this.resourseSubSystem = resourceSubSystem;
			defaultElementProcessor = new DefaultElementProcessor();
		}

		public void AddNodeProcessor(Type type)
		{
			if (typeof(IXmlNodeProcessor).IsAssignableFrom(type))
			{
				IXmlNodeProcessor processor = Activator.CreateInstance(type) as IXmlNodeProcessor;

				foreach(XmlNodeType nodeType in processor.AcceptNodeTypes)
				{
					RegisterProcessor(nodeType, processor);
				}
			}
			else
			{
				throw new XmlProcessorException("{0} does not implement IElementProcessor interface", type.FullName);
			}
		}

		/// <summary>
		/// Processes the element.
		/// </summary>
		/// <param name="nodeList">The element.</param>
		/// <returns></returns>
		public void DispatchProcessAll(IXmlProcessorNodeList nodeList)
		{
			while(nodeList.MoveNext())
			{
				DispatchProcessCurrent(nodeList);
			}
		}

		/// <summary>
		/// Processes the element.
		/// </summary>
		/// <param name="nodeList">The element.</param>
		/// <returns></returns>
		public void DispatchProcessCurrent(IXmlProcessorNodeList nodeList)
		{
			IXmlNodeProcessor processor = GetProcessor(nodeList.Current);

			if (processor != null)
			{
				processor.Process(nodeList, this);
			}
		}

		private IXmlNodeProcessor GetProcessor(XmlNode node)
		{
			IXmlNodeProcessor processor = null;
			IDictionary processors = nodeProcessors[node.NodeType] as IDictionary;

			if (processors != null)
			{
				processor = processors[node.Name] as IXmlNodeProcessor;

				// sometime nodes with the same name will not accept a processor
				if (processor == null || !processor.Accept(node))
				{
					if (node.NodeType == XmlNodeType.Element)
					{
						processor = defaultElementProcessor;
					}
				}
			}

			return processor;
		}

		private void RegisterProcessor(XmlNodeType type, IXmlNodeProcessor processor)
		{
			if (!nodeProcessors.Contains(type))
			{
				nodeProcessors[type] = new Hashtable();
			}

			IDictionary typeProcessors = nodeProcessors[type] as IDictionary;

			if (typeProcessors.Contains(processor.Name))
			{
				throw new XmlProcessorException("There is already a processor register for {0} with name {1} ", type, processor.Name);
			}
			else
			{
				typeProcessors.Add(processor.Name, processor);
			}
		}

		public bool HasFlag(string flag)
		{
			return flags.Contains(GetCanonicalFlagName(flag));
		}

		public void AddFlag(string flag)
		{
			flags[GetCanonicalFlagName(flag)] = true;
		}

		public void RemoveFlag(string flag)
		{
			flags.Remove(GetCanonicalFlagName(flag));
		}

		public void PushResource(IResource resource)
		{
			resourceStack.Push(resource);
		}

		public void PopResource()
		{
			resourceStack.Pop();
		}

		public bool HasSpecialProcessor( XmlNode node )
		{
			return GetProcessor(node) != defaultElementProcessor;
		}

		public IResource GetResource(String uri)
		{
			IResource resource = resourceStack.Count > 0 ? resourceStack.Peek() as IResource : null;

			if (uri.IndexOf(Uri.SchemeDelimiter) != -1)
			{
				return resource == null ? resourseSubSystem.CreateResource(uri) :
					resourseSubSystem.CreateResource(uri, resource.FileBasePath);
			}
			else if (resourceStack.Count > 0)
			{
				return resource.CreateRelative(uri);
			}
			else
			{
				throw new XmlProcessorException("Cannot get relative resource '" + uri + "', resource stack is empty");
			}
		}

		public void AddProperty(XmlElement content)
		{
			properties[content.Name] = content;
		}

		public bool HasProperty(String name)
		{
			return properties.Contains(name);
		}

		public XmlElement GetProperty(string key)
		{
			XmlElement prop = properties[key] as XmlElement;

			return prop == null ? null : prop.CloneNode(true) as XmlElement;
		}

		private string GetCanonicalFlagName(string flag)
		{
			flag = flag.Trim().ToLower();

			if (!flagPattern.IsMatch(flag))
			{
				throw new XmlProcessorException("Invalid flag name '{0}'", flag);
			}

			return flag;
		}
	}
}
