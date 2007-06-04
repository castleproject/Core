// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Configuration;
	using System.IO;
	using Cassini;

	/// <summary>
	/// Manages a <see cref="Cassini.Server"/> instance. This is useful 
	/// to start/stop a lightweight webserver to run acceptance tests.
	/// </summary>
	public static class WebServer
	{
		private const string AppPathWeb = "web.physical.dir";
		
		private static string virtualDir = "/";
		private static int port = 88;
		private static bool started;
		private static Cassini.Server server;

		/// <summary>
		/// Gets or sets the port to run the server. Defaults to 88.
		/// </summary>
		/// <value>The port.</value>
		public static int Port
		{
			get { return port; }
			set { port = value; }
		}

		/// <summary>
		/// Gets or sets the virtual dir to be used by the server. Defaults to <c>/</c>
		/// </summary>
		/// <value>The virtual dir.</value>
		public static string VirtualDir
		{
			get { return virtualDir; }
			set { virtualDir = value; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="WebServer"/> is started.
		/// </summary>
		/// <value><c>true</c> if started; otherwise, <c>false</c>.</value>
		public static bool Started
		{
			get { return started; }
		}

		/// <summary>
		/// Starts the web server. The web project folder is going to be 
		/// extracted from the appSettings.webapp entry (from the configuration file)
		/// <para>
		/// If the path is relative, it is going to be converted to an absolute path.
		/// </para>
		/// </summary>
		public static void StartWebServer()
		{
			string webAppFromConfig = ConfigurationManager.AppSettings[AppPathWeb];
			string webAppAbsPath = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, webAppFromConfig)).FullName;

			StartWebServer(webAppAbsPath);
		}

		/// <summary>
		/// Starts the web server using the specified web project path. Note 
		/// that the path must be absolute. 
		/// </summary>
		/// <param name="webApplicationAbsolutePath">The web application absolute path.</param>
		public static void StartWebServer(string webApplicationAbsolutePath)
		{
			if (!Directory.Exists(webApplicationAbsolutePath))
			{
				throw new ApplicationException("Cannot start web server as the path could not be found. " + 
					"Check if the following folder exists: " + webApplicationAbsolutePath);
			}

			server = new Server(88, VirtualDir, webApplicationAbsolutePath);
			server.Start();

			started = true;
		}

		/// <summary>
		/// Stops the web server.
		/// </summary>
		public static void StopWebServer()
		{
			if (started)
			{
				server.Stop();
			}
		}
	}
}
