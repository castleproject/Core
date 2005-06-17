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

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Web.UI;
	using System.Reflection;
	
	/// <summary>
	/// Control used to invoke method os a specified Helper.
	/// </summary>
	public class InvokeHelper : Control
	{
		private string _name;
		private string _method;
		private object[] _args;
		private object _arg;

		public InvokeHelper()
		{
		}

		/// <summary>
		/// The Helper's Name.
		/// </summary>
		/// <value>A <see cref="String"/> representing the Helper's Name.</value>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// The name of the method which will be invoked.
		/// </summary>
		/// <value>A <see cref="String"/> rerprsenting the method's name</value>
		public string Method
		{
			get
			{
				return _method;
			}
			set
			{
				_method = value;
			}
		}

		/// <summary>
		/// An <see cref="Array"/> of objects which are the method arguments.
		/// </summary>
		/// <value>An object[] representing the arguments.</value>
		public object[] Args
		{
			get
			{
				return _args;
			}
			set
			{
				_args = value;
			}
		}

		/// <summary>
		/// An <see cref="Object"/> which is the method argument.
		/// </summary>
		/// <value>An <see cref="Object"/> representing the argument.</value>
		public object Arg
		{
			get
			{
				return _arg;
			}
			set
			{
				_arg = value;
			}
		}


		protected Controller Controller
		{
			get
			{
				return (Controller) Context.Items[Controller.ControllerContextKey];
			}
		}

		public override void DataBind()
		{
			base.DataBind();

			if (Name == null || Name.Length == 0) throw new ArgumentNullException("Name");

			if (Method == null || Method.Length == 0) throw new ArgumentNullException("Method");

			if (Args == null && Arg != null)
			{
				Args = new object[]{ Arg };
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			DataBind();

			object helper = GetHelper();

			MethodInfo method = GetMethod(helper);

			object result = method.Invoke(helper, Args);

			writer.WriteLine(result);
		}

		private object GetHelper()
		{
			object helper = Controller.Helpers[Name];
	
			if (helper == null)
			{
				throw new RailsException("Helper not found. Helper name: " + Name);
			}

			return helper;
		}

		private MethodInfo GetMethod(object helper)
		{
			Type[] argsTypes = null;
	
			if (Args == null)
			{
				argsTypes = new Type[0];
			}
			else
			{
				argsTypes = Type.GetTypeArray(Args);
			}
	
			MethodInfo method = helper.GetType().GetMethod( Method, BindingFlags.Instance | BindingFlags.Public, null, argsTypes, null);
	
			if (method == null)
			{
				throw new RailsException(string.Format("Helper's method not found. Method: {0}.{1}", Name, Method));
			}

			return method;
		}
	}
}
