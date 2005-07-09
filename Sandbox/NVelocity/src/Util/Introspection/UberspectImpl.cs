using System;
using ArrayIterator = NVelocity.Util.ArrayIterator;
using EnumerationIterator = NVelocity.Util.EnumerationIterator;
using RuntimeServices = NVelocity.Runtime.RuntimeServices;
using RuntimeLogger = NVelocity.Runtime.RuntimeLogger;
using AbstractExecutor = NVelocity.Runtime.Parser.Node.AbstractExecutor;
using PropertyExecutor = NVelocity.Runtime.Parser.Node.PropertyExecutor;
using GetExecutor = NVelocity.Runtime.Parser.Node.GetExecutor;
using BooleanPropertyExecutor = NVelocity.Runtime.Parser.Node.BooleanPropertyExecutor;
//using Iterator = java.util.Iterator;
//using Collection = java.util.Collection;
//using Map = java.util.Map;
//using ArrayList = java.util.ArrayList;
using System.Collections;

namespace NVelocity.Util.Introspection {
    /*
	* Copyright 2002-2004 The Apache Software Foundation.
	*
	* Licensed under the Apache License, Version 2.0 (the "License")
	* you may not use this file except in compliance with the License.
	* You may obtain a copy of the License at
	*
	*     http://www.apache.org/licenses/LICENSE-2.0
	*
	* Unless required by applicable law or agreed to in writing, software
	* distributed under the License is distributed on an "AS IS" BASIS,
	* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	* See the License for the specific language governing permissions and
	* limitations under the License.
	*/
	/// <summary>  Implementation of Uberspect to provide the default introspective
	/// functionality of Velocity
	/// *
	/// </summary>
	/// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version>  $Id: UberspectImpl.cs,v 1.1 2004/12/27 05:55:08 corts Exp $
	/// 
	/// </version>
	public class UberspectImpl : Uberspect, UberspectLoggable
	{
		/// <summary>  Sets the runtime logger - this must be called before anything
		/// else besides init() as to get the logger.  Makes the pull
		/// model appealing...
		/// </summary>
		virtual public RuntimeLogger RuntimeLogger
		{
			set
			{
				rlog = value;
				introspector = new Introspector(rlog);
			}
			
		}
		/// <summary>  Our runtime logger.
		/// </summary>
		private RuntimeLogger rlog;
		
		/// <summary>  the default Velocity introspector
		/// </summary>
		private static Introspector introspector;
		
		/// <summary>  init - does nothing - we need to have setRuntimeLogger
		/// called before getting our introspector, as the default
		/// vel introspector depends upon it.
		/// </summary>
		public virtual void  init()
		{
		}
		
		
		/// <summary>  To support iterative objects used in a <code>#foreach()</code>
		/// loop.
		/// *
		/// </summary>
		/// <param name="obj">The iterative object.
		/// </param>
		/// <param name="i">Info about the object's location.
		/// 
		/// </param>
		public virtual Iterator getIterator(System.Object obj, Info i)
		{
			if (obj.GetType().IsArray)
			{
				return new ArrayIterator(obj);
			}
			else if (obj is ICollection)
			{
				return new EnumerationIterator(((ICollection) obj).GetEnumerator());
			}
			else if (obj is IDictionary)
			{
				return new EnumerationIterator(((IDictionary) obj).Values.GetEnumerator());
			}
			else if (obj is Iterator)
			{
				rlog.debug("The iterative object in the #foreach() loop at " + i + " is of type java.util.Iterator.  Because " + "it is not resettable, if used in more than once it " + "may lead to unexpected results.");
				
				return ((Iterator) obj);
			}
			else if (obj is System.Collections.IEnumerator)
			{
				rlog.debug("The iterative object in the #foreach() loop at " + i + " is of type java.util.Enumeration.  Because " + "it is not resettable, if used in more than once it " + "may lead to unexpected results.");
				
				return new EnumerationIterator((System.Collections.IEnumerator) obj);
			}
			
			/*  we have no clue what this is  */
			rlog.warn("Could not determine type of iterator in " + "#foreach loop at " + i);
			
			return null;
		}
		
		/// <summary>  Method
		/// </summary>
		public virtual VelMethod getMethod(System.Object obj, System.String methodName, System.Object[] args, Info i)
		{
			if (obj == null)
				return null;
			
			System.Reflection.MethodInfo m = introspector.getMethod(obj.GetType(), methodName, args);
			
			return (m != null)?new VelMethodImpl(this, m):null;
		}
		
		/// <summary> Property  getter
		/// </summary>
		public virtual VelPropertyGet getPropertyGet(System.Object obj, System.String identifier, Info i)
		{
			AbstractExecutor executor;
			
			System.Type claz = obj.GetType();
			
			/*
			*  first try for a getFoo() type of property
			*  (also getfoo() )
			*/
			
			executor = new PropertyExecutor(rlog, introspector, claz, identifier);
			
			/*
			*  if that didn't work, look for get("foo")
			*/
			
			if (!executor.isAlive())
			{
				executor = new GetExecutor(rlog, introspector, claz, identifier);
			}
			
			/*
			*  finally, look for boolean isFoo()
			*/
			
			if (!executor.isAlive())
			{
				executor = new BooleanPropertyExecutor(rlog, introspector, claz, identifier);
			}
			
			return (executor != null)?new VelGetterImpl(this, executor):null;
		}
		
