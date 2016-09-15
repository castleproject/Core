// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml.XPath;

	public class XPathBehaviorAccessor : XmlAccessor, IXmlIncludedType, IXmlIncludedTypeMap,
		IConfigurable<XPathAttribute>,
		IConfigurable<XPathVariableAttribute>,
		IConfigurable<XPathFunctionAttribute>
	{
	    private CompiledXPath path;
		private XmlIncludedTypeSet includedTypes;
		private XmlAccessor defaultAccessor;
		private XmlAccessor itemAccessor;

		internal static readonly XmlAccessorFactory<XPathBehaviorAccessor>
			Factory = (name, type, context) => new XPathBehaviorAccessor(type, context);

	    protected XPathBehaviorAccessor(Type type, IXmlContext context)
	        : base(type, context)
		{
			includedTypes = new XmlIncludedTypeSet();

			foreach (var includedType in context.GetIncludedTypes(ClrType))
				includedTypes.Add(includedType);
		}

		XmlName IXmlIncludedType.XsiType
		{
			get { return XmlName.Empty; }
		}

		IXmlIncludedType IXmlIncludedTypeMap.Default
		{
			get { return this; }
		}

		private bool SelectsNodes
		{
			get { return path.Path.ReturnType == XPathResultType.NodeSet; }
		}

		private bool CreatesAttributes
		{
			get { var step = path.LastStep; return step != null && step.IsAttribute; }
		}

		public void Configure(XPathAttribute attribute)
		{
			if (path != null)
				throw Error.AttributeConflict(path.Path.Expression);

			path = attribute.SetPath;

			if (path == attribute.GetPath)
				return;
			else if (Serializer.CanGetStub)
				throw Error.SeparateGetterSetterOnComplexType(path.Path.Expression);

			defaultAccessor = new DefaultAccessor(this, attribute.GetPath);
		}

		public void Configure(XPathVariableAttribute attribute)
		{
			CloneContext().AddVariable(attribute);
		}

		public void Configure(XPathFunctionAttribute attribute)
		{
			CloneContext().AddFunction(attribute);
		}

		public override void Prepare()
		{
			if (CreatesAttributes)
				state &= ~States.Nillable;

			Context.Enlist(path);

			if (defaultAccessor != null)
				defaultAccessor.Prepare();
		}

		public override bool IsPropertyDefined(IXmlNode parentNode)
		{
			return SelectsNodes
				&& base.IsPropertyDefined(parentNode);
		}

		public override object GetPropertyValue(IXmlNode parentNode, IDictionaryAdapter parentObject, XmlReferenceManager references, bool orStub)
		{
			return GetPropertyValueCore   (parentNode, parentObject, references, orStub)
				?? GetDefaultPropertyValue(parentNode, parentObject, references, orStub);
		}

		private object GetPropertyValueCore(IXmlNode parentNode, IDictionaryAdapter parentObject, XmlReferenceManager references, bool orStub)
		{
			return SelectsNodes
				? base.GetPropertyValue(parentNode, parentObject, references, orStub)
				: Evaluate(parentNode);
		}

		private object GetDefaultPropertyValue(IXmlNode parentNode, IDictionaryAdapter parentObject, XmlReferenceManager references, bool orStub)
		{
			return defaultAccessor != null
				? defaultAccessor.GetPropertyValue(parentNode, parentObject, references, orStub)
				: null;
		}

		private object Evaluate(IXmlNode node)
		{
			var value = node.Evaluate(path);
			return value != null
				? Convert.ChangeType(value, ClrType)
				: null;
		}

		public override void SetPropertyValue(IXmlNode parentNode, IDictionaryAdapter parentObject, XmlReferenceManager references, object oldValue, ref object value)
		{
			if (SelectsNodes)
				base.SetPropertyValue(parentNode, parentObject, references, oldValue, ref value);
			else
				throw Error.XPathNotCreatable(path);
		}

		public override IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
		{
			return itemAccessor ?? (itemAccessor = new ItemAccessor(this));
		}

		public override IXmlCursor SelectPropertyNode(IXmlNode node, bool create)
		{
			var flags = CursorFlags.AllNodes.MutableIf(create);
			return node.Select(path, this, Context, flags);
		}

		public override IXmlCursor SelectCollectionNode(IXmlNode node, bool create)
		{
			return node.SelectSelf(ClrType);
		}

		public override IXmlCursor SelectCollectionItems(IXmlNode node, bool create)
		{
			var flags = CursorFlags.AllNodes.MutableIf(create) | CursorFlags.Multiple;
			return node.Select(path, this, Context, flags);
		}

		public bool TryGet(XmlName xsiType, out IXmlIncludedType includedType)
		{
			if (xsiType == XmlName.Empty || xsiType == this.XsiType)
				return Try.Success(out includedType, this);

			if (!includedTypes.TryGet(xsiType, out includedType))
				return false;

			if (!ClrType.IsAssignableFrom(includedType.ClrType))
				return Try.Failure(out includedType);

			return true;
		}

		public bool TryGet(Type clrType, out IXmlIncludedType includedType)
		{
			return clrType == this.ClrType
				? Try.Success(out includedType, this)
				: includedTypes.TryGet(clrType, out includedType);
		}

		private class DefaultAccessor : XPathBehaviorAccessor
		{
			private readonly XPathBehaviorAccessor parent;

			public DefaultAccessor(XPathBehaviorAccessor parent, CompiledXPath path)
				: base(parent.ClrType, parent.Context)
			{
				this.parent = parent;
				this.path   = path;
			}

			public override void Prepare()
			{
				this.includedTypes = parent.includedTypes;
				this.Context       = parent.Context;

				base.Prepare();
			}
		}

		private class ItemAccessor : XPathBehaviorAccessor
		{
			public ItemAccessor(XPathBehaviorAccessor parent)
				: base(parent.ClrType.GetCollectionItemType(), parent.Context)
			{
				includedTypes   = parent.includedTypes;
				path            = parent.path;

				ConfigureNillable(true);
			}

			public override IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
			{
				return GetDefaultCollectionAccessor(itemType);
			}
		}
	}
}
#endif
