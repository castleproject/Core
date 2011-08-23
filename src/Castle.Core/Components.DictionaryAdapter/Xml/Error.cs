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

#if !SILVERLIGHT
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	internal static class Error
	{
		internal static Exception AttributeConflict(PropertyDescriptor property)
		{
			throw new NotImplementedException();
		}

		internal static Exception NoXmlMetadata(Type type)
		{
			throw new NotImplementedException();
		}

		internal static Exception ArgumentNotDictionaryAdapter(string paramName)
		{
			return new ArgumentException("The argument is not a dictionary adapter.", paramName);
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

		internal static Exception ArgumentNull(string p)
		{
			return new ArgumentNullException("obj");
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
		}

		internal static Exception NotSupported()
		{
			return new NotSupportedException();
		}

		internal static Exception NoCurrentItem()
		{
			return new InvalidOperationException();
		}

		internal static Exception IteratorNotMutable()
		{
			throw new NotSupportedException();
		}
	}
}
#endif
