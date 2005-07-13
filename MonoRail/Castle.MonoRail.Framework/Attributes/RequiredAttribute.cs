using System;

namespace Castle.MonoRail.Framework.Attributes
{
	/// <summary>
	/// Summary description for RequiredAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple=false), Serializable]
	public class RequiredAttribute : Attribute
	{
		public RequiredAttribute()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
