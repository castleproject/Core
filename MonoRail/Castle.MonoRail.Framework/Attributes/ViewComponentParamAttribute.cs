namespace Castle.MonoRail.Framework
{
	using System;

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true), Serializable]
	public class ViewComponentParamAttribute : Attribute
	{
		private string paramName;
		private bool required;

		public ViewComponentParamAttribute()
		{
		}

		public ViewComponentParamAttribute(string paramName)
		{
			this.paramName = paramName;
		}

		public bool Required
		{
			get { return required; }
			set { required = value; }
		}

		public string ParamName
		{
			get { return paramName; }
		}
	}
}
