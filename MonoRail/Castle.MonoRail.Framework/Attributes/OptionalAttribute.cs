using System;

namespace Castle.MonoRail.Framework.Attributes
{
	/// <summary>
	/// Summary description for OptionalAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple=false), Serializable]
	public class OptionalAttribute : Attribute
	{
		public OptionalAttribute()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
