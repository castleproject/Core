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

namespace Castle.CastleOnRails.Framework.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.CastleOnRails.Framework.Internal.Graph;

	/// <summary>
	/// Summary description for ControllerTreeTestCase.
	/// </summary>
	[TestFixture]
	public class ControllerTreeTestCase
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InvalidConstruction()
		{
			new ControllerTree(null);
		}

		[Test]
		public void EmptyArea()
		{
			ControllerTree tree = new ControllerTree();
			tree.AddController("", "home", "controller1");
			tree.AddController("", "contact", "controller2");
			tree.AddController("", "cart", "controller3");

			Assert.AreEqual( "controller1", tree.GetController("", "home") );
			Assert.AreEqual( "controller2", tree.GetController("", "contact") );
			Assert.AreEqual( "controller3", tree.GetController("", "cart") );
		}

		[Test]
		public void FewAreas()
		{
			ControllerTree tree = new ControllerTree();
			tree.AddController("", "home", "controller1");
			tree.AddController("", "contact", "controller2");
			tree.AddController("", "cart", "controller3");
			tree.AddController("clients", "home", "controller11");
			tree.AddController("clients", "contact", "controller21");
			tree.AddController("clients", "cart", "controller31");
			tree.AddController("lists", "home", "controller12");
			tree.AddController("lists", "contact", "controller22");
			tree.AddController("lists", "cart", "controller32");

			Assert.AreEqual( "controller1", tree.GetController("", "home") );
			Assert.AreEqual( "controller2", tree.GetController("", "contact") );
			Assert.AreEqual( "controller3", tree.GetController("", "cart") );

			Assert.AreEqual( "controller11", tree.GetController("clients", "home") );
			Assert.AreEqual( "controller21", tree.GetController("clients", "contact") );
			Assert.AreEqual( "controller31", tree.GetController("clients", "cart") );

			Assert.AreEqual( "controller12", tree.GetController("lists", "home") );
			Assert.AreEqual( "controller22", tree.GetController("lists", "contact") );
			Assert.AreEqual( "controller32", tree.GetController("lists", "cart") );
		}
	}
}
