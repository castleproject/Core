namespace Castle.MonoRail.Framework
{
	using System;

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IUrlTokenizer
	{
		/// <summary>
		/// Tokenizes the URL.
		/// </summary>
		/// <param name="rawUrl">The raw URL.</param>
		/// <param name="uri">The URI.</param>
		/// <param name="isLocal">if set to <c>true</c> [is local].</param>
		/// <param name="appVirtualDir">Virtual directory</param>
		/// <returns></returns>
		UrlInfo TokenizeUrl(string rawUrl, Uri uri, bool isLocal, string appVirtualDir);
	}
}
