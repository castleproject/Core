// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.TestSupport
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.IO;
	using System.Text;
	using System.Web;
	using System.Web.Hosting;

	[Serializable]
	public class MockWorkerRequest : SimpleWorkerRequest
	{
		private String protocol = "HTTP/1.0";
		private String[] knownRequestHeaders;
		private String[][] unknownRequestHeaders;

		private NameValueCollection headers = new NameValueCollection(RequestHeaderMaximum);
		private NameValueCollection serverVariables = new NameValueCollection();

		private int port = 81;

		private String uriPath;
		private String queryString;
		private byte[] queryStringBytes;
		private String url;
		private String verb;
		private String remoteAddress;
		private String virtualPath;
		private int responseStatus = 200;
		private String physicalPath;
		private string pathInfo;
		private string filePath;
		private string pathTranslated;
		private readonly TextWriter output;

		private byte[] preloadedContent;

		private String[] queryStringParams;
		private String[] postParams;

		private bool headersSent;

		private IList responseBodyBytes;
		private StringBuilder responseHeadersBuilder;
		private int outputContentLength;

		public MockWorkerRequest(/*String appPath, String physicalPath, 
			String filePath, */ TextWriter output) : 
			base(String.Empty, String.Empty, String.Empty, null, output)
		{
//			this.virtualPath = appPath;
//			this.physicalPath = physicalPath;
//			this.filePath = filePath;
			this.output = output;
		}

		#region Properties

		public string VirtualPath
		{
			get { return virtualPath; }
			set { virtualPath = value; }
		}

		public string PhysicalPath
		{
			get { return physicalPath; }
			set { physicalPath = value; }
		}

		public string PathInfo
		{
			get { return pathInfo; }
			set { pathInfo = value; }
		}

		public string FilePath
		{
			get { return filePath; }
			set { filePath = value; }
		}

		public string PathTranslated
		{
			get { return pathTranslated; }
			set { pathTranslated = value; }
		}

		public NameValueCollection Headers
		{
			get { return headers; }
		}

		public NameValueCollection ServerVariables
		{
			get { return serverVariables; }
		}

		public string Protocol
		{
			get { return protocol; }
			set { protocol = value; }
		}

		public int Port
		{
			get { return port; }
			set { port = value; }
		}

		public string UriPath
		{
			get { return uriPath; }
			set { uriPath = value; }
		}

		public string Url
		{
			get { return url; }
			set { url = value; }
		}

		public string Verb
		{
			get { return verb; }
			set { verb = value; }
		}

		public string RemoteAddress
		{
			get { return remoteAddress; }
			set { remoteAddress = value; }
		}

		public string[] QueryStringParams
		{
			get { return queryStringParams; }
			set { queryStringParams = value; }
		}

		public string[] PostParams
		{
			get { return postParams; }
			set { postParams = value; }
		}

		#endregion

		protected internal void Prepare()
		{
			ProcessHeaders();

			PopulateServerVariables();

			ProcessQueryString();

			ProcessPostBody();

			PrepareResponse();
		}

		private void ProcessQueryString()
		{
			if (queryStringParams != null)
			{
				queryString = String.Empty;

				foreach (String param in queryStringParams)
				{
					queryString += String.Format("{0}&", param);
				}

				queryStringBytes = Encoding.ASCII.GetBytes(queryString);
			}
		}

		private void ProcessPostBody()
		{
			if (postParams != null)
			{
				String postParamsExpanded = String.Empty;

				foreach (String param in postParams)
				{
					postParamsExpanded += String.Format("{0}&", param);
				}

				byte[] buffer = Encoding.ASCII.GetBytes(postParamsExpanded);

				preloadedContent = buffer;
			}
		}

		#region SimpleWorkerRequest overrides

		public override String GetHttpVersion()
		{
			return protocol;
		}

		public override String GetUriPath()
		{
			return uriPath;
		}

		public override String GetQueryString()
		{
			return queryString;
		}

		public override byte[] GetQueryStringRawBytes()
		{
			return queryStringBytes;
		}

		public override String GetRawUrl()
		{
			return url;
		}

		public override String GetHttpVerbName()
		{
			return verb;
		}

		public override String GetRemoteAddress()
		{
			return remoteAddress;
		}

		public override int GetRemotePort()
		{
			return 0;
		}

		public override String GetLocalAddress()
		{
			return "127.0.0.1";
		}

		public override int GetLocalPort()
		{
			return port;
		}

		public override String GetFilePath()
		{
			return filePath;
		}

		public override String GetFilePathTranslated()
		{
			return pathTranslated;
		}

		public override String GetPathInfo()
		{
			return pathInfo;
		}

		public override String GetAppPath()
		{
			return virtualPath;
		}

		public override String GetAppPathTranslated()
		{
			return physicalPath;
		}

		public override byte[] GetPreloadedEntityBody()
		{
			return preloadedContent;
		}

		public override bool IsEntireEntityBodyIsPreloaded()
		{
			return true;
		}

		public override int ReadEntityBody(byte[] buffer, int size)
		{
			int bytestocopy = Math.Min(preloadedContent.Length, size);

			Buffer.BlockCopy(preloadedContent, 0, buffer, 0, bytestocopy);

			return bytestocopy;
		}

		public override String GetKnownRequestHeader(int index)
		{
			return knownRequestHeaders[index];
		}

		public override String GetUnknownRequestHeader(String name)
		{
			for (int i = 0; i < unknownRequestHeaders.Length; i++)
			{
				if (String.Compare(name, unknownRequestHeaders[i][0], true, CultureInfo.InvariantCulture) == 0)
				{
					return unknownRequestHeaders[i][1];
				}
			}

			return null;
		}

		public override String[][] GetUnknownRequestHeaders()
		{
			return unknownRequestHeaders;
		}

		public override String GetServerVariable(String name)
		{
			switch (name)
			{
				case "ALL_RAW":
					// value = null;
					break;

				case "SERVER_PROTOCOL":
					return protocol;
			}

			return serverVariables[name];
		}

		public override String MapPath(String path)
		{
			String mappedPath = String.Empty;

			// TODO: MapPath

			return mappedPath;
		}

		public override void SendStatus(int statusCode, String statusDescription)
		{
			responseStatus = statusCode;
		}

		public override void SendKnownResponseHeader(int index, String value)
		{
			if (headersSent) return;

			switch (index)
			{
				case HttpWorkerRequest.HeaderServer:
				case HttpWorkerRequest.HeaderDate:
				case HttpWorkerRequest.HeaderConnection:
					return;
				case HttpWorkerRequest.HeaderAcceptRanges:
					if (value == "bytes") return;
					break;
				case HttpWorkerRequest.HeaderExpires:
				case HttpWorkerRequest.HeaderLastModified:
					return;
			}

			responseHeadersBuilder.Append(GetKnownResponseHeaderName(index));
			responseHeadersBuilder.Append(": ");
			responseHeadersBuilder.Append(value);
			responseHeadersBuilder.Append("\r\n");
		}

		public override void SendUnknownResponseHeader(String name, String value)
		{
			if (headersSent) return;

			responseHeadersBuilder.Append(name);
			responseHeadersBuilder.Append(": ");
			responseHeadersBuilder.Append(value);
			responseHeadersBuilder.Append("\r\n");
		}

		public override void SendCalculatedContentLength(int contentLength)
		{
			if (headersSent) return;

			responseHeadersBuilder.Append("Content-Length: ");
			responseHeadersBuilder.Append(contentLength.ToString());
			responseHeadersBuilder.Append("\r\n");
		}

		public override bool HeadersSent()
		{
			return headersSent;
		}

		public override bool IsClientConnected()
		{
			return true;
		}

		public override void CloseConnection()
		{
		}

		public override void SendResponseFromMemory(byte[] data, int length)
		{
			if (length > 0)
			{
				outputContentLength += length;

				byte[] bytes = new byte[length];
				Buffer.BlockCopy(data, 0, bytes, 0, length);
				responseBodyBytes.Add(bytes);
			}
		}

		public override void FlushResponse(bool finalFlush)
		{
			if (!headersSent)
			{
				WriteHeaders(responseStatus, responseHeadersBuilder.ToString());

				headersSent = true;
			}

			for (int i = 0; i < responseBodyBytes.Count; i++)
			{
				byte[] bytes = (byte[]) responseBodyBytes[i];

				WriteBody(bytes, 0, bytes.Length);
			}

			responseBodyBytes = new ArrayList();
		}

		public override void EndOfRequest()
		{
		}

		#endregion

		private void WriteHeaders(int statusCode, String extraHeaders) 
		{
			String headers = MakeResponseHeaders(statusCode, extraHeaders);
			
			output.Write( Encoding.UTF8.GetBytes(headers) );
		}

		private String MakeResponseHeaders(int statusCode, String moreHeaders)
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendFormat("HTTP/1.1 {0} {1}r\n", statusCode, HttpWorkerRequest.GetStatusDescription(statusCode) );
			sb.Append("Server: Castle test case\r\n");
			sb.AppendFormat("Date: {0}\r\n", DateTime.Now.ToUniversalTime().ToString("R", DateTimeFormatInfo.InvariantInfo));

			if (outputContentLength >= 0) sb.AppendFormat("Content-Length: {0}\r\n", outputContentLength);
			
			if (moreHeaders != null) sb.Append(moreHeaders);
			
			sb.Append("Connection: Close\r\n");
			
			sb.Append("\r\n");
			
			return sb.ToString();
		}

		public void WriteBody(byte[] data, int offset, int length) 
		{
			output.Write( Encoding.UTF8.GetChars(data, offset, length) );
		}

		#region Helper methods

		private void ProcessHeaders()
		{
			knownRequestHeaders = new string[RequestHeaderMaximum];

			IList unknownHeaders = new ArrayList();

			foreach (DictionaryEntry entry in headers)
			{
				String name = entry.Key.ToString();
				String value = entry.Value.ToString();

				int index = GetKnownRequestHeaderIndex(name);

				if (index >= 0)
				{
					knownRequestHeaders[index] = value;
				}
				else
				{
					unknownHeaders.Add(name);
					unknownHeaders.Add(value);
				}
			}

			int totalunknownHeaders = unknownHeaders.Count/2;
			unknownRequestHeaders = new String[totalunknownHeaders][];

			int j = 0;

			for (int i = 0; i < totalunknownHeaders; i++)
			{
				unknownRequestHeaders[i] = new String[2];
				unknownRequestHeaders[i][0] = (String) unknownHeaders[j++];
				unknownRequestHeaders[i][1] = (String) unknownHeaders[j++];
			}

//			if (_headerByteStrings.Count > 1)
//			{
//				_allRawHeaders = Encoding.UTF8.GetString(
//					_headerBytes, _startHeadersOffset, _endHeadersOffset-_startHeadersOffset);
//			}
//			else
//			{
//				_allRawHeaders = String.Empty;
//			}
		}

		private void PopulateServerVariables()
		{
		}

		private void PrepareResponse()
		{
			headersSent = false;
			responseStatus = 200;
			responseHeadersBuilder = new StringBuilder();
			responseBodyBytes = new ArrayList();
			outputContentLength = 0;
		}

		#endregion
	}
}