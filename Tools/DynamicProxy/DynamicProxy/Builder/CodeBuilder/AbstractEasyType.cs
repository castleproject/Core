// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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
	public abstract class AbstractEasyType
	{
		private int m_counter;

		protected TypeBuilder m_typebuilder;

		protected ConstructorCollection m_constructors;
		protected MethodCollection m_methods;
		protected NestedTypeCollection m_nested;
		protected PropertiesCollection m_properties;

		public AbstractEasyType()
		{
			m_nested = new NestedTypeCollection();
			m_methods = new MethodCollection();
			m_constructors = new ConstructorCollection();
			m_properties = new PropertiesCollection();
		}

		public void CreateDefaultConstructor( )
		{
			m_constructors.Add( new EasyDefaultConstructor( this ) );
		}

		public EasyConstructor CreateConstructor( params ArgumentReference[] arguments )
		{
			EasyConstructor member = new EasyConstructor( this, arguments );
			m_constructors.Add(  member );
			return member;
		}

		public EasyConstructor CreateRuntimeConstructor( params ArgumentReference[] arguments )
		{
			EasyRuntimeConstructor member = new EasyRuntimeConstructor( this, arguments );
			m_constructors.Add(  member );
			return member;
		}

		public EasyMethod CreateMethod( String name, ReturnReferenceExpression returnType, params ArgumentReference[] arguments )
		{
			EasyMethod member = new EasyMethod( this, name, returnType, arguments );
			m_methods.Add(member);
			return member;
		}

		public EasyMethod CreateMethod( String name, MethodAttributes attrs, ReturnReferenceExpression returnType, params Type[] args)
		{
			EasyMethod member = new EasyMethod( this, name, attrs, returnType, ArgumentsUtil.ConvertToArgumentReference(args) );
			m_methods.Add(member);
			return member;
		}

		public EasyRuntimeMethod CreateRuntimeMethod( String name, ReturnReferenceExpression returnType, params ArgumentReference[] arguments )
		{
			EasyRuntimeMethod member = new EasyRuntimeMethod( this, name, returnType, arguments );
			m_methods.Add(member);
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

			FieldBuilder fieldBuilder = m_typebuilder.DefineField( name, fieldType, atts );
			
			return new FieldReference( fieldBuilder );
		}

		public EasyProperty CreateProperty( String name, Type returnType )
		{
			EasyProperty prop = new EasyProperty( this, name, returnType );
			m_properties.Add(prop);
			return prop;
		}

		public ConstructorCollection Constructors
		{
			get { return m_constructors; }
		}

		public MethodCollection Methods
		{
			get { return m_methods; }
		}

		public TypeBuilder TypeBuilder
		{
			get { return m_typebuilder; }
		}

		internal Type BaseType
		{
			get { return TypeBuilder.BaseType; }
		}

		internal int IncrementAndGetCounterValue
		{
			get { return ++m_counter; }
		}

		public virtual Type BuildType()
		{
			EnsureBuildersAreInAValidState();

			Type type = m_typebuilder.CreateType();		

			foreach(EasyNested builder in m_nested)
			{
				builder.BuildType();
			}

			return type;
		}

		protected virtual void EnsureBuildersAreInAValidState()
		{
			if (m_constructors.Count == 0)
			{
				CreateDefaultConstructor();
			}

			foreach(IEasyMember builder in m_properties)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
			foreach(IEasyMember builder in m_constructors)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
			foreach(IEasyMember builder in m_methods)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
		}
	}
}
