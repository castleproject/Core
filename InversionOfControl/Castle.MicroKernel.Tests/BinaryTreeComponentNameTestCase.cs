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

namespace Castle.MicroKernel.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.Model;

	using Castle.MicroKernel.Handlers;
	using Castle.MicroKernel.SubSystems.Naming;

	[TestFixture]
	public class BinaryTreeComponentNameTestCase
	{
		[Test]
		public void Usage()
		{
			BinaryTreeComponentName tree = new BinaryTreeComponentName();
			
			DefaultHandler handler1 = new DefaultHandler( 
				new ComponentModel("A", typeof(DefaultHandler), typeof(DefaultHandler) ) );
			DefaultHandler handler2 = new DefaultHandler( 
				new ComponentModel("B", typeof(DefaultHandler), typeof(DefaultHandler) ) );
			DefaultHandler handler3 = new DefaultHandler( 
				new ComponentModel("C", typeof(DefaultHandler), typeof(DefaultHandler) ) );
			DefaultHandler handler4 = new DefaultHandler( 
				new ComponentModel("D", typeof(DefaultHandler), typeof(DefaultHandler) ) );
			DefaultHandler handler5 = new DefaultHandler( 
				new ComponentModel("E", typeof(DefaultHandler), typeof(DefaultHandler) ) );
			DefaultHandler handler6 = new DefaultHandler( 
				new ComponentModel("F", typeof(DefaultHandler), typeof(DefaultHandler) ) );

			tree.Add( new ComponentName("protocolhandler"), handler1 );
			tree.Add( new ComponentName("protocolhandler:key=1"), handler2 );
			tree.Add( new ComponentName("protocolhandler:key=2"), handler3 );
			tree.Add( new ComponentName("protocolhandler:key=2,secure=true"), handler4 );
			tree.Add( new ComponentName("modelmanager"), handler5 );
			tree.Add( new ComponentName("viewmanager"), handler6 );

			Assert.AreSame( handler1, tree.GetHandler( new ComponentName("protocolhandler") ) );
			Assert.AreSame( handler2, tree.GetHandler( new ComponentName("protocolhandler:key=1") ) );
			Assert.AreSame( handler3, tree.GetHandler( new ComponentName("protocolhandler:key=2") ) );
			Assert.AreSame( handler4, tree.GetHandler( new ComponentName("protocolhandler:key=2,secure=true") ) );
			Assert.AreSame( handler5, tree.GetHandler( new ComponentName("modelmanager") ) );
			Assert.AreSame( handler6, tree.GetHandler( new ComponentName("viewmanager") ) );

			IHandler[] handlers = tree.GetHandlers( new ComponentName("protocolhandler") );

			Assert.AreEqual( 4, handlers.Length );
			Assert.AreSame( handler1, handlers[0] );
			Assert.AreSame( handler2, handlers[1] );
			Assert.AreSame( handler3, handlers[2] );
			Assert.AreSame( handler4, handlers[3] );

			handlers = tree.GetHandlers( new ComponentName("protocolhandler:*") );

			Assert.AreEqual( 4, handlers.Length );
			Assert.AreSame( handler1, handlers[0] );
			Assert.AreSame( handler2, handlers[1] );
			Assert.AreSame( handler3, handlers[2] );
			Assert.AreSame( handler4, handlers[3] );

			handlers = tree.GetHandlers( new ComponentName("protocolhandler:secure=true") );

			Assert.AreEqual( 1, handlers.Length );
			Assert.AreSame( handler4, handlers[0] );

			handlers = tree.GetHandlers( new ComponentName("protocolhandler:key=2") );

			Assert.AreEqual( 2, handlers.Length );
			Assert.AreSame( handler3, handlers[0] );
			Assert.AreSame( handler4, handlers[1] );
		}
	}
}
