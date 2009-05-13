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

namespace NVelocity.Runtime.Resource
{
	using System;
	using System.IO;
	using NVelocity.Exception;

	/// <summary> 
	/// This class represent a general text resource that
	/// may have been retrieved from any number of possible
	/// sources.
	/// 
	/// Also of interest is Velocity's {@link org.apache.velocity.Template}
	/// <code>Resource</code>.
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
	/// </author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: ContentResource.cs,v 1.5 2004/01/02 00:09:23 corts Exp $
	///
	/// </version>
	public class ContentResource : Resource
	{
		/// <summary>Default empty constructor
		/// </summary>
		public ContentResource()
		{
		}

		/// <summary>
		/// Pull in static content and store it.
		///
		/// @exception ResourceNotFoundException Resource could not be
		/// found.
		/// </summary>
		public override bool Process()
		{
			StreamReader reader = null;

			try
			{
				StringWriter sw = new StringWriter();

				reader = new StreamReader(
					new StreamReader(
						resourceLoader.GetResourceStream(name),
						System.Text.Encoding.GetEncoding(encoding)).BaseStream);

				char[] buf = new char[1024];
				int len = 0;

				// -1 in java, 0 in .Net
				while((len = reader.Read(buf, 0, 1024)) > 0)
				{
					sw.Write(buf, 0, len);
				}

				data = sw.ToString();

				return true;
			}
			catch(ResourceNotFoundException e)
			{
				// Tell the ContentManager to continue to look through any
				// remaining configured ResourceLoaders.
				throw e;
			}
			catch(Exception e)
			{
				runtimeServices.Error(string.Format("Cannot process content resource : {0}", e.ToString()));
				return false;
			}
			finally
			{
				if (reader != null)
				{
					try
					{
						reader.Close();
					}
					catch(Exception)
					{
					}
				}
			}
		}
	}
}