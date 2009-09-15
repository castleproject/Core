// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MonoRail.Framework.ViewComponents
{

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;
	using Castle.Core.Resource;
	using Castle.MonoRail.Framework.Services;


	/// <summary>
	/// View component for concatenating and minifying Javascript
	/// and CSS
	/// </summary>
	[ViewComponentDetails("CombineJS")]
	public class CombineJSViewComponent : ViewComponent
	{
		private delegate string PostProcessScript(string file, string script);

		private IScriptBuilder scriptBuilder;

		///<summary>
		/// The default ScriptBuilder can be replaced
		///</summary>
		public IScriptBuilder ScriptBuilder
		{
			get { return scriptBuilder; }
			set { scriptBuilder = value; }
		}

		/// <summary>
		/// Initiliaze it.
		/// </summary>
		public override void Initialize()
		{
			scriptBuilder = EngineContext.Services.ScriptBuilder;

			base.Initialize();
		}

		/// <summary>
		/// Called by the framework so the component can
		/// render its content
		/// </summary>
		public override void Render()
		{
			CombinerConfig combiner = new CombinerConfig(AppDomain.CurrentDomain.BaseDirectory, EngineContext.ApplicationPath);
			PropertyBag["combiner"] = combiner;

			// Evaluate the component body, without output
			RenderBody(new StringWriter());

			string key = (string) ComponentParams["key"];
			string cssKey = key + "css";

			if (!ScriptBuilder.Concatenate)
			{
				foreach (string file in combiner.CssFiles)
					RenderCSS(combiner.Relative(file));
				foreach (string file in combiner.JavascriptFiles)
					RenderJavascript(combiner.Relative(file));
			}
			else
			{
				long cssHash = CalculateChangeSetHash(combiner.CssFiles);
				long javascriptHash = CalculateChangeSetHash(combiner.JavascriptFiles);

				IStaticResourceRegistry resourceRegistry = EngineContext.Services.StaticResourceRegistry;

				if (!resourceRegistry.Exists(key, null, javascriptHash.ToString()))
				{
					RegisterJavascript(combiner, key, resourceRegistry, javascriptHash);
				}

				if (!resourceRegistry.Exists(cssKey, null, cssHash.ToString()))
				{
					RegisterCss(combiner, resourceRegistry, cssKey, cssHash);
				}

				string extension = String.Empty;
				if (EngineContext.Services.UrlBuilder.UseExtensions)
				{
					extension = "." + EngineContext.UrlInfo.Extension;
				}

				if (cssHash != 0)
				{
					string cssFullName = string.Format("{0}/MonoRail/Files/BuiltJS{1}?name={2}&version={3}",
					                                   EngineContext.ApplicationPath, extension, cssKey, cssHash);
					RenderCSS(cssFullName);
				}

				if (javascriptHash != 0)
				{
					string javascriptFullName = string.Format("{0}/MonoRail/Files/BuiltJS{1}?name={2}&version={3}",
					                                          EngineContext.ApplicationPath, extension, key, javascriptHash);
					RenderJavascript(javascriptFullName);
				}
			}
		}

		/// <summary>
		/// Renders the CSS.
		/// </summary>
		/// <param name="file">The file.</param>
		private void RenderCSS(string file)
		{
			RenderText(string.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />{1}",
									 file, Environment.NewLine));
		}

		/// <summary>
		/// Renders the javascript.
		/// </summary>
		/// <param name="file">The file.</param>
		private void RenderJavascript(string file)
		{
			RenderText(string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>{1}",
									 file, Environment.NewLine));
		}

		/// <summary>
		/// Registers the CSS.
		/// </summary>
		/// <param name="combiner">The combiner.</param>
		/// <param name="resourceRegistry">The resource registry.</param>
		/// <param name="cssKey">The CSS key.</param>
		/// <param name="cssHash">The CSS hash.</param>
		private void RegisterCss(CombinerConfig combiner, IStaticResourceRegistry resourceRegistry, string cssKey, long cssHash)
		{
			if (combiner.CssFiles.Count < 1) return;

			string css = CombineCssFileContent(combiner);

			if (ScriptBuilder.Minify)
				css = ScriptBuilder.CompressCSS(css);

			StaticContentResource cssResource = new StaticContentResource(css);

			resourceRegistry.RegisterCustomResource(cssKey, null, cssHash.ToString(), cssResource,
													"text/css", DateTime.Now);
		}

		/// <summary>
		/// Registers the javascript.
		/// </summary>
		/// <param name="combiner">The combiner.</param>
		/// <param name="key">The key.</param>
		/// <param name="resourceRegistry">The resource registry.</param>
		/// <param name="javascriptHash">The javascript hash.</param>
		private void RegisterJavascript(CombinerConfig combiner, string key, IStaticResourceRegistry resourceRegistry, long javascriptHash)
		{
			if (combiner.JavascriptFiles.Count < 1) return;

			string script = CombineJSFileContent(combiner.JavascriptFiles);

			if (ScriptBuilder.Minify)
				script = ScriptBuilder.CompressJavascript(script);

			StaticContentResource staticContentResource = new StaticContentResource(script);

			resourceRegistry.RegisterCustomResource(key, null, javascriptHash.ToString(), staticContentResource,
													"application/x-javascript", DateTime.Now);
		}

		/// <summary>
		/// Combines the content of the CSS files.
		/// </summary>
		/// <param name="combiner">The combiner.</param>
		/// <returns></returns>
		private string CombineCssFileContent(CombinerConfig combiner)
		{
			CssRelativeUrlResolver resolver =
				new CssRelativeUrlResolver(AppDomain.CurrentDomain.BaseDirectory, new Uri(EngineContext.ApplicationPath + "/", UriKind.Relative));

			PostProcessScript postProcess = resolver.Resolve;

			return CombineFileContent(combiner.CssFiles, postProcess);
		}


		/// <summary>
		/// Combines the content of the Javascript files.
		/// </summary>
		/// <param name="files">The files.</param>
		/// <returns></returns>
		private static string CombineJSFileContent(ICollection<string> files)
		{
			return CombineFileContent(files, (file, line) => line);
		}

		/// <summary>
		/// Combines the content of the file.
		/// </summary>
		/// <param name="files">The files.</param>
		/// <param name="postProcess">The post process.</param>
		/// <returns></returns>
		private static string CombineFileContent(ICollection<string> files, PostProcessScript postProcess)
		{
			StringBuilder sb = new StringBuilder(1024 * files.Count);

			foreach (string file in files)
			{
				using (StreamReader reader = File.OpenText(file))
				{
					string line = reader.ReadToEnd();
					line = postProcess.Invoke(file, line);
					sb.Append(line);
				}
				sb.AppendLine();
			}

			return sb.ToString();
		}

		/// <summary>
		/// Calculates the change set hash.
		/// </summary>
		/// <param name="files">The files.</param>
		/// <returns></returns>
		private static long CalculateChangeSetHash(IEnumerable<string> files)
		{
			long hash = 0;

			foreach (string file in files)
			{
				if (File.Exists(file))
				{
					DateTime dt = File.GetLastWriteTimeUtc(file);
					hash += dt.Ticks * 37;
				}
			}

			return hash;
		}

		/// <summary>
		/// Configuration class for the Javascript and CSS files that will be combined.
		/// </summary>
		public class CombinerConfig
		{
			private readonly string basePath;
			private readonly string applicationPath;
			private readonly List<string> javascriptFiles = new List<string>();
			private readonly List<string> cssFiles = new List<string>();

			/// <summary>
			/// Initializes a new instance of the <see cref="CombinerConfig"/> class.
			/// </summary>
			/// <param name="basePath">The base path.</param>
			/// <param name="applicationPath">The application path.</param>
			public CombinerConfig(string basePath, string applicationPath)
			{
				this.basePath = basePath;
				this.applicationPath = applicationPath;
			}

			/// <summary>
			/// Gets the Javascript files.
			/// </summary>
			/// <value>The javascript files.</value>
			public IList<string> JavascriptFiles
			{
				get { return javascriptFiles; }
			}

			/// <summary>
			/// Gets the CSS files.
			/// </summary>
			/// <value>The CSS files.</value>
			public IList<string> CssFiles
			{
				get { return cssFiles; }
			}

			/// <summary>
			/// Adds the specified file.
			/// </summary>
			/// <param name="file">The file.</param>
			public void Add(string file)
			{
				Add(file, null);
			}

			/// <summary>
			/// Adds the specified file.
			/// </summary>
			/// <param name="file">The file.</param>
			/// <param name="options">The options.</param>
			public void Add(string file, IDictionary options)
			{
				if (file.EndsWith(".js"))
					javascriptFiles.Add(Path.Combine(basePath, file));
				else if (file.EndsWith(".css"))
					cssFiles.Add(Path.Combine(basePath, file));
			}

			/// <summary>
			/// Gets the relative path to the file.
			/// </summary>
			/// <param name="file">The file.</param>
			/// <returns></returns>
			public string Relative(string file)
			{
				return string.Format("{0}/{1}", applicationPath, file.Remove(0, basePath.Length));
			}
		}

		/// <summary>
		/// Utility class for determining the absolute path for any CSS files that have
		/// relative URLs.
		/// </summary>
		public class CssRelativeUrlResolver
		{
			private static readonly Regex CssUrls = new Regex(@"(.*?)(url)\((?!http|/)(?<Url>.*?)\)(.*?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			private readonly string physicalRoot;
			private readonly Uri virtualRoot;

			/// <summary>
			/// Initializes a new instance of the <see cref="CssRelativeUrlResolver"/> class.
			/// </summary>
			/// <param name="physicalRoot">The physical root.</param>
			/// <param name="virtualRoot">The virtual root.</param>
			public CssRelativeUrlResolver(string physicalRoot, Uri virtualRoot)
			{
				this.physicalRoot = Path.GetFullPath(physicalRoot);

				if (virtualRoot.IsAbsoluteUri)
					throw new ArgumentException("virtualRoot must be relative", "virtualRoot");

				//Assign the virtual root to the loopback so that we can use System.Uri to combine relative Uri's.  Below
				//use the AbsolutePath so that the loopback is removed.
				this.virtualRoot = new Uri(new Uri(@"http://127.0.0.1/"), virtualRoot);
			}

			/// <summary>
			/// Converts any URLs in the CSS string to be absolute URLs.
			/// </summary>
			/// <param name="file">The file.</param>
			/// <param name="input">The input.</param>
			/// <returns></returns>
			public string Resolve(string file, string input)
			{
				return CssUrls.Replace(input, match => RootPathInFileForMatch(file, match));
			}

			private string RootPathInFileForMatch(string file, Match match)
			{
				Group urlGroup = match.Groups["Url"];
				string url = urlGroup.Value;

				if (IsRelative(url))
				{
					string relativePhysicalFilePath = MakeRelative(Path.GetDirectoryName(file), physicalRoot);

					Uri startPath = new Uri(virtualRoot, relativePhysicalFilePath);
					Uri rootedUrl = new Uri(startPath, url);
					int relativeGroupIndex = urlGroup.Index - match.Index;
					return match.Value.Substring(0, relativeGroupIndex) + rootedUrl.AbsolutePath + match.Value.Substring(relativeGroupIndex + urlGroup.Length);
				}

				return match.Value;
			}

			private static string MakeRelative(string path, string root)
			{
				string[] partParts = path.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
				string[] rootParts = root.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);

				int start = 0;

				for (int i = 0; i < partParts.Length; i++)
				{
					if ((rootParts.Length > i) && partParts[i].Equals(rootParts[i], StringComparison.InvariantCultureIgnoreCase)) start = i;
					else break;
				}

				var parts = partParts.Where((part, index) => index > start);

				string joined = string.Join(Path.DirectorySeparatorChar.ToString(), parts.ToArray());

				if (!joined.EndsWith(Path.DirectorySeparatorChar.ToString()))
				{
					joined = joined + Path.DirectorySeparatorChar;
				}

				return joined;
			}

			private static bool IsRelative(string url)
			{
				return !Uri.IsWellFormedUriString(url, UriKind.Absolute);
			}
		}
	}
}
