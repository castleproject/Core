namespace Castle.MonoRail.Framework
{
	using System.Web;

	/// <summary>
	/// Custom attributes can implement this
	/// interface to have a chance to apply
	/// some specific configuration to the 
	/// <see cref="HttpCachePolicy"/>
	/// </summary>
	public interface ICachePolicyConfigurer
	{
		/// <summary>
		/// Implementors should configure 
		/// the specified policy.
		/// </summary>
		/// <param name="policy">The cache policy.</param>
		void Configure(HttpCachePolicy policy);
	}
}
