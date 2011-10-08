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

#if !SL3
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml.XPath;

	public class XmlXPathBehaviorAccessor : XmlAccessor, IXmlIncludedType, IXmlIncludedTypeMap,
		IConfigurable<XPathAttribute>,
		IConfigurable<XPathVariableAttribute>,
		IConfigurable<XPathFunctionAttribute>
	{
		private XmlAccessor itemAccessor;
		private XmlIncludedTypeSet includedTypes;
	    private CompiledXPath getPath;
	    private CompiledXPath setPath;

		internal static readonly XmlAccessorFactory<XmlXPathBehaviorAccessor>
			Factory = (name, type, context) => new XmlXPathBehaviorAccessor(type, context);

	    protected XmlXPathBehaviorAccessor(Type type, IXmlContext context)
	        : base(type, context)
		{
			includedTypes = new XmlIncludedTypeSet();

			foreach (var includedType in context.GetIncludedTypes(type))
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

		public void Configure(XPathAttribute attribute)
		{
			if (getPath != null)
				throw Error.AttributeConflict(getPath.Path.Expression);

			getPath = attribute.GetPath;
			setPath = attribute.SetPath;

			if (getPath != setPath && Serializer.Kind != XmlTypeKind.Simple)
				throw Error.SeparateGetterSetterOnComplexType(getPath.Path.Expression);
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
			Context.Enlist(getPath);

			if (getPath != setPath)
				Context.Enlist(setPath);
		}

		public override bool IsPropertyDefined(IXmlNode parentNode)
		{
			return SelectsNodes(getPath)
				&& base.IsPropertyDefined(parentNode);
		}

		public override object GetPropertyValue(IXmlNode node, IDictionaryAdapter da, bool ifExists)
		{
			return SelectsNodes(getPath)
				? base.GetPropertyValue(node, da, ifExists)
				: Evaluate(node);
		}

		public override void SetPropertyValue(IXmlNode node, IDictionaryAdapter da, ref object value)
		{
			if (SelectsNodes(setPath))
				base.SetPropertyValue(node, da, ref value);
			else
				throw Error.XPathNotCreatable(setPath);
		}

		private object Evaluate(IXmlNode node)
		{
			var value = node.Evaluate(getPath);
			return Convert.ChangeType(value, ClrType);
		}

		private static bool SelectsNodes(CompiledXPath path)
		{
			return path.Path.ReturnType == XPathResultType.NodeSet;
		}

		public override IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
		{
			return itemAccessor ?? (itemAccessor = new ItemAccessor(this));
		}

		public override IXmlCursor SelectPropertyNode(IXmlNode node, bool create)
		{
			var flags = CursorFlags.AllNodes.MutableIf(create);
			var path  = create ? setPath : getPath;
			return node.Select(path, this, Context, flags);
		}

		public override IXmlCursor SelectCollectionNode(IXmlNode node, bool create)
		{
			return node.SelectSelf(ClrType);
		}

		public override IXmlCursor SelectCollectionItems(IXmlNode node, bool create)
		{
			var flags = CursorFlags.AllNodes.MutableIf(create) | CursorFlags.Multiple;
			var path  = create ? setPath : getPath;
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

		private class ItemAccessor : XmlXPathBehaviorAccessor
		{
			public ItemAccessor(XmlXPathBehaviorAccessor parent)
				: base(parent.ClrType.GetCollectionItemType(), parent.Context)
			{
				getPath       = parent.getPath;
				setPath       = parent.setPath;
				includedTypes = parent.includedTypes;

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
