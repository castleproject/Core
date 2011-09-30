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

	public class XmlSelfAccessor : XmlAccessor
	{
		private States states;

		internal static readonly XmlAccessorFactory<XmlSelfAccessor>
			Factory = (name, type, context) => new XmlSelfAccessor(type, context);

		public XmlSelfAccessor(Type clrType, IXmlAccessorContext context)
			: base(clrType, context) { }

		public override bool IsNillable
		{
			get { return 0 != (states & States.Nillable); }
		}

		public override bool IsVolatile
		{
			get { return 0 != (states & States.Volatile); }
		}

		public override void ConfigureNillable(bool isNillable)
		{
			if (isNillable)
				states |= States.Nillable;
		}

		public override void ConfigureVolatile(bool isVolatile)
		{
			if (isVolatile)
				states |= States.Volatile;
		}

		public override IXmlCursor SelectPropertyNode(IXmlNode parentNode, bool mutable)
		{
			return parentNode.SelectSelf(ClrType);
		}

		[Flags]
		private enum States
		{
			Nillable,
			Volatile
		}
	}
}
