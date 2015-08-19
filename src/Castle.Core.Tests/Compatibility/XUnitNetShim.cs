// Copyright 2004-2015 Castle Project - http://www.castleproject.org/
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

#if FEATURE_XUNITNET

// Don't run unit tests in parallel
[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]

// NUnit Shim to run on xUnit.net
namespace NUnit.Framework
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;

	// Pretend to be a Fact attribute
	[Xunit.Sdk.XunitTestCaseDiscoverer("Castle.Core.Tests.XunitExtensions.CustomFactDiscoverer", "Castle.Core.Tests")]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class TestAttribute : Xunit.FactAttribute
	{
		public string Description { get; set; }
	}

	// Does nothing, just to make it compile
	public class TestFixtureAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	[Xunit.Sdk.DataDiscoverer("Xunit.Sdk.MemberDataDiscoverer", "xunit.core")]
	public class TestCaseSourceAttribute : Xunit.MemberDataAttributeBase
	{
		public TestCaseSourceAttribute(string memberName, params object[] parameters)
			: base(memberName, parameters)
		{
		}

		protected override object[] ConvertDataItem(MethodInfo testMethod, object item)
		{
			if (item == null) return null;

			var array = item as object[];
			if (array == null)
			{
				throw new ArgumentException(string.Format("Property {0} on {1} yielded an item that is not an object[]",
					MemberName, MemberType ?? testMethod.DeclaringType));
			}

			return array;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class IgnoreAttribute : Attribute
	{
		public IgnoreAttribute(string reason)
		{
			Reason = reason;
		}

		public string Reason { get; set; }
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class ExplicitAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class PlatformAttribute : Attribute
	{
		public string Exclude { get; set; }
		public string Reason { get; set; }
	}

	public class IgnoreException : Exception
	{
		public string Reason { get; set; }

		public IgnoreException(string reason)
		{
			Reason = reason;
		}
	}

	//TODO: Couldn't work out a way to simulate the following attributes:
	//      TestFixtureSetUp, TestFixtureTearDown
	//      SetUp, TearDown

	// Shim assertions
	public static class Assert
	{
		// Assert.cs

		public static void Fail()
		{
			Xunit.Assert.True(false);
		}

		public static void Fail(string userMessage, params object[] args)
		{
			Xunit.Assert.True(false, string.Format(userMessage, args));
		}

		public static void Ignore(string userMessage)
		{
			throw new IgnoreException(userMessage);
		}

		public static void Contains(object expected, IEnumerable<object> collection)
		{
			Xunit.Assert.Contains(expected, collection);
		}

		// Assert.Comparisons.cs

		//TODO: Greater
		//TODO: Less
		//TODO: GreaterOrEqual
		//TODO: LessOrEqual

		// Assert.Conditions.cs

		public static void True(bool condition, string userMessage = null, params object[] args)
		{
			if (userMessage != null)
			{
				Xunit.Assert.True(condition, string.Format(userMessage, args));
			}
			else
			{
				Xunit.Assert.True(condition);
			}
		}

		public static void IsTrue(bool condition, string userMessage = null, params object[] args)
		{
			True(condition, userMessage, args);
		}

		public static void False(bool condition, string userMessage = null, params object[] args)
		{
			if (userMessage != null)
			{
				Xunit.Assert.False(condition, string.Format(userMessage, args));
			}
			else
			{
				Xunit.Assert.False(condition);
			}
		}

		public static void IsFalse(bool condition, string userMessage = null, params object[] args)
		{
			False(condition, userMessage, args);
		}

		public static void NotNull(object @object)
		{
			Xunit.Assert.NotNull(@object);
		}

		public static void IsNotNull(object @object)
		{
			Xunit.Assert.NotNull(@object);
		}

		public static void IsNull(object @object)
		{
			Xunit.Assert.Null(@object);
		}

		public static void IsEmpty(IEnumerable collection)
		{
			Xunit.Assert.Empty(collection);
		}

		public static void IsNotEmpty(IEnumerable collection)
		{
			Xunit.Assert.NotEmpty(collection);
		}

		// Assert.Equality.cs

		public static void AreEqual(object expected, object actual, string userMessage = null)
		{
			Xunit.Assert.Equal(expected, actual);
		}

		public static void AreNotEqual(object expected, object actual, string userMessage = null)
		{
			Xunit.Assert.NotEqual(expected, actual);
		}

		public static void AreSame(object expected, object actual, string userMessage = null)
		{
			Xunit.Assert.Same(expected, actual);
		}

		public static void AreNotSame(object expected, object actual, string userMessage = null)
		{
			Xunit.Assert.NotSame(expected, actual);
		}

		// Assert.Exceptions.cs

		public static Exception Throws(Type exceptionType, Action testCode)
		{
			return Xunit.Assert.Throws(exceptionType, testCode);
		}

		public static T Throws<T>(Action testCode) where T : Exception
		{
			return Xunit.Assert.Throws<T>(testCode);
		}

		public static void DoesNotThrow(Action testCode)
		{
			testCode();
		}

		// Assert.That.cs

		// Assert.Types.cs

		//TODO: IsAssignableFrom
		//TODO: IsNotAssignableFrom

		public static void IsInstanceOf(Type expectedType, object @object)
		{
			Xunit.Assert.IsAssignableFrom(expectedType, @object);
		}

		public static void IsInstanceOf<T>(object @object)
		{
			Xunit.Assert.IsAssignableFrom<T>(@object);
		}

		//TODO: IsNotInstanceOf

		public static void IsNotInstanceOf<T>(object @object)
		{
			Xunit.Assert.False(@object is T);
		}
	}

	public static class StringAssert
	{
		public static void Contains(string expectedSubstring, string actualString)
		{
			Xunit.Assert.Contains(expectedSubstring, actualString);
		}

		public static void DoesNotContain(string expectedSubstring, string actualString)
		{
			Xunit.Assert.DoesNotContain(expectedSubstring, actualString);
		}

		public static void StartsWith(string expectedStartString, string actualString)
		{
			Xunit.Assert.StartsWith(expectedStartString, actualString);
		}

		//TODO: DoesNotStartWith
		//TODO: EndsWith
		//TODO: DoesNotEndWith
		//TODO: AreEqualIgnoringCase
		//TODO: AreNotEqualIgnoringCase
		//TODO: IsMatch
		//TODO: DoesNotMatch
	}

	public static class CollectionAssert
	{
		//TODO: AllItemsAreInstancesOfType
		//TODO: AllItemsAreNotNull
		//TODO: AllItemsAreUnique

		public static void AreEqual(IEnumerable expected, IEnumerable actual)
		{
			Xunit.Assert.Equal(expected, actual);
		}

		public static void AreEquivalent(IEnumerable expected, IEnumerable actual)
		{
			Xunit.Assert.NotStrictEqual(expected, actual);
		}

		//TODO: AreNotEqual
		//TODO: AreNotEquivalent

		public static void Contains<T>(IEnumerable<T> collection, T expected)
		{
			Xunit.Assert.Contains(expected, collection);
		}

		public static void DoesNotContain<T>(IEnumerable<T> collection, T expected)
		{
			Xunit.Assert.DoesNotContain(expected, collection);
		}

		//TODO: IsNotSubsetOf
		//TODO: IsSubsetOf

		public static void IsEmpty(IEnumerable collection)
		{
			Xunit.Assert.Empty(collection);
		}

		//TODO: IsEmpty
		//TODO: IsNotEmpty
		//TODO: IsOrdered
	}
}

namespace Castle.Core.Tests.XunitExtensions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using NUnit.Framework; // the shim classes above

	using Xunit.Abstractions;
	using Xunit.Sdk;

	public class CustomFactDiscoverer : IXunitTestCaseDiscoverer
	{
		private readonly IMessageSink diagnosticMessageSink;

		public CustomFactDiscoverer(IMessageSink diagnosticMessageSink)
		{
			this.diagnosticMessageSink = diagnosticMessageSink;
		}

		public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions,
			ITestMethod testMethod, IAttributeInfo factAttribute)
		{
			yield return new CustomTestCase(diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod);
		}
	}

	public class CustomTestCase : XunitTestCase
	{
		[Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
		public CustomTestCase()
		{
		}

		public CustomTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay,
			ITestMethod testMethod, object[] testMethodArguments = null)
			: base(diagnosticMessageSink, defaultMethodDisplay, testMethod, testMethodArguments)
		{
		}

		protected override void Initialize()
		{
			base.Initialize();

			// Skip if the test is ignored
			if (SkipReason == null)
			{
				var ignoreAttr = TestMethod.Method.GetCustomAttributes(typeof(IgnoreAttribute)).FirstOrDefault();
				if (ignoreAttr != null)
				{
					SkipReason = ignoreAttr.GetNamedArgument<string>("Reason");
				}
			}

			// Skip if the test is "explicit"
			if (SkipReason == null)
			{
				if (TestMethod.Method.GetCustomAttributes(typeof(ExplicitAttribute)).Any())
				{
					SkipReason = "Explicit";
				}
			}

			// Skip if we are running on an unsupported platform
			if (SkipReason == null)
			{
				var platformAttr = TestMethod.Method.GetCustomAttributes(typeof(PlatformAttribute)).FirstOrDefault();
				if (platformAttr != null &&
					platformAttr.GetNamedArgument<string>("Exclude") == "mono" &&
					Type.GetType("Mono.Runtime") != null)
				{
					SkipReason = platformAttr.GetNamedArgument<string>("Reason");
				}
			}
		}

		public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus,
			object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
		{
			// Run the test
			var wrappedMessageBus = new CustomMessageBus(messageBus);
			var result = await base.RunAsync(diagnosticMessageSink, wrappedMessageBus, constructorArguments, aggregator, cancellationTokenSource);

			// Adjust the failed & skipped counts if the test threw an IgnoreException
			if (wrappedMessageBus.DynamicallySkippedTestCount > 0)
			{
				result.Failed -= wrappedMessageBus.DynamicallySkippedTestCount;
				result.Skipped += wrappedMessageBus.DynamicallySkippedTestCount;
			}

			return result;
		}
	}

	public class CustomMessageBus : IMessageBus
	{
		private readonly IMessageBus innerBus;

		public CustomMessageBus(IMessageBus innerBus)
		{
			this.innerBus = innerBus;
		}

		public int DynamicallySkippedTestCount { get; private set; }

		public bool QueueMessage(IMessageSinkMessage message)
		{
			// Handle if the test threw an IgnoreException
			var testFailed = message as ITestFailed;
			if (testFailed != null)
			{
				var exceptionType = testFailed.ExceptionTypes.FirstOrDefault();
				if (exceptionType == typeof(IgnoreException).FullName)
				{
					DynamicallySkippedTestCount++;
					return innerBus.QueueMessage(new TestSkipped(testFailed.Test, testFailed.Messages.FirstOrDefault()));
				}
			}

			// Nothing we care about, send it on its way
			return innerBus.QueueMessage(message);
		}

		public void Dispose()
		{
		}
	}
}

#endif