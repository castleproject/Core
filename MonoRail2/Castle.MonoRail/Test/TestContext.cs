namespace Castle.MonoRail.Test
{
	public class TestContext : IExecutionContext
	{
		private readonly UrlInfo urlInfo;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestContext"/> class.
		/// </summary>
		/// <param name="urlInfo">The URL info.</param>
		public TestContext(UrlInfo urlInfo)
		{
			this.urlInfo = urlInfo;
		}

		/// <summary>
		/// Gets the requested URL information.
		/// </summary>
		/// <value>The UrlInfo instance.</value>
		public UrlInfo OriginalUrl
		{
			get { return urlInfo; }
		}
	}
}
