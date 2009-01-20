namespace NVelocity.Test.Extensions
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Text.RegularExpressions;
	using App;
	using NUnit.Framework;
	using NVelocity.App.Events;

	/// <summary>
	/// This class exemplifies an extension to NVelocity rendering, using
	/// the <see cref="EventCartridge"/> to catch the 
	/// <see cref="EventCartridge.ReferenceInsertion"/> event.
	/// </summary>
	[TestFixture]
	public class CustomRenderingTestCase
	{
		private VelocityEngine velocityEngine;
		private VelocityContext velocityContext;

		[SetUp]
		public void Setup()
		{
			velocityEngine = new VelocityEngine();
			velocityEngine.Init();

			// creates the context...
			velocityContext = new VelocityContext();

			// attach a new event cartridge
			velocityContext.AttachEventCartridge(new EventCartridge());

			// add our custom handler to the ReferenceInsertion event
			velocityContext.EventCartridge.ReferenceInsertion += EventCartridge_ReferenceInsertion;
		}

		[Test]
		public void EscapeEscapableSimpleObject()
		{
			velocityContext.Put("escString", new EscapableString("<escape me>"));
			velocityContext.Put("normal", "normal>not<escapable");

			StringWriter sw = new StringWriter();

			Boolean ok = velocityEngine.Evaluate(velocityContext, sw,
			                                     "ExtensionsTest.EscapeEscapableSimpleObject",
			                                     @"$escString | $normal");

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual(@"&lt;escape me&gt; | normal>not<escapable", sw.ToString());
		}

		[Test]
		public void EscapeEscapableComplexObject()
		{
			velocityContext.Put("escComplex", new EscapableComplexObject("my>name", "my&value"));
			velocityContext.Put("normal", "normal>not<escapable");

			StringWriter sw = new StringWriter();

			Boolean ok = velocityEngine.Evaluate(velocityContext, sw,
			                                     "ExtensionsTest.EscapeEscapableComplexObject",
			                                     @"$escComplex.name $escComplex.value | $normal");

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual(@"my&gt;name my&amp;value | normal>not<escapable", sw.ToString());
		}

		[Test]
		public void EscapeEscapableComplexMixedObject()
		{
			// adds some objects, escapable and not escapable
			velocityContext.Put("escMixed", new EscapableComplexObject("escape&me", new NotEscapableString("don't &escape> me")));
			velocityContext.Put("normal", "normal>not<escapable");

			StringWriter sw = new StringWriter();

			Boolean ok = velocityEngine.Evaluate(velocityContext, sw,
			                                     "ExtensionsTest.EscapeEscapableComplexMixedObject",
			                                     @"$escMixed.name $escMixed.value | $normal");

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual(@"escape&amp;me don't &escape> me | normal>not<escapable", sw.ToString());
		}

		[Test]
		public void ReplaceSingleItem()
		{
			velocityContext.Put("singleItem", "text");

			StringWriter sw = new StringWriter();

			bool ok = velocityEngine.Evaluate(velocityContext, sw, "", @"$singleItem");

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual(@"New single item", sw.ToString());
		}

		[Test]
		public void ReplaceMultipleItems()
		{
			velocityContext.Put("multipleItems", new string[] {"text", "text"});

			StringWriter sw = new StringWriter();

			bool ok = velocityEngine.Evaluate(velocityContext, sw, "",
				"#foreach($item in $multipleItems)\n$item\n#end");

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual("Item 1\nItem 2\n", sw.ToString());
		}

		/// <summary>
		/// This is a sample of an ReferenceInsertion handler that escapes objects into
		/// XML strings. What matters for this handler is the topmost "escapable" or
		/// "not escapable" specification.
		/// </summary>
		private void EventCartridge_ReferenceInsertion(object sender, ReferenceInsertionEventArgs e)
		{
			Stack rs = e.GetCopyOfReferenceStack();
			while(rs.Count > 0)
			{
				Object current = rs.Pop();
				if (current is INotEscapable)
					return;

				if (current is IEscapable)
				{
					e.NewValue = Regex.Replace(e.OriginalValue.ToString(), "[&<>\"]", new MatchEvaluator(Escaper));
					return;
				}
			}

			if (e.RootString == "$multipleItems")
			{
				e.NewValue = new string[] {"Item 1", "Item 2"};
			}
			else if (e.RootString == "$singleItem")
			{
				e.NewValue = "New single item";
			}
		}

		private string Escaper(Match m)
		{
			switch(m.Value)
			{
				case "&":
					return "&amp;";
				case "<":
					return "&lt;";
				case ">":
					return "&gt;";
				case "\"":
					return "&quot;";
				default:
					return m.Value;
			}
		}

		#region IEscapable, INotEscapable and sample objects

		public interface IEscapable
		{
		}

		public interface INotEscapable
		{
		}

		public class EscapableString : IEscapable
		{
			private String value;

			public EscapableString(String value)
			{
				this.value = value;
			}

			public override string ToString()
			{
				return value;
			}
		}

		public class NotEscapableString : INotEscapable
		{
			private String value;

			public NotEscapableString(String value)
			{
				this.value = value;
			}

			public override string ToString()
			{
				return value;
			}
		}

		public class EscapableComplexObject : IEscapable
		{
			private Object name;
			private Object value;

			public EscapableComplexObject(Object name, Object value)
			{
				this.name = name;
				this.value = value;
			}

			public Object Name
			{
				get { return name; }
			}

			public Object Value
			{
				get { return value; }
			}
		}

		#endregion
	}
}