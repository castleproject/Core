namespace Castle.DynamicProxy.Test.Classes
{
	using System;

	/// <summary>
	/// Summary description for GuidClass.
	/// </summary>
	public class GuidClass 
	{
		public virtual Guid GooId 
		{
			get { return Guid.NewGuid(); }
		}
	}
}