		/// <summary> Property setter
		/// </summary>
		public virtual VelPropertySet getPropertySet(System.Object obj, System.String identifier, System.Object arg, Info i)
		{
			System.Type claz = obj.GetType();
			
			VelPropertySet vs = null;
			VelMethod vm = null;
			try
			{
				/*
				*  first, we introspect for the set<identifier> setter method
				*/
				
				System.Object[] params_Renamed = new System.Object[]{arg};
				
				try
				{
					vm = getMethod(obj, "set" + identifier, params_Renamed, i);
					
					if (vm == null)
					{
						throw new System.MethodAccessException();
					}
				}
				catch (System.MethodAccessException nsme2)
				{
					System.Text.StringBuilder sb = new System.Text.StringBuilder("set");
					sb.Append(identifier);
					
					if (System.Char.IsLower(sb[3]))
					{
						sb[3] = System.Char.ToUpper(sb[3]);
					}
					else
					{
						sb[3] = System.Char.ToLower(sb[3]);
					}
					
					vm = getMethod(obj, sb.ToString(), params_Renamed, i);
					
					if (vm == null)
					{
						throw new System.MethodAccessException();
					}
				}
			}
			catch (System.MethodAccessException nsme)
			{
				/*
				*  right now, we only support the Map interface
				*/
				
				if (typeof(IDictionary).IsAssignableFrom(claz))
				{
					System.Object[] params_Renamed = new System.Object[]{new System.Object(), new System.Object()};
					
					vm = getMethod(obj, "put", params_Renamed, i);
					
					if (vm != null)
						return new VelSetterImpl(this, vm, identifier);
				}
			}
			
			return (vm != null)?new VelSetterImpl(this, vm):null;
		}
		
		/// <summary>  Implementation of VelMethod
		/// </summary>
		//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'VelMethodImpl' to access its enclosing instance. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1019"'
		public class VelMethodImpl : VelMethod
		{
			private void  InitBlock(UberspectImpl enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private UberspectImpl enclosingInstance;
			virtual public bool Cacheable
			{
				get
				{
					return true;
				}
				
			}
			virtual public System.String MethodName
			{
				get
				{
					return method.Name;
				}
				
			}
			virtual public System.Type ReturnType
			{
				get
				{
					return method.ReturnType;
				}
				
			}
			public UberspectImpl Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			internal System.Reflection.MethodInfo method = null;
			
			public VelMethodImpl(UberspectImpl enclosingInstance, System.Reflection.MethodInfo m)
			{
				InitBlock(enclosingInstance);
				method = m;
			}
			
			private VelMethodImpl(UberspectImpl enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}
			
			public virtual System.Object invoke(System.Object o, System.Object[] params_Renamed)
			{
				return method.Invoke(o, (System.Object[]) params_Renamed);
			}
			
			
			
		}
		
		//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'VelGetterImpl' to access its enclosing instance. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1019"'
		public class VelGetterImpl : VelPropertyGet
		{
			private void  InitBlock(UberspectImpl enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private UberspectImpl enclosingInstance;
			virtual public bool Cacheable
			{
				get
				{
					return true;
				}
				
			}
			virtual public System.String MethodName
			{
				get
				{
					return ae.Method.Name;
				}
				
			}
			public UberspectImpl Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			internal AbstractExecutor ae = null;
			
			public VelGetterImpl(UberspectImpl enclosingInstance, AbstractExecutor exec)
			{
				InitBlock(enclosingInstance);
				ae = exec;
			}
			
			private VelGetterImpl(UberspectImpl enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}
			
			public virtual System.Object invoke(System.Object o)
			{
				return ae.execute(o);
			}
			
			
		}
		
		//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'VelSetterImpl' to access its enclosing instance. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1019"'
		public class VelSetterImpl : VelPropertySet
		{
			private void  InitBlock(UberspectImpl enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private UberspectImpl enclosingInstance;
			virtual public bool Cacheable
			{
				get
				{
					return true;
				}
				
			}
			virtual public System.String MethodName
			{
				get
				{
					return vm.MethodName;
				}
				
			}
			public UberspectImpl Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			internal VelMethod vm = null;
			internal System.String putKey = null;
			
			public VelSetterImpl(UberspectImpl enclosingInstance, VelMethod velmethod)
			{
				InitBlock(enclosingInstance);
				this.vm = velmethod;
			}
			
			public VelSetterImpl(UberspectImpl enclosingInstance, VelMethod velmethod, System.String key)
			{
				InitBlock(enclosingInstance);
				this.vm = velmethod;
				putKey = key;
			}
			
			private VelSetterImpl(UberspectImpl enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}
			
			public virtual System.Object invoke(System.Object o, System.Object value_Renamed)
			{
				ArrayList al = new ArrayList();
				
				if (putKey != null)
				{
					al.Add(putKey);
					al.Add(value_Renamed);
				}
				else
				{
					al.Add(value_Renamed);
				}
				
				return vm.invoke(o, al.ToArray());
			}
			
			
		}
	}
}