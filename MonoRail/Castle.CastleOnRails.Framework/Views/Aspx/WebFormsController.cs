using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
// ${Copyrigth}

namespace Castle.CastleOnRails.Framework.Views.Aspx
{
	using System;

	
	public class WebFormsController : SmartDispatcherController
	{
		private const char EvilChar = ':';
		public WebFormsController()
		{
		}

		protected override object[] BuildMethodArguments(ParameterInfo[] parameters, NameValueCollection webParams, IDictionary files)
		{
			return base.BuildMethodArguments(parameters, Normalize(webParams), files);
		}

		private NameValueCollection Normalize(NameValueCollection webParams)
		{
			NameValueCollection normalized = new NameValueCollection(webParams.Count, CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
			
			foreach (string key in webParams)
			{
				normalized.Add(GetKeyWithoutPrefix(key), webParams[key]);
			}

			return normalized;
		}

		private string GetKeyWithoutPrefix(string key)
		{
			if(key.IndexOf(EvilChar) > -1)
			{
				return key.Substring(key.LastIndexOf(EvilChar) + 1);		
			}
			else
			{
				return key;
			}
		}
	}
}
