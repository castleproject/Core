// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions.Default
{
	using System;
	using System.Reflection;

	public enum ComponentType
	{
		None,
		Standard,
		Dynamic
	}

	/// <summary>
	/// Summary description for MInspector.
	/// </summary>
	public class MInspector
	{
		private MInspector()
		{
		}

		/// <summary>
		/// TODO: Summary
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static ComponentType Inspect(Object instance)
		{
			return Inspect( instance.GetType() );
		}

		public static ComponentType Inspect(Type target)
		{
			if (typeof(MDynamicSupport).IsAssignableFrom(target))
			{
				return ComponentType.Dynamic;
			}
			else
			{
				if (target.IsDefined( typeof(ManagedComponentAttribute), true ))
				{
					return ComponentType.Standard;
				}
			}

			return ComponentType.None;
		}

		/// <summary>
		/// TODO: Summary
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static ManagementInfo BuildInfoFromStandardComponent(Object instance)
		{
			ManagementInfo info = new ManagementInfo();

			SetupManagedComponent(info, instance);
			SetupManagedOperations(info, instance);
			SetupManagedAttributes(info, instance);

			return info;
		}

		private static void SetupManagedComponent(ManagementInfo info, Object instance)
		{
			Object[] componentAtt = 
				instance.GetType().GetCustomAttributes( 
					typeof(ManagedComponentAttribute), true );

			if (componentAtt == null || componentAtt.Length == 0)
			{
				throw new StandardComponentException("Standard component must use ManagedComponentAttribute attribute.");
			}

			ManagedComponentAttribute compAtt = componentAtt[0] as ManagedComponentAttribute;

			info.Description = compAtt.Description;
		}
		
		private static void SetupManagedOperations(ManagementInfo info, Object instance)
		{
			MethodInfo[] methods = instance.GetType().GetMethods(BindingFlags.Public|BindingFlags.Instance);

			foreach(MethodInfo minfo in methods)
			{
				if (minfo.IsDefined( typeof(ManagedOperationAttribute), true ))
				{
					object[] atts = minfo.GetCustomAttributes( typeof(ManagedOperationAttribute), true );

					ManagedOperationAttribute att = (ManagedOperationAttribute) atts[0];

					ParameterInfo[] parameters = minfo.GetParameters();

					Type[] arguments = new Type[ parameters.Length ];

					for(int i=0 ; i < parameters.Length; i++ )
					{
						arguments[i] = parameters[i].ParameterType;
					}

					ManagementOperation operation = new ManagementOperation(minfo.Name, att.Description, arguments);

					info.Operations.Add(operation);
				}
			}
		}

		private static void SetupManagedAttributes(ManagementInfo info, Object instance)
		{
			PropertyInfo[] properties = instance.GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance);

			foreach(PropertyInfo minfo in properties)
			{
				if (minfo.IsDefined( typeof(ManagedAttributeAttribute), true ))
				{
					object[] atts = minfo.GetCustomAttributes( typeof(ManagedAttributeAttribute), true );

					ManagedAttributeAttribute att = (ManagedAttributeAttribute) atts[0];

					ManagementAttribute attribute = new ManagementAttribute(minfo.Name, att.Description, minfo.PropertyType);

					info.Attributes.Add(attribute);
				}
			}
		}
	}
}
