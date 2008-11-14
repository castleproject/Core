// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.Brail
{
	using System;
	using System.Collections;
	using System.Reflection;
	using Boo.Lang;
	using Boo.Lang.Compiler.Steps;
	using Boo.Lang.Compiler.TypeSystem;
	using Boo.Lang.Runtime;

	/// <summary>
	/// This is here because we need to overcome a tendecy of Dynamic Proxy to generate virtual override that
	/// do not match exactly what the compiler does.
	/// This means that when you do GetMethod("Foo") and DP has proxied it, you would get an exception because it would
	/// recognize two methods with this name.
	/// We recognize when we are trying to invoke something that DP has build and act accordingly.
	/// 
	/// The code is mostly taken fro Boo.Lang.Runtime.RuntimeServices, and modified to understand that when the type is from DP, is should
	/// use DeclareOnly
	/// </summary>
	public class ExpandDuckTypedExpressions_WorkaroundForDuplicateVirtualMethods : ExpandDuckTypedExpressions
	{
		private const BindingFlags DefaultBindingFlags = BindingFlags.Public |
		                                                 BindingFlags.NonPublic |
		                                                 BindingFlags.OptionalParamBinding |
		                                                 BindingFlags.Static |
		                                                 BindingFlags.FlattenHierarchy |
		                                                 BindingFlags.Instance;

		private const BindingFlags GetPropertyBindingFlags = DefaultBindingFlags |
		                                                     BindingFlags.GetProperty |
		                                                     BindingFlags.GetField;

		private const BindingFlags InvokeBindingFlags = DefaultBindingFlags |
		                                                BindingFlags.InvokeMethod;

		private const BindingFlags SetPropertyBindingFlags = DefaultBindingFlags |
		                                                     BindingFlags.SetProperty |
		                                                     BindingFlags.SetField;

		protected override void InitializeDuckTypingServices()
		{
			base.InitializeDuckTypingServices();
			IType duckTypingServices = TypeSystemServices.Map(GetType());
			RuntimeServices_Invoke = GetResolvedMethod(duckTypingServices, "Invoke");
			RuntimeServices_SetProperty = GetResolvedMethod(duckTypingServices, "SetProperty");
			RuntimeServices_GetProperty = GetResolvedMethod(duckTypingServices, "GetProperty");
			RuntimeServices_SetSlice = GetResolvedMethod(duckTypingServices, "SetSlice");
			RuntimeServices_GetSlice = GetResolvedMethod(duckTypingServices, "GetSlice");
		}

		public static object Invoke(object target, string name, object[] args)
		{
			args = IgnoreNull.ReplaceIgnoreNullsWithTargets(args);

			IQuackFu duck = target as IQuackFu;
			if (null != duck) return duck.QuackInvoke(name, args);


			Type type = target as Type;
			if (null != type)
			{
				// static method
				return type.InvokeMember(name,
				                         InvokeBindingFlags,
				                         null,
				                         null,
				                         args);
			}
			if(target==null)
				throw new NullReferenceException("Could not invoke method "+name+" on null target");

			Type targetType = target.GetType();
            if (args.Length == 2 && 
                args[0] is string && 
                name == "op_Addition")
            {
                return ((string) args[0]) + args[1];
            }
			return targetType.InvokeMember(name,
			                               ResolveFlagsToUse(targetType, InvokeBindingFlags),
			                               null,
			                               target,
			                               args);
		}

		public static object SetProperty(object target, string name, object value)
		{

			if (value is IgnoreNull) {
				value = IgnoreNull.ExtractTarget((IgnoreNull)value);
			}

			IQuackFu duck = target as IQuackFu;
			if (null != duck) return duck.QuackSet(name, null, value);					

			Type type = target as Type;
			if (null == type)
			{
				target.GetType().InvokeMember(name,
				                              ResolveFlagsToUse(target.GetType(), SetPropertyBindingFlags),
				                              null,
				                              target,
				                              new object[] {value});
			}
			else
			{
				// static member
				type.InvokeMember(name,
				                  SetPropertyBindingFlags,
				                  null,
				                  null,
				                  new object[] {value});
			}
			return value;
		}

		public static object GetProperty(object target, string name)
		{
			IQuackFu duck = target as IQuackFu;
			if (null != duck) return duck.QuackGet(name, null);
			Type type = target as Type;
			if (null == type)
			{
				return target.GetType().InvokeMember(name,
				                                     ResolveFlagsToUse(target.GetType(), GetPropertyBindingFlags),
				                                     null,
				                                     target,
				                                     null);
			}
			else
			{
				// static member
				return type.InvokeMember(name,
				                         GetPropertyBindingFlags,
				                         null,
				                         null,
				                         null);
			}
		}

		public static object GetSlice(object target, string name, object[] args)
		{
			IQuackFu duck = target as IQuackFu;
			if (null != duck) return duck.QuackGet(name, args);

			Type type = target.GetType();
			if ("" == name)
			{
				if (IsGetArraySlice(target, args))
				{
					return GetArraySlice(target, args);
				}
				name = GetDefaultMemberName(type);
			}

			MemberInfo member = SelectSliceMember(GetMember(type, name), ref args, SetOrGet.Get);
			return GetSlice(target, member, args);
		}

		public static object SetSlice(object target, string name, object[] args)
		{
			args = IgnoreNull.ReplaceIgnoreNullsWithTargets(args);
			IQuackFu duck = target as IQuackFu;
			if (null != duck)
				return duck.QuackSet(name, (object[]) RuntimeServices.GetRange2(args, 0, args.Length - 1), args[args.Length - 1]);

			Type type = target.GetType();
			if ("" == name)
			{
				if (IsSetArraySlice(target, args))
				{
					return SetArraySlice(target, args);
				}
				name = GetDefaultMemberName(type);
			}
			MemberInfo member = SelectSliceMember(GetMember(type, name), ref args, SetOrGet.Set);
			return SetSlice(target, member, args);
		}

		private static object SetSlice(object target, MemberInfo member, object[] args)
		{
			switch(member.MemberType)
			{
				case MemberTypes.Field:
					{
						FieldInfo field = (FieldInfo) member;
						SetSlice(field.GetValue(target), "", args);
						break;
					}
				case MemberTypes.Method:
					{
						MethodInfo method = (MethodInfo) member;
						method.Invoke(target, args);
						break;
					}
				case MemberTypes.Property:
					{
                        PropertyInfo prop = (PropertyInfo)member;
                        if (prop.GetIndexParameters().Length != 0)
                            GetSetMethod(prop).Invoke(target, args);
                        else
                            SetArraySlice(GetGetMethod(prop).Invoke(target, new object[0]), args);
						break;
					}
				default:
					{
						MemberNotSupported(member);
						break;
					}
			}
			// last argument is the value
			return args[args.Length - 1];
		}

		private static MemberInfo[] GetMember(Type type, string name)
		{
			MemberInfo[] found = type.GetMember(name, ResolveFlagsToUse(type, DefaultBindingFlags));
			if (null == found || 0 == found.Length)
			{
				throw new MissingMemberException(type.FullName, name);
			}
			return found;
		}


		private static BindingFlags ResolveFlagsToUse(Type type, BindingFlags flags)
		{
			if (type.Assembly.FullName.StartsWith("DynamicAssemblyProxyGen") ||
			    type.Assembly.FullName.StartsWith("DynamicProxyGenAssembly2"))
			{
				return flags | BindingFlags.DeclaredOnly;
			}

			return flags;
		}

		#region Taken Directly from RuntimeServices

		private static object GetSlice(object target, MemberInfo member, object[] args)
		{
			switch(member.MemberType)
			{
				case MemberTypes.Field:
					{
						FieldInfo field = (FieldInfo) member;
						return GetSlice(field.GetValue(target), "", args);
					}
				case MemberTypes.Method:
					{
						MethodInfo method = (MethodInfo) member;
						return method.Invoke(target, args);
					}
				case MemberTypes.Property:
			        {
			            PropertyInfo prop = (PropertyInfo) member;
                        if (prop.GetIndexParameters().Length != 0)
                            return GetGetMethod(prop).Invoke(target, args);
                        else
                            return GetArraySlice(GetGetMethod(prop).Invoke(target, new object[0]), args);
			        }
			    default:
					{
						MemberNotSupported(member);
						return null; // this line is never reached
					}
			}
		}

		private static MemberInfo SelectSliceMember(MemberInfo[] found, ref object[] args, SetOrGet sliceKind)
		{
			if (1 == found.Length) return found[0];
			MethodBase[] candidates = new MethodBase[found.Length];
			for(int i = 0; i < found.Length; ++i)
			{
				MemberInfo member = found[i];
				PropertyInfo property = member as PropertyInfo;
				if (null == property) MemberNotSupported(member);
				MethodInfo method = sliceKind == SetOrGet.Get ? GetGetMethod(property) : GetSetMethod(property);
				candidates[i] = method;
			}
			object state = null;
			return
				Type.DefaultBinder.BindToMethod(DefaultBindingFlags | BindingFlags.OptionalParamBinding, candidates, ref args, null,
				                                null, null, out state);
		}

		private static void MemberNotSupported(MemberInfo member)
		{
			throw new ArgumentException(string.Format("Member not supported: {0}", member));
		}

		private static String GetDefaultMemberName(Type type)
		{
			DefaultMemberAttribute attribute =
				(DefaultMemberAttribute) Attribute.GetCustomAttribute(type, typeof(DefaultMemberAttribute));
			return attribute != null ? attribute.MemberName : "";
		}

		private static MethodInfo GetSetMethod(PropertyInfo property)
		{
			MethodInfo method = property.GetSetMethod(true);
			if (null == method) MemberNotSupported(property);
			return method;
		}

		private static object GetArraySlice(object target, object[] args)
		{
			IList list = (IList) target;
			return list[RuntimeServices.NormalizeIndex(list.Count, (int) args[0])];
		}

		private static bool IsGetArraySlice(object target, object[] args)
		{
			return args.Length == 1 && target is Array;
		}

		private static MethodInfo GetGetMethod(PropertyInfo property)
		{
			MethodInfo method = property.GetGetMethod(true);
			if (null == method) MemberNotSupported(property);
			return method;
		}

		private static object SetArraySlice(object target, object[] args)
		{
			IList list = (IList) target;
			list[RuntimeServices.NormalizeIndex(list.Count, (int) args[0])] = args[1];
			return args[1];
		}

		private static bool IsSetArraySlice(object target, object[] args)
		{
			return args.Length == 2 && target is Array;
		}

		

		private IMethod GetResolvedMethod(IType type, string name)
		{
			IMethod method = NameResolutionService.ResolveMethod(type, name);
			if (null == method) throw new ArgumentException(string.Format("Method '{0}' not found in type '{1}'", type, name));
			return method;
		}

		
		private enum SetOrGet
		{
			Set,
			Get
		} ;

		#endregion
	}
}