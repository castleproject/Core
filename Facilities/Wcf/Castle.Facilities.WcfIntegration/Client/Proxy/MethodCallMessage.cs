// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.WcfIntegration.Proxy
{
	using System;
	using System.Collections;
	using System.Linq;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Runtime.Remoting.Messaging;

	public class MethodCallMessage : IMethodCallMessage
	{
		private readonly MethodInfo method;
		private readonly ParameterInfo[] parameters;
		private readonly object[] arguments;
		private readonly object[] inArguments;
		private IDictionary properties;
		private LogicalCallContext logicalContext;

		private static readonly object[] EmptyArguments = new object[0];
		private static Func<LogicalCallContext> createLogicalContext = LogicalCallContextBuilder();

		public MethodCallMessage(MethodInfo method, object[] arguments)
		{
			this.method = method;
			parameters = method.GetParameters();
			this.arguments = arguments ?? EmptyArguments;
			inArguments = arguments.Where((a, i) => !parameters[i].IsOut).ToArray();
		}

		public IDictionary Properties
		{
			get
			{
				if (properties == null)
					properties = new Hashtable();
				return properties;
			}
		}

		public string GetArgName(int index)
		{
			return parameters[index].Name;
		}

		public object GetArg(int argNum)
		{
			return arguments[argNum];
		}

		public string Uri
		{
			get { return null; }
		}

		public string MethodName
		{
			get { return method.Name; }
		}

		public string TypeName
		{
			get { return method.DeclaringType.Name; }
		}

		public object MethodSignature
		{
			get { return parameters.Select(p => p.ParameterType).ToArray(); }
		}

		public int ArgCount
		{
			get { return arguments.Length; }
		}

		public object[] Args
		{
			get { return arguments; }
		}

		public bool HasVarArgs
		{
			get { return false; }
		}

		public LogicalCallContext LogicalCallContext
		{
			get
			{
				if (logicalContext == null)
					logicalContext = createLogicalContext();
				return logicalContext;
			}
		}

		public MethodBase MethodBase
		{
			get { return method; }
		}

		public string GetInArgName(int index)
		{
			return parameters.Where(p => !p.IsOut).ElementAt(index).Name;
		}

		public object GetInArg(int argNum)
		{
			return inArguments[argNum];
		}

		public int InArgCount
		{
			get { return inArguments.Length; }
		}

		public object[] InArgs
		{
			get { return inArguments;}
		}

		private static Func<LogicalCallContext> LogicalCallContextBuilder()
		{
			var type = typeof(LogicalCallContext);
			var ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
			var creator = new DynamicMethod("CreateLogicalContext", type, null, type);
			var generator = creator.GetILGenerator();
			generator.Emit(OpCodes.Newobj, ctor);
			generator.Emit(OpCodes.Ret);
			return (Func<LogicalCallContext>)creator.CreateDelegate(typeof(Func<LogicalCallContext>));
		}
	}
}