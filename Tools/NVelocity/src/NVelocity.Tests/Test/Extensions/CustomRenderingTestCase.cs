namespace NVelocity.Test.Extensions
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Specialized;

	using NUnit.Framework;
	using NVelocity.App;
	using NVelocity.App.Events;
	using System.Text.RegularExpressions;

	/// <summary>
	/// This class exemplifies an extension to NVelocity rendering, using
	/// the <see cref="EventCartridge"/> to catch the 
	/// <see cref="EventCartridge.ReferenceInsertion"/> event.
	/// </summary>
	[TestFixture]
	public class CustomRenderingTestCase
	{
		VelocityEngine ve;
		VelocityContext c;

		[SetUp]
		public void Setup()
		{
			ve = new VelocityEngine();
			ve.Init();
			
			// creates the context...
			c = new VelocityContext();

			// attach a new event cartridge
			c.AttachEventCartridge(new EventCartridge());

			// add our custom handler to the ReferenceInsertion event
			c.EventCartridge.ReferenceInsertion +=
				new ReferenceInsertionEventHandler(EventCartridge_ReferenceInsertion);
		}
		
		[Test]
		public void EscapeEscapableSimpleObject()
		{
			c.Put("escString", new EscapableString("<escape me>"));
			c.Put("normal", "normal>not<escapable");

			StringWriter sw = new StringWriter();

			Boolean ok = ve.Evaluate(c, sw,
				"ExtensionsTest.EscapeEscapableSimpleObject",
				@"$escString | $normal");

			Assert.IsTrue(ok, "Evalutation returned failure");
			Assert.AreEqual(@"&lt;escape me&gt; | normal>not<escapable", sw.ToString());
		}

		[Test]
		public void EscapeEscapableComplexObject()
		{
			c.Put("escComplex", new EscapableComplexObject("my>name", "my&value"));
			c.Put("normal", "normal>not<escapable");

			StringWriter sw = new StringWriter();

			Boolean ok = ve.Evaluate(c, sw,
				"ExtensionsTest.EscapeEscapableComplexObject",
				@"$escComplex.name $escComplex.value | $normal");

			Assert.IsTrue(ok, "Evalutation returned failure");
			Assert.AreEqual(@"my&gt;name my&amp;value | normal>not<escapable", sw.ToString());
		}

		[Test]
		public void EscapeEscapableComplexMixedObject()
		{
			// adds some objects, escapable and not escapable
			c.Put("escMixed", new EscapableComplexObject("escape&me", new NotEscapableString("don't &escape> me")));
			c.Put("normal", "normal>not<escapable");

			StringWriter sw = new StringWriter();

			Boolean ok = ve.Evaluate(c, sw,
				"ExtensionsTest.EscapeEscapableComplexMixedObject",
				@"$escMixed.name $escMixed.value | $normal");

			Assert.IsTrue(ok, "Evalutation returned failure");
			Assert.AreEqual(@"escape&amp;me don't &escape> me | normal>not<escapable", sw.ToString());
		}

		/// <summary>
		/// This is a sample of an ReferenceInsertion handler that escapes objects into
		/// XML strings. What matters for this handler is the topmost "escapable" or
		/// "not escapable" specification.
		/// </summary>
		private void EventCartridge_ReferenceInsertion(object sender, ReferenceInsertionEventArgs e)
		{
			Stack rs = e.GetCopyOfReferenceStack();
			while (rs.Count > 0)
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
		}

		private string Escaper(Match m)
		{
			switch (m.Value)
			{
				case "&": return "&amp;";
				case "<": return "&lt;";
				case ">": return "&gt;";
				case "\"": return "&quot;";
				default: return m.Value;
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
			String value;

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
			String value;

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
			Object name, value;

			public EscapableComplexObject(Object name, Object value)
			{
				this.name = name;
				this.value = value;
			}

			public Object Name { get { return this.name; } }
			public Object Value { get { return this.value; } }
		}
		#endregion
	}
}
