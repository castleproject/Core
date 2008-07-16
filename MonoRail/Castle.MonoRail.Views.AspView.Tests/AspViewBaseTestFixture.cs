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

namespace Castle.MonoRail.Views.AspView.Tests
{
	using System.Web;
	using NUnit.Framework;
	using System.IO;

	[TestFixture]
	public class AspViewBaseTestFixture
	{
		[Test]
		public void Output_NullString_OutputsEmpty()
		{
			TestableView view = new TestableView();

			string input = null;

			string expected = string.Empty;

			string actual = GetFrom(view, delegate { view.Output(input); });

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Output_NullObject_OutputsEmpty()
		{
			TestableView view = new TestableView();

			object input = null;

			string expected = string.Empty;

			string actual = GetFrom(view, delegate { view.Output(input); });

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Output_String_OutputsTheString()
		{
			TestableView view = new TestableView();

			string input = "<a href='ken'>egozi</a>";

			string expected = "<a href='ken'>egozi</a>";

			string actual = GetFrom(view, delegate { view.Output(input); });

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Output_Object_OutputsToString()
		{
			TestableView view = new TestableView();

			SillyString input = new SillyString("<a href='ken'>egozi</a>");

			string expected = "<a href='ken'>egozi</a>";

			string actual = GetFrom(view, delegate { view.Output(input); });

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void OutputEncoded_NullString_OutputsEmpty()
		{
			TestableView view = new TestableView();

			string input = null;

			string expected = string.Empty;

			string actual = GetFrom(view, delegate { view.OutputEncoded(input); });

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void OutputEncoded_NullObject_OutputsEmpty()
		{
			TestableView view = new TestableView();

			object input = null;

			string expected = string.Empty;

			string actual = GetFrom(view, delegate { view.OutputEncoded(input); });

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void OutputEncoded_String_OutputsTheStringEncoded()
		{
			TestableView view = new TestableView();

			string input = "<a href='ken'>egozi</a>";

			string expected = HttpUtility.HtmlEncode(input).Replace("'", "&#39;");

			string actual = GetFrom(view, delegate { view.OutputEncoded(input); });

			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void OutputEncoded_Object_OutputsToStringEncoded()
		{
			TestableView view = new TestableView();

			string input = "<a href='ken'>egozi</a>";

			SillyString inputAsObject = new SillyString(input);

			string expected = HttpUtility.HtmlEncode(input).Replace("'", "&#39;");

			string actual = GetFrom(view, delegate { view.OutputEncoded(inputAsObject); });

			Assert.AreEqual(expected, actual);
		}

		#region helper
		private delegate void OutputMethod();
		private static string GetFrom(IViewBaseInternal view, OutputMethod output)
		{
			string actual;
			using (StringWriter writer = new StringWriter())
			using (view.SetDisposeableOutputWriter(writer))
			{
				output.Invoke();
				actual = writer.GetStringBuilder().ToString();
			}
			return actual;
		}

		class TestableView : AspViewBase
		{
			public new void Output(object message)
			{
				base.Output(message);
			}

			public new void Output(string message)
			{
				base.Output(message);
			}

			public new void OutputEncoded(object fragment)
			{
				base.OutputEncoded(fragment);
			}

			public new void OutputEncoded(string fragment)
			{
				base.OutputEncoded(fragment);
			}

			public override void Render()
			{
				throw new System.Exception("The method or operation is not implemented.");
			}
			protected override string ViewDirectory
			{
				get { throw new System.Exception("The method or operation is not implemented."); }
			}
			protected override string ViewName
			{
				get { throw new System.Exception("The method or operation is not implemented."); }
			}
		}

		class SillyString
		{
			readonly string value;
			public SillyString(string value)
			{
				this.value = value;
			}
			public override string ToString()
			{
				return value;
			}
		}
		#endregion
	}
}
