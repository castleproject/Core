namespace NVelocity.Test
{
	using System;
	using System.Collections;
	using System.Text;

	public class AjaxHelper2
	{
		public AjaxHelper2()
		{
		}

		public String LinkToRemote(String name, String url, IDictionary options)
		{
			if (options == null) throw new ArgumentNullException("options");

			StringBuilder sb = new StringBuilder(name + " " + url + " ");

			
			Array keysSorted = (new ArrayList(options.Keys)).ToArray(typeof(string)) as string[] ;

			Array.Sort( keysSorted );

			foreach(string key in keysSorted)
			{
				sb.Append(key).Append("=<").Append(options[key]).Append("> ");
			}

			sb.Length--;

			return sb.ToString();
		}

		public String LinkToRemote(String name, String url, string options)
		{
			if (options == null) throw new ArgumentNullException("options");

			return name + " " + url + " " + options;
		}
	}
}