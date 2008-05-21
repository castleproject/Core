// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Text;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Pendent
	/// </summary>
	public class UrlParts
	{
		private readonly StringBuilder url;
		private PathInfoBuilder pathInfoBuilder;
		private PathInfoDictBuilder pathInfoDictBuilder;
		private bool nextPathBelongsToPathInfo;
		private string queryString;
		private NameValueCollection queryStringDict;

		/// <summary>
		/// Initializes a new instance of the <see cref="UrlParts"/> class.
		/// </summary>
		/// <param name="pathPieces">The path pieces.</param>
		public UrlParts(params string[] pathPieces)
		{
			url = new StringBuilder();

			AppendPaths(pathPieces);
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		public static UrlParts Parse(string url)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}

			Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);

			if (uri.IsAbsoluteUri)
			{
				return CreateForAbsolutePath(uri);
			}
			else
			{
				return CreateForRelativePath(url);
			}
		}

		/// <summary>
		/// Gets the path info builder.
		/// </summary>
		/// <value>The path info.</value>
		public PathInfoBuilder PathInfo
		{
			get
			{
				if (pathInfoBuilder == null)
				{
					pathInfoBuilder = new PathInfoBuilder(this);
				}
				return pathInfoBuilder;
			}
		}

		/// <summary>
		/// Gets the path info builder with dictionary api.
		/// </summary>
		/// <value>The path info.</value>
		public PathInfoDictBuilder PathInfoDict
		{
			get
			{
				if (pathInfoDictBuilder == null)
				{
					pathInfoDictBuilder = new PathInfoDictBuilder(this);
				}
				return pathInfoDictBuilder;
			}
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="queryStringParam">The query string.</param>
		public UrlParts SetQueryString(string queryStringParam)
		{
			if (queryStringParam != null && queryStringParam.StartsWith("?"))
			{
				queryStringParam = queryStringParam.Substring(1);
			}

			queryString = queryStringParam;

			return this;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <value>The query string.</value>
		public NameValueCollection QueryString
		{
			get
			{
				if (queryStringDict == null)
				{
					queryStringDict = CreateQueryStringNameValueCollection(queryString);
				}

				return queryStringDict;
			}
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <returns></returns>
		public string QueryStringAsString()
		{
			if (queryStringDict != null)
			{
				queryString = CommonUtils.BuildQueryString(queryStringDict);
			}

			return queryString;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <returns></returns>
		public UrlParts ConvertPathInfoToDict()
		{
			if (pathInfoBuilder == null)
			{
				return this;
			}

			PathInfoDict.Parse(pathInfoBuilder.ToString());

			pathInfoBuilder = null;

			return this;
		}

		/// <summary>
		/// Builds the path.
		/// </summary>
		/// <returns></returns>
		public string BuildPath()
		{
			StringBuilder sb = new StringBuilder(url.ToString());

			BuildPathInfo(sb);

			if (queryStringDict != null && queryStringDict.Count != 0)
			{
				sb.Append('?');
				sb.Append(QueryStringAsString());
			}
			else if (!string.IsNullOrEmpty(queryString))
			{
				sb.Append('?');
				sb.Append(queryString);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Builds the path.
		/// </summary>
		/// <returns></returns>
		public string BuildPathForLink(IServerUtility serverUtiliy)
		{
			StringBuilder sb = new StringBuilder(url.ToString());

			BuildPathInfo(sb);

			if (queryStringDict != null && queryStringDict.Count != 0)
			{
				sb.Append('?');
				sb.Append(CommonUtils.BuildQueryString(serverUtiliy, QueryString, true));
			}
			else if (!string.IsNullOrEmpty(queryString))
			{
				sb.Append('?');
				sb.Append(serverUtiliy.HtmlEncode(queryString));
			}

			return sb.ToString();
		}

		/// <summary>
		/// Inserts the piece in front of the existing path.
		/// </summary>
		/// <param name="piece">The path.</param>
		public void InsertFrontPath(string piece)
		{
			if (piece.Length != 1 && piece.EndsWith("/") && url.Length != 0 && url[0] == '/')
			{
				piece = piece.Substring(0, piece.Length - 1);
			}
			if (piece.Length != 1 && !piece.EndsWith("/") && url.Length != 0 && url[0] != '/')
			{
				piece += "/";
			}

			string newUrl = piece + url;

			url.Length = 0;
			url.Append(newUrl);

		}
		
		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="piece">The piece.</param>
		public UrlParts AppendPath(string piece)
		{
			if (piece.Length != 1 && piece.EndsWith("/"))
			{
				piece = piece.Substring(0, piece.Length - 1);
			}

			if (nextPathBelongsToPathInfo)
			{
				PathInfo.Add(piece);
			}
			else
			{
				if (!piece.StartsWith("/") && HasLastChar && LastChar != '/')
				{
					url.Append('/');
				}

				url.Append(piece);
			}

			if (HasLastChar && piece.IndexOf('.') != -1) // this is fragile!
			{
				nextPathBelongsToPathInfo = true;
			}

			return this;
		}

		private void AppendPaths(string[] pieces)
		{
			foreach (string piece in pieces)
			{
				AppendPath(piece);
			}
		}

		private bool HasLastChar
		{
			get { return url.Length != 0; }
		}

		private char LastChar
		{
			get
			{
				if (url.Length != 0)
				{
					return url[url.Length - 1];
				}

				return '\0';
			}
		}

		private void BuildPathInfo(StringBuilder sb)
		{
			if (pathInfoBuilder != null)
			{
				pathInfoBuilder.Build(sb);
			}
			if (pathInfoDictBuilder != null)
			{
				pathInfoDictBuilder.Build(sb);
			}
		}

		private static UrlParts CreateForRelativePath(string url)
		{
			string path = url;
			string qs = null;
			string pathInfo = null;

			int queryStringStartIndex = url.IndexOf('?');
			int fileExtIndex = url.IndexOf('.');

			if (queryStringStartIndex != -1)
			{
				qs = url.Substring(queryStringStartIndex);
				path = url.Substring(0, queryStringStartIndex);
			}

			if (fileExtIndex != -1)
			{
				int pathInfoStartIndex = path.IndexOf('/', fileExtIndex);

				if (pathInfoStartIndex != -1)
				{
					pathInfo = path.Substring(pathInfoStartIndex);
					path = path.Substring(0, pathInfoStartIndex);
				}
			}

			UrlParts parts = new UrlParts(path);
			parts.SetQueryString(qs);
			parts.PathInfoDict.Parse(pathInfo);

			return parts;
		}

		private static UrlParts CreateForAbsolutePath(Uri uri)
		{
			string host = uri.AbsoluteUri.Substring(0, uri.AbsoluteUri.Length - uri.PathAndQuery.Length);

			UrlParts parts = new UrlParts(host);

			foreach (string segment in uri.Segments)
			{
				parts.AppendPath(segment);
			}

			parts.ConvertPathInfoToDict();
			parts.SetQueryString(uri.Query);

			return parts;
		}

		private static NameValueCollection CreateQueryStringNameValueCollection(string queryString)
		{
			NameValueCollection coll = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);

			if (queryString == null)
			{
				return coll;
			}

			foreach(string valuePair in queryString.Split('&'))
			{
				string[] pairs = valuePair.Split(new char[] { '=' }, 2);

				if (pairs.Length == 2)
				{
					coll.Add(pairs[0], pairs[1]);
				}
				else if (pairs.Length == 1)
				{
					coll.Add(pairs[0], string.Empty);
				}
			}

			return coll;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public class PathInfoBuilder
		{
			private readonly UrlParts parent;
			private readonly List<string> pieces;

			/// <summary>
			/// Initializes a new instance of the <see cref="PathInfoBuilder"/> class.
			/// </summary>
			/// <param name="parent">The parent.</param>
			public PathInfoBuilder(UrlParts parent)
			{
				this.parent = parent;
				pieces = new List<string>();
			}

			/// <summary>
			/// Adds a path info piece.
			/// </summary>
			/// <param name="pathInfoPiece">The path info piece.</param>
			/// <returns></returns>
			public PathInfoBuilder Add(string pathInfoPiece)
			{
				pieces.Add(pathInfoPiece);
				return this;
			}

			/// <summary>
			/// Returns to the previous builder context.
			/// </summary>
			public UrlParts Done
			{
				get { return parent; }
			}

			/// <summary>
			/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
			/// </summary>
			///
			/// <returns>
			/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
			/// </returns>
			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();

				pieces.ForEach(delegate(string piece) { sb.Append(piece).Append('/'); });

				return sb.ToString();
			}

			/// <summary>
			/// Builds the specified URL.
			/// </summary>
			/// <param name="url">The URL.</param>
			protected internal void Build(StringBuilder url)
			{
				if (pieces.Count == 0)
				{
					return;
				}

				foreach(string piece in pieces)
				{
					if (url[url.Length - 1] != '/')
					{
						url.Append('/');
					}

					url.Append(piece);
				}
			}
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public class PathInfoDictBuilder
		{
			private readonly UrlParts parent;
			private readonly IDictionary<string, string> parameters;

			/// <summary>
			/// Initializes a new instance of the <see cref="PathInfoBuilder"/> class.
			/// </summary>
			/// <param name="parent">The parent.</param>
			public PathInfoDictBuilder(UrlParts parent)
			{
				this.parent = parent;
				parameters = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			}

			/// <summary>
			/// Gets or sets the <see cref="System.String"/> with the specified key.
			/// </summary>
			/// <value></value>
			public string this[string key]
			{
				get { return parameters[key]; }
				set { parameters[key] = value; }
			}

			/// <summary>
			/// Gets the count.
			/// </summary>
			/// <value>The count.</value>
			public int Count
			{
				get { return parameters.Count; }
			}

			/// <summary>
			/// Adds a path info piece.
			/// </summary>
			/// <param name="key">The key.</param>
			/// <param name="value">The value.</param>
			/// <returns></returns>
			public PathInfoDictBuilder Add(string key, string value)
			{
				parameters[key] = value;
				return this;
			}

			/// <summary>
			/// Parses the specified path info.
			/// </summary>
			/// <param name="pathInfo">The path info.</param>
			public void Parse(string pathInfo)
			{
				if (string.IsNullOrEmpty(pathInfo))
				{
					return;
				}

				string key = null;

				foreach(string piece in pathInfo.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
				{
					if (key == null)
					{
						key = piece;
					}
					else
					{
						this[key] = piece;
						key = null;
					}
				}

				if (key != null)
				{
					this[key] = string.Empty;
				}

			}

			/// <summary>
			/// Returns to the previous builder context.
			/// </summary>
			public UrlParts Done
			{
				get { return parent; }
			}

			/// <summary>
			/// Builds the specified URL.
			/// </summary>
			/// <param name="url">The URL.</param>
			protected internal void Build(StringBuilder url)
			{
				if (parameters.Count == 0)
				{
					return;
				}

				foreach(KeyValuePair<string, string> pair in parameters)
				{
					if (url[url.Length - 1] != '/')
					{
						url.Append('/');
					}

					url.Append(pair.Key);
	
					if (pair.Value != string.Empty)
					{
						url.Append('/');
						url.Append(pair.Value);
					}
				}
			}
		}
	}
}
