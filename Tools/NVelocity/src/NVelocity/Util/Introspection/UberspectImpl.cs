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

namespace NVelocity.Util.Introspection
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Text;
	using NVelocity.Runtime.Parser.Node;
	using Runtime;

	/// <summary>  Implementation of Uberspect to provide the default introspective
	/// functionality of Velocity
	/// *
	/// </summary>
	/// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version>  $Id: UberspectImpl.cs,v 1.1 2004/12/27 05:55:08 corts Exp $
	/// 
	/// </version>
	public class UberspectImpl : IUberspect, UberspectLoggable
	{
		/// <summary>
		/// Our runtime logger.
		/// </summary>
		private IRuntimeLogger runtimeLogger;

		/// <summary>
		/// the default Velocity introspector
		/// </summary>
		private static Introspector introspector;

		/// <summary>
		/// Sets the runtime logger - this must be called before anything
		/// else besides init() as to get the logger.  Makes the pull
		/// model appealing...
		/// </summary>
		public IRuntimeLogger RuntimeLogger
		{
			set
			{
				runtimeLogger = value;
				introspector = new Introspector(runtimeLogger);
			}
		}

		/// <summary>
		/// init - does nothing - we need to have setRuntimeLogger
		/// called before getting our introspector, as the default
		/// vel introspector depends upon it.
		/// </summary>
		public void Init()
		{
		}

		/// <summary>
		/// Method
		/// </summary>
		public IVelMethod GetMethod(Object obj, String methodName, Object[] args, Info i)
		{
			if (obj == null)
			{
				return null;
			}

			MethodInfo m = introspector.GetMethod(obj.GetType(), methodName, args);

			return (m != null) ? new VelMethodImpl(m) : null;
		}

		/// <summary>
		/// Property  getter
		/// </summary>
		public IVelPropertyGet GetPropertyGet(Object obj, String identifier, Info i)
		{
			AbstractExecutor executor;

			Type type = obj.GetType();

			/*
			*  first try for a getFoo() type of property
			*  (also getfoo() )
			*/

			executor = new PropertyExecutor(runtimeLogger, introspector, type, identifier);

			/*
			*  if that didn't work, look for get("foo")
			*/

			if (!executor.IsAlive)
			{
				executor = new GetExecutor(runtimeLogger, introspector, type, identifier);
			}

			/*
			*  finally, look for boolean isFoo()
			*/

			if (!executor.IsAlive)
			{
				executor = new BooleanPropertyExecutor(runtimeLogger, introspector, type, identifier);
			}

			return new VelGetterImpl(executor);
		}

		/// <summary> Property setter
		/// </summary>
		public IVelPropertySet GetPropertySet(Object obj, String identifier, Object arg, Info i)
		{
			Type type = obj.GetType();

			IVelMethod method = null;

			try
			{
				/*
				*  first, we introspect for the set<identifier> setter method
				*/

				Object[] parameters = new Object[] {arg};

				try
				{
					method = GetMethod(obj, string.Format("set{0}", identifier), parameters, i);

					if (method == null)
					{
						throw new MethodAccessException();
					}
				}
				catch(MethodAccessException)
				{
					StringBuilder sb = new StringBuilder("set");
					sb.Append(identifier);

					if (Char.IsLower(sb[3]))
					{
						sb[3] = Char.ToUpper(sb[3]);
					}
					else
					{
						sb[3] = Char.ToLower(sb[3]);
					}

					method = GetMethod(obj, sb.ToString(), parameters, i);

					if (method == null)
						throw;
				}
			}
			catch(MethodAccessException)
			{
				// right now, we only support the IDictionary interface
				if (typeof(IDictionary).IsAssignableFrom(type))
				{
					Object[] parameters = new Object[] {new Object(), new Object()};

					method = GetMethod(obj, "Add", parameters, i);

					if (method != null)
					{
						return new VelSetterImpl(method, identifier);
					}
				}
			}

			return (method != null) ? new VelSetterImpl(method) : null;
		}

		/// <summary>
		/// Implementation of <see cref="IVelMethod"/>.
		/// </summary>
		public class VelMethodImpl : IVelMethod
		{
			public VelMethodImpl(MethodInfo methodInfo)
			{
				method = methodInfo;
			}

			public bool Cacheable
			{
				get { return true; }
			}

			public String MethodName
			{
				get { return method.Name; }
			}

			public Type ReturnType
			{
				get { return method.ReturnType; }
			}

			internal MethodInfo method = null;

			public Object Invoke(Object o, Object[] parameters)
			{
				return method.Invoke(o, parameters);
			}
		}

		/// <summary>
		/// Implementation of <see cref="IVelPropertyGet"/>.
		/// </summary>
		public class VelGetterImpl : IVelPropertyGet
		{
			internal AbstractExecutor abstractExecutor = null;

			public VelGetterImpl(AbstractExecutor abstractExecutor)
			{
				this.abstractExecutor = abstractExecutor;
			}

			public bool Cacheable
			{
				get { return true; }
			}

			public String MethodName
			{
				get
				{
					if (abstractExecutor.Property != null)
					{
						return abstractExecutor.Property.Name;
					}

					if (abstractExecutor.Method != null)
					{
						return abstractExecutor.Method.Name;
					}

					return "undefined";
				}
			}

			public Object Invoke(Object o)
			{
				return abstractExecutor.Execute(o);
			}
		}

		public class VelSetterImpl : IVelPropertySet
		{
			internal IVelMethod velMethod = null;
			internal String putKey = null;

			public VelSetterImpl(IVelMethod velMethod)
			{
				this.velMethod = velMethod;
			}

			public VelSetterImpl(IVelMethod velMethod, string key)
			{
				this.velMethod = velMethod;
				putKey = key;
			}

			public bool Cacheable
			{
				get { return true; }
			}

			public String MethodName
			{
				get { return velMethod.MethodName; }
			}

			public Object Invoke(Object o, Object value)
			{
				ArrayList al = new ArrayList();

				if (putKey != null)
				{
					al.Add(putKey);
					al.Add(value);
				}
				else
				{
					al.Add(value);
				}

				return velMethod.Invoke(o, al.ToArray());
			}
		}
	}
}