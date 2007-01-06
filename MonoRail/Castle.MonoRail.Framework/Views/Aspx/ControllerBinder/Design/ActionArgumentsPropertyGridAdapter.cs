// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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


#if DOTNET2

namespace Castle.MonoRail.Framework.Views.Aspx.Design
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;

	internal class ActionArgumentsPropertyGridAdapter : ICustomTypeDescriptor

	{
		private readonly ActionArgumentCollection actionArgs;

		public ActionArgumentsPropertyGridAdapter(ActionArgumentCollection actionArgs)
		{
			this.actionArgs = actionArgs;
		}

		#region ICustomTypeDescriptor

		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return actionArgs;
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return GetProperties(new Attribute[0]);
		}

		#endregion

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			List<ActionArgumentDescriptor> properties = new List<ActionArgumentDescriptor>();

			foreach (ActionArgument actionArg in actionArgs)
			{
				properties.Add(new ActionArgumentDescriptor(actionArg));
			}

			return new PropertyDescriptorCollection(properties.ToArray());
		}
	}

	class ActionArgumentDescriptor : PropertyDescriptor
	{
		private readonly ActionArgument actionArg;

		private static readonly CategoryAttribute DataCategory = new CategoryAttribute("Data");

		internal ActionArgumentDescriptor(ActionArgument actionArg)
			: base(actionArg.Name, new Attribute[] {DataCategory})
		{
			this.actionArg = actionArg;
		}

		public override Type PropertyType
		{
			get { return typeof (string); }
		}

		public override void SetValue(object component, object value)
		{
			actionArg.Expression = (string) value;
		}

		public override object GetValue(object component)
		{
			return actionArg.Expression;
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override Type ComponentType
		{
			get { return typeof(ActionArgument); }
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override void ResetValue(object component)
		{
		}

		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}
	}
}

#endif
