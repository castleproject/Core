// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.CastleOnRails.Framework.Internal
{
	using System;

	/// <summary>
	/// Represents the splitted information on a Url.
	/// </summary>
	public class UrlInfo
	{
		private readonly String _urlRaw;
		private readonly String _area;
		private readonly String _controller;
		private readonly String _action;

		public UrlInfo(String urlRaw, String area, String controller, String action)
		{
			_controller = controller;
			_area = area;
			_action = action;
			_urlRaw = urlRaw;
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
	}
}
