// Copyright 2003-2004 The Apache Software Foundation
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

namespace Apache.Avalon.Framework.Test
{
	using System;
	using System.Text;
	using NUnit.Framework;

	using Apache.Avalon.Framework;


	/// <summary>
	/// Summary description for ContextTestCase.
	/// </summary>
	[TestFixture]
	public class ContextTestCase
	{
		private class ResolvableString : IResolvable
		{
			private string m_content;

			public ResolvableString( string content )
			{
				this.m_content = content;
			}

			public ResolvableString() : this( "This is a ${test}." )
			{
			}
		
			#region IResolvable Members

			public object Resolve(IContext context)
			{
				int index = this.m_content.IndexOf( "${" );
				if ( index < 0 )
				{
					return this.m_content;
				}

				StringBuilder buf = new StringBuilder( this.m_content.Substring( 0, index ) );

				while ( index >= 0 && index <= this.m_content.Length )
				{
					index += 2;
					int end = this.m_content.IndexOf( "}", index);

					if ( end < 0 )
					{
						end = this.m_content.Length;
					}

					buf.Append( context[ this.m_content.Substring( index, end - index ) ] );
					end++;

					index = this.m_content.IndexOf( "${", end ) + 2;

					if ( index < 2 )
					{
						index = -1;
						buf.Append( this.m_content.Substring( end, this.m_content.Length - end ) );
					}

					if ( index >=0 && index <= this.m_content.Length )
					{
						buf.Append( this.m_content.Substring( end, index - end ) );
					}
				}

				return buf.ToString();
			}

			#endregion
		}

		[Test]
		public void AddContext()
		{
			DefaultContext context = new DefaultContext();
			context.Put( "key1", "value1" );
			Assertion.Assert( "value1".Equals( context["key1"] ) );
			context.Put( "key1", String.Empty );
			Assertion.Assert( String.Empty.Equals( context["key1"] ) );

			context.Put( "key1", "value1" );
			context.MakeReadOnly();

			try
			{
				context.Put( "key1", String.Empty );
				Assertion.Fail( "You are not allowed to change a value after it has been made read only" );
			}
			catch ( ContextException )
			{
				Assertion.Assert( "Value is null", "value1".Equals( context["key1"] ) );
			}
		}

		[Test]
		public void ResolveableObject()
		{
			DefaultContext context = new DefaultContext();
			context.Put( "key1", new ResolvableString() );
			context.Put( "test", "Cool Test" );
			context.MakeReadOnly();

			IContext newContext = (IContext) context;
			Assertion.Assert( "Cool Test".Equals( newContext["test"] ) );
			Assertion.Assert( ! "This is a ${test}.".Equals( newContext["key1"] ) );
			Assertion.Assert( "This is a Cool Test.".Equals( newContext["key1"] ) );
		}

		[Test]
		public void CascadingContext()
		{
			DefaultContext parent = new DefaultContext();
			parent.Put( "test", "ok test" );
			parent.MakeReadOnly();
			DefaultContext child = new DefaultContext( parent );
			child.Put( "check", new ResolvableString("This is an ${test}.") );
			child.MakeReadOnly();
			IContext context = (IContext) child;

			Assertion.Assert ( "ok test".Equals( context["test"] ) );
			Assertion.Assert ( ! "This is an ${test}.".Equals( context["check"] ) );
			Assertion.Assert ( "This is an ok test.".Equals( context["check"] ) );
		}

		[Test]
		public void HiddenItems()
		{
			DefaultContext parent = new DefaultContext();
			parent.Put( "test", "test" );
			parent.MakeReadOnly();
			DefaultContext child = new DefaultContext( parent );
			child.Put( "check", "check" );
			IContext context = (IContext) child;
	        
			Assertion.Assert ( "check".Equals( context["check"] ) );
			Assertion.Assert ( "test".Equals( context["test"] ) );
	                
			child.Hide( "test" );
			try 
			{
				object o = context["test"];
				Assertion.Fail( "The item \"test\" was hidden in the child context, but could still be retrieved via Get()." );
			}
			catch (ContextException)
			{
				// Supposed to be thrown.
			}
	        
			child.MakeReadOnly();
	        
			try 
			{
				child.Hide( "test" );
				Assertion.Fail( "Hide() did not throw an exception, even though the context is supposed to be read-only." );
			}
			catch (ContextException)
			{
				// Supposed to be thrown.
			}
		}

	}
}
