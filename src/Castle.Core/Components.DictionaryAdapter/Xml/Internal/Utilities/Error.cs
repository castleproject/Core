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
	using SerializationException = System.Runtime.Serialization.SerializationException;
	using System.Xml.XPath;

	internal static class Error
	{
		internal static Exception ArgumentNull(string paramName)
		{
			return new ArgumentNullException(paramName);
		}

		internal static Exception ArgumentOutOfRange(string paramName)
		{
			return new ArgumentOutOfRangeException(paramName);
		}

		internal static Exception InvalidOperation()
		{
			return new InvalidOperationException();
		}

		internal static Exception NotSupported()
		{
			return new NotSupportedException();
		}

		internal static Exception ObjectDisposed(string objectName)
		{
			return new ObjectDisposedException(objectName);
		}

		internal static Exception AttributeConflict(string propertyName)
		{
			var message = string.Format
			(
				"The behaviors defined for property '{0}' are ambiguous or conflicting.",
				propertyName
			);
			return new InvalidOperationException(message);
		}

		internal static Exception SeparateGetterSetterOnComplexType(string propertyName)
		{
			var message = string.Format
			(
				"Cannot apply getter/setter behaviors for property '{0}'.  Separate getters/setters are supported for simple types only.",
				propertyName
			);
			return new InvalidOperationException(message);
		}

		internal static Exception XmlMetadataNotAvailable(Type clrType)
		{
			var message = string.Format
			(
				"XML metadata is not available for type '{0}'.",
				clrType.FullName
			);
			return new InvalidOperationException(message);
		}

		internal static Exception NotDictionaryAdapter(string paramName)
		{
			var message = "The argument is not a dictionary adapter.";
			return new ArgumentException(message, paramName);
		}

		internal static Exception NoInstanceDescriptor(string paramName)
		{
			var message = "The dictionary adapter does not have an instance descriptor.";
			return new ArgumentException(message, paramName);
		}

		internal static Exception NoXmlAdapter(string paramName)
		{
			var message = "The dictionary adapter does not have XmlAdapter behavior.";
			return new ArgumentException(message, paramName);
		}

		internal static Exception NotRealizable<T>()
		{
			var message = string.Format(
				"The given node cannot provide an underlying object of type {0}.",
				typeof(T).FullName);
			return new NotSupportedException(message);
		}

		internal static Exception CursorNotMutable()
		{
			var message = "The cursor does not support creation, removal, or modification of nodes.";
			return new NotSupportedException(message);
		}

		internal static Exception CursorNotInCreatableState()
		{
			var message = "The cursor cannot create nodes in its current state.";
			return new InvalidOperationException(message);
		}

		internal static Exception CursorNotInRemovableState()
		{
			var message = "The cursor cannot remove nodes in its current state.";
			return new InvalidOperationException(message);
		}

		internal static Exception CursorNotInCoercibleState()
		{
			var message = "The cursor cannot change node types in its current state.";
			return new InvalidOperationException(message);
		}

		internal static Exception CursorNotInRealizableState()
		{
			var message = "The cursor cannot realize virtual nodes in its current state";
			return new InvalidOperationException(message);
		}

		internal static Exception CursorCannotMoveToGivenNode()
		{
			var message = "The cursor cannot move to the given node.";
			return new InvalidOperationException(message);
		}

		internal static Exception CannotSetAttribute(IXmlIdentity identity)
		{
			var message = string.Format
			(
				"Cannot set attribute on node '{0}'.",
				identity.Name.ToString()
			);
			return new InvalidOperationException(message);
		}

		internal static Exception NotXmlKnownType(Type clrType)
		{
			var message = string.Format
			(
				"No XML type is defined for CLR type {0}.",
				clrType.FullName
			);
			return new SerializationException(message);
		}

		internal static Exception UnsupportedCollectionType(Type clrType)
		{
			var message = string.Format
			(
				"Unsupported collection type: {0}.",
				clrType.FullName
			);
			return new SerializationException(message);
		}

		internal static Exception NotCollectionType(string paramName)
		{
			var message = "The argument is not a valid collection type.";
			return new ArgumentException(message, paramName);
		}


		internal static Exception InvalidLocalName()
		{
			var message = "Invalid local name.";
			return new FormatException(message);
		}

		internal static Exception InvalidNamespaceUri()
		{
			var message = "Invalid namespace URI.";
			return new FormatException(message);
		}

		internal static Exception NoDefaultKnownType()
		{
			var message = "No default XML type exists in the given context.";
			return new InvalidOperationException(message);
		}

		internal static Exception XPathNotCreatable(CompiledXPath path)
		{
			var message = string.Format(
				"The path '{0}' is not a creatable XPath expression.",
				path.Path.Expression);
			return new XPathException(message);
		}

		internal static Exception XPathNavigationFailed(XPathExpression path)
		{
			var message = string.Format(
				"Failed navigation to {0} element after creation.",
				path.Expression);
			return new XPathException(message);
		}

		internal static Exception ObjectIdNotFound(string id)
		{
			var message = string.Format(
				"No object with ID '{0}' was present in the XML.",
				id);
			return new SerializationException(message);
		}
	}
}
#endif
