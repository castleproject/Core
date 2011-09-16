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

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	internal static class Error
	{
		internal static Exception ArgumentNull(string name)
		{
			return new ArgumentNullException(name);
		}

		internal static Exception ArgumentNotDictionaryAdapter(string name)
		{
			return new ArgumentException("The argument is not a dictionary adapter.", name);
		}

		internal static Exception NotSupported()
		{
			return new NotSupportedException();
		}

		internal static Exception AttributeConflict(PropertyDescriptor property)
		{
			throw new NotImplementedException();
		}

		internal static Exception NoXmlMetadata(Type type)
		{
			throw new NotImplementedException();
		}

		internal static Exception NoInstanceDescriptor()
		{
			return new InvalidOperationException("The dictionary adapter does not have an instance descriptor."); // TODO: What to do here?
		}

		internal static Exception NoGetterOnInstanceDescriptor()
		{
			return new InvalidOperationException("The dictionary adapter instance descriptor does not have a getter."); // TODO: What to do here?
		}

		internal static Exception NoXmlAdapter()
		{
			return new InvalidOperationException("The dictionary adapter does not have XmlAdapter behavior.");
		}

		internal static Exception IteratorNotInCreatableState()
		{
			return new InvalidOperationException();
		}

		internal static Exception IteratorNotInRemovableState()
		{
			return new InvalidOperationException();
		}

		internal static Exception MultipleAttributesNotSupported()
		{
			return new NotSupportedException();
		}

		internal static Exception SelectedMultipleNodes()
		{
			return new InvalidOperationException();
				//var message = string.Format(
				//    "The path '{0}' selected multiple nodes, but only one was expected.",
				//    path.Expression);
			//new XmlTransformException(message);
		}

		internal static Exception NoCurrentItem()
		{
			return new InvalidOperationException();
		}

		internal static Exception IteratorNotMutable()
		{
			throw new NotSupportedException();
		}

		internal static Exception NotXmlKnownType()
		{
			throw new NotImplementedException();
		}

		internal static Exception ValueNotAssignableToProperty()
		{
			return new InvalidCastException();
		}

		internal static Exception UnsupportedCollectionType()
		{
			throw new NotImplementedException();
		}

		internal static Exception CannotSetXsiTypeOnAttribute()
		{
			throw new NotImplementedException();
		}

		internal static Exception CursorNotInCoercibleState()
		{
			return new InvalidOperationException();
		}

		internal static Exception CursorCannotMoveToThatNode()
		{
			return new InvalidOperationException();
		}

		internal static Exception MustBeXmlNodeBasedCursor()
		{
			return new ArgumentException();
		}

		internal static Exception CursorNotInRealizableState()
		{
			return new InvalidOperationException();
		}

		internal static Exception OperationNotValidOnAttribute()
		{
			return new InvalidOperationException();
		}

		internal static Exception InvalidNamespaceUri()
		{
			throw new NotImplementedException();
		}

		internal static Exception NamespacePrefixIsRequired()
		{
			throw new NotImplementedException();
		}

		internal static Exception ArgumentNotCollectionType(string name)
		{
			return new ArgumentException();
		}
	}
}
