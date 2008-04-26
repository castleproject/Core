// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.TransformFilters
{
	using System.IO;
	using System.Text;
	using Castle.MonoRail.Framework.TransformFilters;
	using NUnit.Framework;

	[TestFixture]
	public class TransformFilterTestCase
	{
		[Test]
		public void UpperCaseTransformFilter_Works_Fine()
		{
			MemoryStream stream = new MemoryStream();
			UpperCaseTransformFilter filter = new UpperCaseTransformFilter(stream);

			byte[] bytes = Encoding.ASCII.GetBytes("this should be in uppercase");
			filter.Write(bytes, 0, bytes.Length);

			string transformed = Encoding.ASCII.GetString(stream.ToArray());
			Assert.AreEqual("THIS SHOULD BE IN UPPERCASE", transformed);

		}

		[Test]
		public void LowerCaseTransformFilter_Works_Fine()
		{
			MemoryStream stream = new MemoryStream();
			LowerCaseTransformFilter filter = new LowerCaseTransformFilter(stream);

			byte[] bytes = Encoding.ASCII.GetBytes("THIS SHOULD BE IN LOWERCASE");
			filter.Write(bytes, 0, bytes.Length);

			string transformed = Encoding.ASCII.GetString(stream.ToArray());
			Assert.AreEqual("this should be in lowercase", transformed);

		}

		[Test]
		public void WikiTransformFilter_Works_Fine()
		{
			MemoryStream stream = new MemoryStream();
			WikiTransformFilter filter = new WikiTransformFilter(stream);

			byte[] bytes = Encoding.ASCII.GetBytes("__Underlined text__");
			filter.Write(bytes, 0, bytes.Length);

			string transformed = Encoding.ASCII.GetString(stream.ToArray());
			Assert.AreEqual("<u>Underlined text</u>\n", transformed);

		}
	}
}
