// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Builder.CodeBuilder
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;

	/// <summary>
	/// Summary description for AbstractEasyType.
	/// </summary>
	[CLSCompliant(false)]
	public abstract class AbstractEasyType
	{
		private int _counter;

		protected TypeBuilder _typebuilder;

		protected ConstructorCollection _constructors;
		protected MethodCollection _methods;
		protected NestedTypeCollection _nested;
		protected PropertiesCollection _properties;
		protected EventsCollection _events;

		public AbstractEasyType()
		{
			_nested = new NestedTypeCollection();
			_methods = new MethodCollection();
			_constructors = new ConstructorCollection();
			_properties = new PropertiesCollection();
			_events = new EventsCollection();
		}

		public void CreateDefaultConstructor( )
		{
			_constructors.Add( new EasyDefaultConstructor( this ) );
		}

		public EasyConstructor CreateConstructor( params ArgumentReference[] arguments )
		{
			EasyConstructor member = new EasyConstructor( this, arguments );
			_constructors.Add(  member );
			return member;
		}

		public EasyConstructor CreateRuntimeConstructor( params ArgumentReference[] arguments )
		{
			EasyRuntimeConstructor member = new EasyRuntimeConstructor( this, arguments );
			_constructors.Add(  member );
			return member;
		}

		public EasyMethod CreateMethod( String name, ReturnReferenceExpression returnType, params ArgumentReference[] arguments )
		{
			EasyMethod member = new EasyMethod( this, name, returnType, arguments );
			_methods.Add(member);
			return member;
		}

		public EasyMethod CreateMethod( String name, ReturnReferenceExpression returnType, MethodAttributes attributes, params ArgumentReference[] arguments )
		{
			EasyMethod member = new EasyMethod( this, name, attributes, returnType, arguments );
			_methods.Add(member);
			return member;
		}

		public EasyMethod CreateMethod( String name, MethodAttributes attrs, ReturnReferenceExpression returnType, params Type[] args)
		{
			EasyMethod member = new EasyMethod( this, name, attrs, returnType, ArgumentsUtil.ConvertToArgumentReference(args) );
			_methods.Add(member);
			return member;
		}

		public EasyRuntimeMethod CreateRuntimeMethod( String name, ReturnReferenceExpression returnType, params ArgumentReference[] arguments )
		{
			EasyRuntimeMethod member = new EasyRuntimeMethod( this, name, returnType, arguments );
			_methods.Add(member);
			return member;
		}

		public FieldReference CreateField( string name, Type fieldType )
		{
			return CreateField(name, fieldType, true);
		}

		public FieldReference CreateField( string name, Type fieldType, bool serializable )
		{
			FieldAttributes atts = FieldAttributes.Public;

			if (!serializable)
			{
				atts |= FieldAttributes.NotSerialized;
			}

			FieldBuilder fieldBuilder = _typebuilder.DefineField( name, fieldType, atts );
			
			return new FieldReference( fieldBuilder );
		}

		public EasyProperty CreateProperty( String name, Type returnType )
		{
			EasyProperty prop = new EasyProperty( this, name, returnType );
			_properties.Add(prop);
			return prop;
		}

		public EasyProperty CreateProperty( PropertyInfo property )
		{
			EasyProperty prop = new EasyProperty( this, property.Name, property.PropertyType );
			prop.IndexParameters = property.GetIndexParameters();
			_properties.Add(prop);
			return prop;
		}

		public EasyEvent CreateEvent( String name, Type eventHandlerType )
		{
			EasyEvent easyEvent = new EasyEvent( this, name, eventHandlerType );
			_events.Add(easyEvent);
			return easyEvent;
		}

		public ConstructorCollection Constructors
		{
			get { return _constructors; }
		}

		public MethodCollection Methods
		{
			get { return _methods; }
		}

		public TypeBuilder TypeBuilder
		{
			get { return _typebuilder; }
		}

		internal Type BaseType
		{
			get { return TypeBuilder.BaseType; }
		}

		internal int IncrementAndGetCounterValue
		{
			get { return ++_counter; }
		}

		public virtual Type BuildType()
		{
			EnsureBuildersAreInAValidState();

			Type type = _typebuilder.CreateType();		

			foreach(EasyNested builder in _nested)
			{
				builder.BuildType();
			}

			return type;
		}

		protected virtual void EnsureBuildersAreInAValidState()
		{
			if (_constructors.Count == 0)
			{
				CreateDefaultConstructor();
			}

			foreach(IEasyMember builder in _properties)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
			foreach(IEasyMember builder in _events)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
			foreach(IEasyMember builder in _constructors)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
			foreach(IEasyMember builder in _methods)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
		}
	}
}
