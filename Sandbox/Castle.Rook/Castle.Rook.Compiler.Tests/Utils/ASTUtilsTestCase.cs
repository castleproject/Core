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

namespace Castle.Rook.Compiler.Tests.Utils
{
	using System;

	using Castle.Rook.Compiler.AST.Util;
	
	using NUnit.Framework;

	[TestFixture]
	public class ASTUtilsTestCase
	{
		[Test]
		public void CollectMethodInformation()
		{
			String boundTo;
			String name;
			bool isStatic;

			ASTUtils.CollectMethodInformation("simple_name", out boundTo, out name, out isStatic);

			Assert.AreEqual(null, boundTo);
			Assert.AreEqual("simple_name", name);
			Assert.IsFalse(isStatic);

			ASTUtils.CollectMethodInformation("self.simple_name", out boundTo, out name, out isStatic);

			Assert.AreEqual(null, boundTo);
			Assert.AreEqual("simple_name", name);
			Assert.IsTrue(isStatic);

			ASTUtils.CollectMethodInformation("IList.Add", out boundTo, out name, out isStatic);

			Assert.AreEqual("IList", boundTo);
			Assert.AreEqual("Add", name);
			Assert.IsFalse(isStatic);

			ASTUtils.CollectMethodInformation("System::Collection::IList.Add", out boundTo, out name, out isStatic);

			Assert.AreEqual("System::Collection::IList", boundTo);
			Assert.AreEqual("Add", name);
			Assert.IsFalse(isStatic);
		}
	}
}
