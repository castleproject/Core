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

namespace Castle.ManagementExtensions.Default.Strategy
{
	using System;
	using System.Text;
	using System.Collections;
	using System.Reflection;

	/// <summary>
	/// Summary description for ReflectionInvokerStrategy.
	/// </summary>
	public class ReflectionInvokerStrategy : InvokerStrategy
	{
		public ReflectionInvokerStrategy()
		{
		}

		#region InvokerStrategy Members

		public MDynamicSupport Create(Object instance)
		{
			ManagementInfo info = MInspector.BuildInfoFromStandardComponent(instance);

			return new ReflectedDynamicSupport(
				instance, info, 
				new MemberResolver(info, instance.GetType()));
		}

		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	class ReflectedDynamicSupport : MDynamicSupport
	{
		private Object instance;
		private ManagementInfo info;
		private MemberResolver resolver;

		public ReflectedDynamicSupport(Object instance, ManagementInfo info, MemberResolver resolver)
		{
			this.info     = info;
			this.instance = instance;
			this.resolver = resolver;
		}

		#region MDynamicSupport Members

		/// <summary>
		/// TODO: Summary
		/// </summary>
		/// <param name="action"></param>
		/// <param name="args"></param>
		/// <param name="signature"></param>
		/// <returns></returns>
		public Object Invoke(String action, Object[] args, Type[] signature)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			ManagementOperation operation = (ManagementOperation) info.Operations[action];

			if (operation == null)
			{
				throw new InvalidOperationException(String.Format("Operation {0} doesn't exists.", action));
			}

			MethodInfo method = resolver.GetMethod(MemberResolver.BuildOperationName(action, signature));

			if (method == null)
			{
				foreach(MethodInfo met in resolver.Methods)
				{
					if (!met.Name.Equals( operation.Name ))
					{
						continue;
					}

					ParameterInfo[] parameters = met.GetParameters();

					if (MemberResolver.Match(parameters, signature))
					{
						method = met;
						break;
					}
				}
			}

			if (method == null)
			{
				throw new InvalidOperationException(String.Format("Operation {0} doesn't exists for the specified signature.", action));
			}

			return method.Invoke(instance, args);
		}

		/// <summary>
		/// TODO: Summary
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Object GetAttributeValue(String name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			ManagementAttribute attribute = (ManagementAttribute) info.Attributes[name];

			if (attribute == null)
			{
				throw new InvalidOperationException(String.Format("Attribute {0} doesn't exists.", name));
			}

			PropertyInfo property = resolver.GetProperty(attribute.Name);
			
			if (!property.CanRead)
			{
				throw new InvalidOperationException(String.Format("Attribute {0} can't be read.", name));
			}

			MethodInfo getMethod = property.GetGetMethod();

			return getMethod.Invoke(instance, null);
		}

		/// <summary>
		/// TODO: Summary
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetAttributeValue(String name, Object value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			ManagementAttribute attribute = (ManagementAttribute) info.Attributes[name];

			if (attribute == null)
			{
				throw new InvalidOperationException(String.Format("Attribute {0} doesn't exists.", name));
			}
			
			PropertyInfo property = resolver.GetProperty(attribute.Name);
				
			if (!property.CanWrite)
			{
				throw new InvalidOperationException(String.Format("Attribute {0} is read-only.", name));
			}

			MethodInfo setMethod = property.GetSetMethod();

			setMethod.Invoke(instance, new object[] { value } );
		}

		/// <summary>
		/// TODO: Summary
		/// </summary>
		public ManagementInfo Info
		{
			get
			{
				return info;
			}
		}

		#endregion
	}
}
