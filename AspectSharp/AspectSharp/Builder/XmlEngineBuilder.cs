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

namespace AspectSharp.Builder
{
	using System;
	using System.IO;
	using System.Xml;
	using AspectSharp.Lang;
	using AspectSharp.Lang.AST;

	/// <summary>
	/// Summary description for XmlEngineBuilder.
	/// </summary>
	public class XmlEngineBuilder : AspectLanguageEngineBuilder
	{
		private XmlNode _node;

		/// <summary>
		/// Should be used cautiously by subclasses.
		/// </summary>
		protected XmlEngineBuilder()
		{
		}

		public XmlEngineBuilder(String xmlContents)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlContents);
			_node = doc.DocumentElement;
		}

		public XmlEngineBuilder(XmlNode node)
		{
			_node = node;
		}

		protected XmlNode Node
		{
			get { return _node; }
			set { _node = value; }
		}

		public override AspectEngine Build()
		{
			if (!_node.Name.Equals("configuration"))
			{
				throw new BuilderException("Invalid root node. Expecting 'configuration'");
			}

			XmlNode contentNode = _node;

			if (contentNode.FirstChild.NodeType == XmlNodeType.Whitespace ||
				contentNode.FirstChild.NodeType == XmlNodeType.Text)
			{
				base.Reader = new StringReader(contentNode.InnerText);
				return base.Build();
			}
			else if (contentNode.FirstChild.NodeType == XmlNodeType.CDATA)
			{
				// CData node containing language configuration
				// Parse it 

				base.Reader = new StringReader(contentNode.FirstChild.Value);
				return base.Build();
			}

			Configuration = new EngineConfiguration();

			LoadImports();
			LoadGlobals();
			LoadAspects();

			ExecuteSteps();

			return new AspectEngine(Configuration);
		}

		private void LoadAspects()
		{
			XmlNodeList aspects = _node.SelectNodes("aspect");
			foreach (XmlNode node in aspects)
			{
				String name = GetRequiredAttribute(node, "name");
				AspectDefinition aspect = new AspectDefinition(LexicalInfo.Empty, name);
				Configuration.Aspects.Add(aspect);

				XmlNode singleType = node.SelectSingleNode("for/singletype");
				aspect.TargetType = new TargetTypeDefinition();
				aspect.TargetType.SingleType = CreateTypeReference(singleType);

				XmlNodeList mixins = node.SelectNodes("mixin");
				foreach (XmlNode inner in mixins)
				{
					MixinDefinition def = new MixinDefinition(LexicalInfo.Empty);
					def.TypeReference = CreateTypeReference(inner);
					aspect.Mixins.Add(def);
				}

				XmlNodeList pointcuts = node.SelectNodes("pointcut");
				foreach (XmlNode inner in pointcuts)
				{
					PointCutDefinition def = CreatePointCutDefinition(inner);
					aspect.PointCuts.Add(def);
				}
			}
		}

		private PointCutDefinition CreatePointCutDefinition(XmlNode inner)
		{
			String flags = GetRequiredAttribute(inner, "symbol");
			PointCutFlags pcflags = (PointCutFlags) Enum.Parse(typeof (PointCutFlags), flags);
			PointCutDefinition def = new PointCutDefinition(LexicalInfo.Empty, pcflags);

			ParseSignature(inner, def);
			LoadAdvices(inner, def);

			return def;
		}

		private void LoadAdvices(XmlNode inner, PointCutDefinition def)
		{
			XmlNodeList advices = inner.SelectNodes("interceptor");
			foreach (XmlNode advice in advices)
			{
				InterceptorDefinition inter = new InterceptorDefinition(LexicalInfo.Empty);
				inter.TypeReference = CreateTypeReference(advice);
				def.Advices.Add(inter);
			}
		}

		private void ParseSignature(XmlNode inner, PointCutDefinition def)
		{
			XmlNode signature = inner.SelectSingleNode("signature");
			StringReader reader = new StringReader(signature.InnerText);
			AspectLanguageLexer lexer = new AspectLanguageLexer(reader);
			AspectParser parser = new AspectParser(lexer);
			parser.ParsePointcutSignature(def);
		}

		private void LoadGlobals()
		{
			LoadMixins();
			LoadInterceptors();
		}

		private void LoadInterceptors()
		{
			XmlNodeList interceptors = _node.SelectNodes("interceptors/interceptor");
			foreach (XmlNode node in interceptors)
			{
				String key = GetRequiredAttribute(node, "key");
				InterceptorEntryDefinition inter = new InterceptorEntryDefinition(key, LexicalInfo.Empty);
				inter.TypeReference = CreateTypeReference(node);

				Configuration.Interceptors.Add(inter);
			}
		}

		private void LoadMixins()
		{
			XmlNodeList mixins = _node.SelectNodes("mixins/mixin");
			foreach (XmlNode node in mixins)
			{
				String key = GetRequiredAttribute(node, "key");
				MixinEntryDefinition mixin = new MixinEntryDefinition(key, LexicalInfo.Empty);
				mixin.TypeReference = CreateTypeReference(node);

				Configuration.Mixins.Add(mixin);
			}
		}

		private void LoadImports()
		{
			XmlNodeList imports = _node.SelectNodes("import");
			foreach (XmlNode node in imports)
			{
				String ns = GetRequiredAttribute(node, "namespace");
				ImportDirective import = new ImportDirective(LexicalInfo.Empty, ns);
				import.AssemblyReference = CreateAssemblyReference(node);

				Configuration.Imports.Add(import);
			}
		}

		private TypeReference CreateTypeReference(XmlNode node)
		{
			String type = GetRequiredAttribute(node, "type");
			String refTypeEnum = GetRequiredAttribute(node, "refTypeEnum");
			TargetTypeEnum typeEnum = (TargetTypeEnum) Enum.Parse(typeof (TargetTypeEnum), refTypeEnum);

			return new TypeReference(LexicalInfo.Empty, type, typeEnum);
		}

		private AssemblyReference CreateAssemblyReference(XmlNode node)
		{
			String assemblyName = GetAttribute(node, "assembly", null);

			if (assemblyName != null)
			{
				return new AssemblyReference(LexicalInfo.Empty, assemblyName);
			}

			return null;
		}

		private String GetRequiredAttribute(XmlNode node, String key)
		{
			String value = GetAttribute(node, key, null);
			if (value == null)
			{
				throw new BuilderException("Error parsing contents. XmlNode " + node.Name + "requires an attribute named " + key);
			}
			return value;
		}

		private String GetAttribute(XmlNode node, String key, String defaultValue)
		{
			XmlAttribute att = node.Attributes[key];
			if (att == null)
			{
				return defaultValue;
			}
			return att.Value;
		}
	}
}