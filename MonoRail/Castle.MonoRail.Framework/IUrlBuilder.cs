namespace Castle.MonoRail.Framework
{
	using System.Collections;
	using System.Collections.Specialized;

	public interface IUrlBuilder
	{
		string BuildUrl(UrlInfo current, IDictionary parameters);

		string BuildUrl(UrlInfo current, string controller, string action);

		string BuildUrl(UrlInfo current, string controller, string action, IDictionary queryStringParams);

		string BuildUrl(UrlInfo current, string controller, string action, NameValueCollection queryStringParams);

		string BuildUrl(UrlInfo current, string area, string controller, string action);

		string BuildUrl(UrlInfo current, string area, string controller, string action, IDictionary queryStringParams);

		string BuildUrl(UrlInfo current, string area, string controller, string action, NameValueCollection queryStringParams);
	}
}
