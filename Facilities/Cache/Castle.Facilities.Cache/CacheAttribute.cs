using System;

namespace Castle.Facilities.Cache
{
	/// <summary>
	/// Indicates the cache support for a method.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=false)]
	public class CacheAttribute : System.Attribute
	{

	}
}
