using System.Reflection;
// ${Copyrigth}

namespace Castle.CastleOnRails.Framework.Views.Aspx
{
	using System;
	using System.Web.UI;
	
	public class InvokeHelper : Control
	{
		private string _name;
		private string _method;
		private object[] _args;
		private object _arg;

		public InvokeHelper()
		{
		}

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
				return (Controller) Context.Items["rails.controller"];
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			base.DataBind();

			//TODO: Add some assertions.

			object helper = Controller.Helpers[Name];

			MethodInfo method = helper.GetType().GetMethod( Method, BindingFlags.Instance | BindingFlags.Public);

			if (Args == null)
			{
				Args = new object[]{ Arg };
			}

			object result = method.Invoke(helper, Args);

			writer.WriteLine(result);
		}
	}
}
