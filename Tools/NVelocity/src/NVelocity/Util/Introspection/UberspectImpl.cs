namespace NVelocity.Util.Introspection
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Text;

	using NVelocity.Runtime;
	using NVelocity.Runtime.Parser.Node;
	
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
		private IRuntimeLogger rlog;

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
				rlog = value;
				introspector = new Introspector(rlog);
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
				return null;

			MethodInfo m = introspector.GetMethod(obj.GetType(), methodName, args);

			return (m != null) ? new VelMethodImpl(m) : null;
		}

		/// <summary>
		/// Property  getter
		/// </summary>
		public IVelPropertyGet GetPropertyGet(Object obj, String identifier, Info i)
		{
			AbstractExecutor executor;

			Type claz = obj.GetType();

			/*
			*  first try for a getFoo() type of property
			*  (also getfoo() )
			*/

			executor = new PropertyExecutor(rlog, introspector, claz, identifier);

			/*
			*  if that didn't work, look for get("foo")
			*/

			if (!executor.IsAlive)
			{
				executor = new GetExecutor(rlog, introspector, claz, identifier);
			}

			/*
			*  finally, look for boolean isFoo()
			*/

			if (!executor.IsAlive)
			{
				executor = new BooleanPropertyExecutor(rlog, introspector, claz, identifier);
			}

			return (executor != null) ? new VelGetterImpl(executor) : null;
		}

		/// <summary> Property setter
		/// </summary>
		public IVelPropertySet GetPropertySet(Object obj, String identifier, Object arg, Info i)
		{
			Type claz = obj.GetType();

			IVelMethod vm = null;
			try
			{
				/*
				*  first, we introspect for the set<identifier> setter method
				*/

				Object[] parameters = new Object[] {arg};

				try
				{
					vm = GetMethod(obj, "set" + identifier, parameters, i);

					if (vm == null)
					{
						throw new MethodAccessException();
					}
				}
				catch (MethodAccessException)
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

					vm = GetMethod(obj, sb.ToString(), parameters, i);

					if (vm == null)
						throw;
				}
			}
			catch (MethodAccessException)
			{
				// right now, we only support the IDictionary interface
				if (typeof(IDictionary).IsAssignableFrom(claz))
				{
					Object[] parameters = new Object[] {new Object(), new Object()};

					vm = GetMethod(obj, "Add", parameters, i);

					if (vm != null)
						return new VelSetterImpl(vm, identifier);
				}
			}

			return (vm != null) ? new VelSetterImpl(vm) : null;
		}

		/// <summary>
		/// Implementation of <see cref="IVelMethod"/>.
		/// </summary>
		public class VelMethodImpl : IVelMethod
		{
			public VelMethodImpl(MethodInfo m)
			{
				this.method = m;
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
			public VelGetterImpl(AbstractExecutor exec)
			{
				this.ae = exec;
			}

			public bool Cacheable
			{
				get { return true; }
			}

			public String MethodName
			{
				get
				{
					if (ae.Property.Name != null)
						return ae.Property.Name;

					if (ae.Method != null)
						return ae.Method.Name;
					
					return "undefined";
				}
			}

			internal AbstractExecutor ae = null;

			public Object Invoke(Object o)
			{
				return ae.Execute(o);
			}
		}

		public class VelSetterImpl : IVelPropertySet
		{
			public bool Cacheable
			{
				get { return true; }
			}

			public String MethodName
			{
				get { return vm.MethodName; }
			}

			internal IVelMethod vm = null;
			internal String putKey = null;

			public VelSetterImpl(IVelMethod velmethod)
			{
				this.vm = velmethod;
			}

			public VelSetterImpl(IVelMethod velmethod, string key)
			{
				this.vm = velmethod;
				this.putKey = key;
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

				return vm.Invoke(o, al.ToArray());
			}
		}
	}
}
