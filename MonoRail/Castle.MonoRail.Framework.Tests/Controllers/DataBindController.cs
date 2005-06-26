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

namespace Castle.MonoRail.Framework.Tests.Controllers
{
	using System;

	using NUnit.Framework;

	using Castle.MonoRail.Framework.Attributes;


	public class DataBindController : SmartDispatcherController
	{
		public static string Text = "this is some bloody text";
		public static int Value = new Random().Next();
		public static DateTime Date = DateTime.Now;
		
		public const string FormPrefix = "myooo";

		public void MapNoPrefix( [DataBind] BindObject instance )
		{
			Validate( instance );
		}

		public void MapWithPrefix( [DataBind(Prefix=FormPrefix)] BindObject instance )
		{
			Validate( instance );
		}

		public void MapFromQueryGood( [DataBind(From=ParamStore.QueryString)] BindObject instance )
		{
			Validate( instance );
		}

		public void MapFromQueryBad( [DataBind(From=ParamStore.Form)] BindObject instance )
		{
			Validate( instance );
		}

		public void MapWithErrors( [DataBind] BindObject instance )
		{
			ErrorList errors = GetDataBindErrors( instance );

			Assert.AreEqual( 2, errors.Count );
			Assert.IsTrue( errors.Contains( "value" ) );
			Assert.IsTrue( errors.Contains( "internal.date" ) );
			Assert.IsFalse( errors.Contains( "internal.text" ) );

			foreach ( IPropertyError e in errors )
			{
				if ( e.Property == "Value" )
				{
					Assert.AreEqual( "BindObject.Value", e.Key );
					Assert.AreEqual( "BindError.BindObject.Value", e.ToString() );
					Assert.IsNotNull( e.Exception );
				}
				else if ( e.Property == "Internal.Date" )
				{
					Assert.AreEqual( "BindObject.Internal.Date", e.Key );
					Assert.AreEqual( "BindError.BindObject.Internal.Date", e.ToString() );
					Assert.IsNotNull( e.Exception );
				}
				else
					throw new RailsException( "Invalid property error: " + e.Property );
			}

			RenderText( "ok" );
		}

		private void Validate( BindObject instance )
		{
			Assert.AreEqual( Value, instance.Value );
			Assert.AreEqual( Text, instance.Internal.Text );
			// Do a string comparison of the date instances (precision stuff)
			Assert.AreEqual( Date.ToString(), instance.Internal.Date.ToString() );

			RenderText("ok");
		}
	}

	public class BindObject
	{
		private int _Value;
		private InternalBindObject _Internal;

		public int Value
		{
			get { return _Value; }
			set { _Value = value; }
		}

		public InternalBindObject Internal
		{
			get { return _Internal; }
			set { _Internal = value; }
		}

		public class InternalBindObject
		{
			private string _Text;
			private DateTime _Date;

			public string Text
			{
				get { return _Text; }
				set { _Text = value; }
			}

			public DateTime Date
			{
				get { return _Date; }
				set { _Date = value; }
			}
		}
	}
}
