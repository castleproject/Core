using System;
using Castle.MVC.States;

namespace Castle.MVC.Web
{
	/// <summary>
	/// Summary description for MyState.
	/// </summary>
	public class MyApplicationState : BaseState
	{
		public string SomeSessionString
		{
			get { return this["SomeSessionString"] as string;}
			set{this["SomeSessionString"]=value;}
		}
	}
}
