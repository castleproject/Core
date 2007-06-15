namespace Castle.NewGenerator.Core
{
	using System;

	public class ParamAttribute : Attribute
	{
		private bool required;

		public bool Required
		{
			get { return required; }
			set { required = value; }
		}
	}
}
