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

namespace AspectSharp.Lang.AST.Visitors
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Summary description for PrintTreeVisitor.
	/// </summary>
	public class XmlTreeVisitor : DepthFirstVisitor
	{
		private XmlDocument _document;
		private Stack _nodes = new Stack();

		public XmlTreeVisitor()
		{
			_document = new XmlDocument();
		}

		private void Push( XmlNode node )
		{
			if (Current != null)
			{
				Current.AppendChild( node );
			}
			else
			{
				_document.AppendChild(node);
			}

			_nodes.Push(node);
		}

		private void Pop()
		{
			XmlNode node = (XmlNode) _nodes.Pop();
		}

		private XmlNode Current
		{
			get { return _nodes.Count != 0 ? _nodes.Peek() as XmlNode : _document.DocumentElement; }
		}

		public XmlDocument Document
		{
			get { return _document; }
		}

		public override void OnEngineConfiguration(EngineConfiguration conf)
		{
			Push( Document.CreateNode(XmlNodeType.Element, "configuration", null) );
			base.OnEngineConfiguration (conf);
			Pop();
		}

		public override void OnGlobalInterceptorDeclaration(NodeCollectionBase dict)
		{
			if (dict.Count == 0) return;

			Push( Document.CreateNode(XmlNodeType.Element, "interceptors", null) );
			base.OnGlobalInterceptorDeclaration(dict);
			Pop();
		}

		public override void OnGlobalMixinDeclaration(NodeCollectionBase dict)
		{
			if (dict.Count == 0) return;

			Push( Document.CreateNode(XmlNodeType.Element, "mixins", null) );
			base.OnGlobalMixinDeclaration(dict);
			Pop();
		}
	
		public override void OnAspectDefinition(AspectDefinition aspect)
		{
			Push( Document.CreateNode(XmlNodeType.Element, "aspect", null) );

			XmlAttribute att = Document.CreateAttribute("name");
			att.Value = aspect.Name;
			Current.Attributes.Append( att );

			Push( Document.CreateNode(XmlNodeType.Element, "for", null) );
			SerializeTargetType( aspect.TargetType );
			Pop();

			base.OnAspectDefinition (aspect);
			Pop();
		}
	
		public override void OnImportDirective(ImportDirective import)
		{
			Push( Document.CreateNode(XmlNodeType.Element, "import", null) );
			
			XmlAttribute att = Document.CreateAttribute("namespace");
			att.Value = import.Namespace;
			Current.Attributes.Append( att );

			SerializeAssemblyReference( import.AssemblyReference );
			
			Pop();
		}
	
		public override void OnInterceptorDefinition(InterceptorDefinition interceptor)
		{
			Push( Document.CreateNode(XmlNodeType.Element, "interceptor", null) );
			SerializeTypeReference(interceptor.TypeReference);
			Pop();
		}
	
		public override void OnMixinDefinition(MixinDefinition mixin)
		{
			Push( Document.CreateNode(XmlNodeType.Element, "mixin", null) );
			SerializeTypeReference(mixin.TypeReference);
			Pop();
		}

		public override void OnInterceptorEntryDefinition(InterceptorEntryDefinition interceptor)
		{
			Push( Document.CreateNode(XmlNodeType.Element, "interceptor", null) );
			XmlAttribute att = Document.CreateAttribute("key");
			att.Value = interceptor.Key;
			Current.Attributes.Append( att );
			SerializeTypeReference(interceptor.TypeReference);
			Pop();
		}
	
		public override void OnMixinEntryDefinition(MixinEntryDefinition mixin)
		{
			Push( Document.CreateNode(XmlNodeType.Element, "mixin", null) );
			XmlAttribute att = Document.CreateAttribute("key");
			att.Value = mixin.Key;
			Current.Attributes.Append( att );
			SerializeTypeReference(mixin.TypeReference);
			Pop();
		}

		public override void OnPointCutDefinition(PointCutDefinition pointcut)
		{
			Push( Document.CreateNode(XmlNodeType.Element, "pointcut", null) );
			
			XmlAttribute att = Document.CreateAttribute("symbol");
			att.Value = pointcut.Flags.ToString();
			Current.Attributes.Append( att );

			Push( Document.CreateNode(XmlNodeType.Element, "signature", null) );
			Current.InnerText = pointcut.Method.ToString();
			Pop();

			base.OnPointCutDefinition (pointcut);
			Pop();
		}

		private void SerializeTargetType( TargetTypeDefinition def )
		{
			Push( Document.CreateNode(XmlNodeType.Element, "singletype", null) );
			SerializeTypeReference( def.SingleType );
			Pop();
		}

		private void SerializeTypeReference(TypeReference def)
		{
			XmlAttribute att = Document.CreateAttribute("type");
			
			if (def.TargetType == TargetTypeEnum.Link)
			{
				att.Value = def.LinkRef;
			}
			else
			{
				att.Value = def.TypeName;
			}
			Current.Attributes.Append( att );

			att = Document.CreateAttribute("refTypeEnum");
			att.Value = def.TargetType.ToString();
			Current.Attributes.Append( att );

			SerializeAssemblyReference(def.AssemblyReference);
		}

		private void SerializeAssemblyReference(AssemblyReference def)
		{
			if (def == null) return;

			XmlAttribute att = Document.CreateAttribute("assembly");
			att.Value = def.AssemblyName;
			Current.Attributes.Append( att );
		}
	}
}
