// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Test
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Reflection;
	using System.Diagnostics;
	using System.Reflection.Emit;
	
	using Castle.DynamicProxy.Builder.CodeBuilder;
	using Castle.DynamicProxy.Builder.CodeGenerators;
	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;

	using NUnit.Framework;

	[TestFixture]
	public class EasyTypeBuilderTestCase
	{
		ModuleScope module;

		[SetUp]
		public void Init()
		{
			module = new ModuleScope();
		}

		public void RunPEVerify()
		{
			if (false && module.SaveAssembly())
			//if (module.SaveAssembly())
			{
				Process process = new Process();
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.Arguments = ModuleScope.FILE_NAME;
				// Hack. Should it find in the path? 
				// I thought it would.
				process.StartInfo.FileName = @"C:\dev\dotnet\VSNet2003\SDK\v1.1\Bin\PEVerify.exe";
				process.Start();
				process.WaitForExit( Int32.MaxValue );
				if (process.ExitCode != 0)
				{
					Assert.Fail( process.StandardOutput.ReadToEnd() );
				}
			}
		}

		[Test]
		public void CreateSimpleType()
		{
			EasyType typebuilder = new EasyType( module, "mytype" );

			Type newType = typebuilder.BuildType();
			Assert.IsNotNull( newType );
			object instance = Activator.CreateInstance( newType );
			Assert.IsNotNull( instance );
			
			RunPEVerify();
		}

		[Test]
		public void CreateSimpleTypeWithExplicitDefaultConstructor()
		{
			EasyType typebuilder = new EasyType( module, "mytype" );
			typebuilder.CreateDefaultConstructor();

			Type newType = typebuilder.BuildType();
			Assert.IsNotNull( newType );
			object instance = Activator.CreateInstance( newType );
			Assert.IsNotNull( instance );

			RunPEVerify();
		}

		[Test]
		public void CreateSimpleTypeWithConstructor()
		{
			EasyType typebuilder = new EasyType( module, "mytype" );

			ArgumentReference arg1 = new ArgumentReference( typeof(String) );
			ArgumentReference arg2 = new ArgumentReference( typeof(int) );

			typebuilder.CreateConstructor( arg1, arg2 );
		
			Type newType = typebuilder.BuildType();
			Assert.IsNotNull( newType );
			object instance = Activator.CreateInstance( newType, new object[] { "message", 10 } );
			Assert.IsNotNull( instance );

			RunPEVerify();
		}

		[Test]
		public void EmptyMethodReturningVoid()
		{
			EasyType typebuilder = new EasyType( module, "mytype" );

			EasyMethod emptyMethod = typebuilder.CreateMethod( "DoSomething", 
				new ReturnReferenceExpression( typeof(void) ) );

			Type newType = typebuilder.BuildType();
			Assert.IsNotNull( newType );
			object instance = Activator.CreateInstance( newType );
			Assert.IsNotNull( instance );

			MethodInfo method = instance.GetType().GetMethod("DoSomething");
			method.Invoke( instance, new object[0] );

			RunPEVerify();
		}

		[Test]
		public void EmptyMethodReturningInt()
		{
			EasyType typebuilder = new EasyType( module, "mytype" );

			EasyMethod emptyMethod = typebuilder.CreateMethod( "DoSomething", 
				new ReturnReferenceExpression(typeof(int)) );

			Type newType = typebuilder.BuildType();
			Assert.IsNotNull( newType );
			object instance = Activator.CreateInstance( newType );
			Assert.IsNotNull( instance );

			MethodInfo method = instance.GetType().GetMethod("DoSomething");
			Assert.AreEqual( 0, method.Invoke( instance, new object[0] ) );

			RunPEVerify();
		}

		[Test]
		public void MethodCalc()
		{
			EasyType typebuilder = new EasyType( module, "mytype" );

			ArgumentReference arg1 = new ArgumentReference(typeof(int));
			ArgumentReference arg2 = new ArgumentReference(typeof(int));
			ReturnReferenceExpression ret = new ReturnReferenceExpression(typeof(int));

			EasyMethod calcMethod = typebuilder.CreateMethod( "Calc", ret, arg1, arg2 );

			calcMethod.CodeBuilder.AddStatement( 
				new ReturnStatement( 
					new BinaryExpression( BinaryExpression.Add, arg1.ToExpression(), arg2.ToExpression() ) ) );

			Type newType = typebuilder.BuildType();
			Assert.IsNotNull( newType );
			object instance = Activator.CreateInstance( newType );
			Assert.IsNotNull( instance );

			MethodInfo method = instance.GetType().GetMethod("Calc");
			Assert.AreEqual( 2, method.Invoke( instance, new object[] { 1,1 } ) );
			Assert.AreEqual( 5, method.Invoke( instance, new object[] { 3,2 } ) );

			RunPEVerify();
		}

		[Test]
		public void FieldsStoreAndLoad()
		{
			EasyType typebuilder = new EasyType( module, "mytype" );

			FieldReference field1 = typebuilder.CreateField( "field1", typeof(int) );
			FieldReference field2 = typebuilder.CreateField( "field2", typeof(string) );

			{
				ArgumentReference arg1 = new ArgumentReference(typeof(int));
				ArgumentReference arg2 = new ArgumentReference(typeof(string));

				EasyConstructor constr = typebuilder.CreateConstructor( arg1, arg2 );
				constr.CodeBuilder.InvokeBaseConstructor();
				constr.CodeBuilder.AddStatement( new AssignStatement( field1, arg1.ToExpression() ) );
				constr.CodeBuilder.AddStatement( new AssignStatement( field2, arg2.ToExpression() ) );
				constr.CodeBuilder.AddStatement( new ReturnStatement() );
			}

			{
				ReturnReferenceExpression ret1 = new ReturnReferenceExpression( typeof(int) );
				EasyMethod m1 = typebuilder.CreateMethod( "GetField1", ret1 );
				m1.CodeBuilder.AddStatement( new ReturnStatement( field1 ) );

				ReturnReferenceExpression ret2 = new ReturnReferenceExpression( typeof(string) );
				EasyMethod m2 = typebuilder.CreateMethod( "GetField2", ret2 );
				m2.CodeBuilder.AddStatement( new ReturnStatement( field2 ) );
			}

			Type newType = typebuilder.BuildType();
			Assert.IsNotNull( newType );
			object instance = Activator.CreateInstance( newType, new object[] { 10, "hello" } );
			Assert.IsNotNull( instance );

			MethodInfo method1 = instance.GetType().GetMethod("GetField1");
			MethodInfo method2 = instance.GetType().GetMethod("GetField2");
			Assert.AreEqual( 10, method1.Invoke( instance, new object[0] ));
			Assert.AreEqual( "hello", method2.Invoke( instance, new object[0] ));

			RunPEVerify();
		}

		[Test]
		public void MethodInvokingMethod()
		{
			EasyType typebuilder = new EasyType( module, "mytype" );

			ArgumentReference rarg1 = new ArgumentReference(typeof(int));
			ArgumentReference rarg2 = new ArgumentReference(typeof(int));
			ReturnReferenceExpression rret = new ReturnReferenceExpression(typeof(int));
			EasyMethod realCalcMethod = typebuilder.CreateMethod( "RealCalc", rret, rarg1, rarg2 );
			realCalcMethod.CodeBuilder.AddStatement( 
				new ReturnStatement( 
					new BinaryExpression( BinaryExpression.Add, rarg1.ToExpression(), rarg2.ToExpression() ) ) );

			ArgumentReference arg1 = new ArgumentReference(typeof(int));
			ArgumentReference arg2 = new ArgumentReference(typeof(int));
			ReturnReferenceExpression ret = new ReturnReferenceExpression(typeof(int));
			EasyMethod calcMethod = typebuilder.CreateMethod( "Calc", ret, arg1, arg2 );
			calcMethod.CodeBuilder.AddStatement( 
				new ReturnStatement( 
					new MethodInvocationExpression( realCalcMethod, arg1.ToExpression(), arg2.ToExpression() ) ) );

			Type newType = typebuilder.BuildType();
			Assert.IsNotNull( newType );
			object instance = Activator.CreateInstance( newType, new object[0] );
			Assert.IsNotNull( instance );

			MethodInfo method = instance.GetType().GetMethod("Calc");
			Assert.AreEqual( 2, method.Invoke( instance, new object[] { 1,1 } ) );
			Assert.AreEqual( 5, method.Invoke( instance, new object[] { 3,2 } ) );
			method = instance.GetType().GetMethod("RealCalc");
			Assert.AreEqual( 2, method.Invoke( instance, new object[] { 1,1 } ) );
			Assert.AreEqual( 5, method.Invoke( instance, new object[] { 3,2 } ) );

			RunPEVerify();
		}

		[Test]
		public void NewInstanceExpression()
		{
			EasyType typebuilder = new EasyType( module, "mytype" );

			FieldReference cachefield = typebuilder.CreateField( "cache", typeof(ArrayList) );

			EasyConstructor constructor = typebuilder.CreateConstructor( );
			constructor.CodeBuilder.InvokeBaseConstructor();
			constructor.CodeBuilder.AddStatement( new AssignStatement(cachefield, 
				new NewInstanceExpression( typeof(ArrayList), new Type[0] ) ) );
			constructor.CodeBuilder.AddStatement( new ReturnStatement() );

			ReturnReferenceExpression ret = new ReturnReferenceExpression(typeof(ArrayList));
			EasyMethod getCache = typebuilder.CreateMethod( "GetCache", ret );
			getCache.CodeBuilder.AddStatement( new ReturnStatement( cachefield ) );

			Type newType = typebuilder.BuildType();
			Assert.IsNotNull( newType );
			object instance = Activator.CreateInstance( newType, new object[0] );
			Assert.IsNotNull( instance );

			MethodInfo method = instance.GetType().GetMethod("GetCache");
			Assert.IsNotNull( method.Invoke( instance, new object[0] ) );

			RunPEVerify();
		}

		[Test]
		public void BlockWithLock()
		{
			EasyType typebuilder = new EasyType( module, "mytype" );

			FieldReference cachefield = typebuilder.CreateField( "cache", typeof(ArrayList) );

			EasyConstructor constructor = typebuilder.CreateConstructor( );
			constructor.CodeBuilder.InvokeBaseConstructor();

			LockBlockExpression block = new LockBlockExpression( SelfReference.Self );

			block.AddStatement( new AssignStatement(cachefield, 
				new NewInstanceExpression( typeof(ArrayList), new Type[0] ) ) );
			
			constructor.CodeBuilder.AddStatement( new ExpressionStatement(block) );
			constructor.CodeBuilder.AddStatement( new ReturnStatement() );

			ReturnReferenceExpression ret = new ReturnReferenceExpression(typeof(ArrayList));
			EasyMethod getCache = typebuilder.CreateMethod( "GetCache", ret );
			getCache.CodeBuilder.AddStatement( new ReturnStatement( cachefield ) );

			Type newType = typebuilder.BuildType();
			Assert.IsNotNull( newType );
			object instance = Activator.CreateInstance( newType, new object[0] );
			Assert.IsNotNull( instance );

			MethodInfo method = instance.GetType().GetMethod("GetCache");
			Assert.IsNotNull( method.Invoke( instance, new object[0] ) );

			RunPEVerify();
		}

		[Test]
		public void Conditionals()
		{
			EasyType typebuilder = new EasyType( module, "mytype" );

			FieldReference cachefield = typebuilder.CreateField( "cache", typeof(IDictionary) );

			ArgumentReference arg = new ArgumentReference( typeof(bool) );

			EasyConstructor constructor = typebuilder.CreateConstructor( arg );
			constructor.CodeBuilder.InvokeBaseConstructor();
			
			ConditionExpression exp = new ConditionExpression(OpCodes.Brtrue_S, arg.ToExpression());
			exp.AddTrueStatement( new AssignStatement(cachefield, 
				new NewInstanceExpression( typeof(HybridDictionary), new Type[0] ) ) );
			exp.AddFalseStatement( new AssignStatement(cachefield, 
				new NewInstanceExpression( typeof(Hashtable), new Type[0] ) ) );
			
			constructor.CodeBuilder.AddStatement( new ExpressionStatement(exp) );
			constructor.CodeBuilder.AddStatement( new ReturnStatement() );

			ReturnReferenceExpression ret = new ReturnReferenceExpression(typeof(IDictionary));
			EasyMethod getCache = typebuilder.CreateMethod( "GetCache", ret );
			getCache.CodeBuilder.AddStatement( new ReturnStatement( cachefield ) );

			Type newType = typebuilder.BuildType();
			object instance = Activator.CreateInstance( newType, new object[] { true } );
			MethodInfo method = instance.GetType().GetMethod("GetCache");
			object dic = method.Invoke( instance, new object[0] );
			Assert.IsTrue( dic is HybridDictionary );

			instance = Activator.CreateInstance( newType, new object[] { false } );
			dic = method.Invoke( instance, new object[0] );
			Assert.IsTrue( dic is Hashtable );

			RunPEVerify();
		}

		[Test]
		public void ArrayRefs()
		{
			EasyType typebuilder = new EasyType( module, "mytype" );

			FieldReference field1 = typebuilder.CreateField( "field1", typeof(object) );
			FieldReference field2 = typebuilder.CreateField( "field2", typeof(object) );

			ArgumentReference arg = new ArgumentReference( typeof(object[]) );

			EasyConstructor constructor = typebuilder.CreateConstructor( arg );
			constructor.CodeBuilder.InvokeBaseConstructor();

			constructor.CodeBuilder.AddStatement( new AssignStatement(field1, 
				new LoadRefArrayElementExpression( 0, arg ) ) );
			constructor.CodeBuilder.AddStatement( new AssignStatement(field2, 
				new LoadRefArrayElementExpression( 1, arg ) ) );
			
			constructor.CodeBuilder.AddStatement( new ReturnStatement() );

			ReturnReferenceExpression ret1 = new ReturnReferenceExpression(typeof(object));
			EasyMethod getField1 = typebuilder.CreateMethod( "GetField1", ret1 );
			getField1.CodeBuilder.AddStatement( new ReturnStatement( field1 ) );

			ReturnReferenceExpression ret2 = new ReturnReferenceExpression(typeof(object));
			EasyMethod getField2 = typebuilder.CreateMethod( "GetField2", ret2 );
			getField2.CodeBuilder.AddStatement( new ReturnStatement( field2 ) );

			Type newType = typebuilder.BuildType();
			object[] innerArgs = new object[] { "hammett", "verissimo" };
			object instance = Activator.CreateInstance( newType, new object[] { innerArgs } );

			MethodInfo method = instance.GetType().GetMethod("GetField1");
			object result = method.Invoke( instance, new object[0] );
			Assert.AreEqual( "hammett", result );
			
			method = instance.GetType().GetMethod("GetField2");
			result = method.Invoke( instance, new object[0] );
			Assert.AreEqual( "verissimo", result );

			RunPEVerify();
		}

		[Test]
		public void CreateCallable()
		{
			EasyType typebuilder = new EasyType( module, "mytype" );
			EasyCallable callable = typebuilder.CreateCallable( new ReturnReferenceExpression(typeof(string)) );

			FieldReference field1 = typebuilder.CreateField( "field1", callable.TypeBuilder );

			SimpleCallback sc = new SimpleCallback();

			ArgumentReference arg = new ArgumentReference( typeof(SimpleCallback) );
			EasyConstructor constructor = typebuilder.CreateConstructor( arg );
			constructor.CodeBuilder.InvokeBaseConstructor();

			constructor.CodeBuilder.AddStatement( new AssignStatement(field1, 
				new NewInstanceExpression( callable, 
					arg.ToExpression(), 
					new MethodPointerExpression( arg, typeof(SimpleCallback).GetMethod("Run") ) ) ) );
			
			constructor.CodeBuilder.AddStatement( new ReturnStatement() );

			ReturnReferenceExpression ret1 = new ReturnReferenceExpression(typeof(string));
			EasyMethod getField1 = typebuilder.CreateMethod( "Exec", ret1 );
			getField1.CodeBuilder.AddStatement( 
				new ReturnStatement( 
					new ConvertExpression( typeof(String), 
					new MethodInvocationExpression( field1, 
						callable.Callmethod, 
							new ReferencesToObjectArrayExpression() ) ) ) );

			Type newType = typebuilder.BuildType();

			RunPEVerify();
			
			object instance = Activator.CreateInstance( newType, new object[] { sc } );

			MethodInfo method = instance.GetType().GetMethod("Exec");
			object result = method.Invoke( instance, new object[0] );
			Assert.AreEqual( "hello", result );
		}

		[Test]
		public void CreateMoreComplexCallable()
		{
			EasyType typebuilder = new EasyType( module, "mytype" );

			ArgumentReference arg1 = new ArgumentReference( typeof(int) );
			ArgumentReference arg2 = new ArgumentReference( typeof(DateTime) );
			ArgumentReference arg3 = new ArgumentReference( typeof(object) );
			EasyCallable callable = typebuilder.CreateCallable( 
				new ReturnReferenceExpression(typeof(string)), 
				arg1, arg2, arg3 );

			FieldReference field1 = typebuilder.CreateField( "field1", callable.TypeBuilder );

			SimpleCallback sc = new SimpleCallback();

			ArgumentReference arg = new ArgumentReference( typeof(SimpleCallback) );
			EasyConstructor constructor = typebuilder.CreateConstructor( arg );
			constructor.CodeBuilder.InvokeBaseConstructor();

			constructor.CodeBuilder.AddStatement( new AssignStatement(field1, 
				new NewInstanceExpression( callable, 
				arg.ToExpression(), 
				new MethodPointerExpression( arg, typeof(SimpleCallback).GetMethod("RunAs") ) ) ) );
			
			constructor.CodeBuilder.AddStatement( new ReturnStatement() );

			arg1 = new ArgumentReference( typeof(int) );
			arg2 = new ArgumentReference( typeof(DateTime) );
			arg3 = new ArgumentReference( typeof(object) );
			
			ReturnReferenceExpression ret1 = new ReturnReferenceExpression(typeof(string));
			
			EasyMethod getField1 = typebuilder.CreateMethod( "Exec", ret1, arg1, arg2, arg3 );
			getField1.CodeBuilder.AddStatement( 
				new ReturnStatement( 
					new ConvertExpression( typeof(String), 
						new MethodInvocationExpression( field1, 
							callable.Callmethod, 
							new ReferencesToObjectArrayExpression(arg1, arg2, arg3) ) ) ) );

			Type newType = typebuilder.BuildType();

			RunPEVerify();
			
			object instance = Activator.CreateInstance( newType, new object[] { sc } );

			MethodInfo method = instance.GetType().GetMethod("Exec");
			object result = method.Invoke( instance, new object[] { 1, DateTime.Now, "" } );
			Assert.AreEqual( "hello2", result );
		}

		public class SimpleCallback
		{
			public String Run()
			{
				return "hello";
			}

			public String RunAs(int value, DateTime dt, object placeholder)
			{
				return "hello2";
			}
		}
	}
}
