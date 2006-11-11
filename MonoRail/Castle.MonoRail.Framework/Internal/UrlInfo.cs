// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Internal
{
	using System;

	/// <summary>
	/// Represents the splitted information on a Url.
	/// </summary>
	[Serializable]
	public class UrlInfo
	{
		private readonly String _urlRaw;
		private readonly String _area;
		private readonly String _controller;
		private readonly String _action;
		private readonly String _extension;

		public UrlInfo( String urlRaw, String area, String controller, String action, String extension )
		{
			_controller = controller;
			_area = area;
			_action = action;
			_urlRaw = urlRaw;
			_extension = extension;
		}

		public String Controller
		{
			get { return _controller; }
		}

		public String Action
		{
			get { return _action; }
		}

		public String Area
		{
			get { return _area; }
		}

		public String UrlRaw
		{
			get { return _urlRaw; }
		}

		/// <summary>
		/// The URL extension, without the leading dot.
		/// </summary>
		public String Extension
		{
			get { return _extension; }
		}

		/// <summary>
		/// Creates an absolute MonoRail url.
		/// </summary>
		public static String CreateAbsoluteRailsUrl(String appPath, String controller, 
		                                            String action, String extension)
		{
			return appPath + '/' + controller + '/' + action + '.' + extension;
		}

		/// <summary>
		/// Creates an absolute MonoRail url.
		/// </summary>
		public static String CreateAbsoluteRailsUrl(String appPath, String area, 
		                                            String controller, String action, 
		                                            String extension)
		{
			if (area == null || area.Length == 0)
			{
				return CreateAbsoluteRailsUrl(appPath, controller, action, extension);
			}
			
			return appPath + '/' + area + '/' + controller + '/' + action + '.' + extension;
		}
	}
}
