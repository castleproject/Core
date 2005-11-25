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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Text;
	using System.Runtime.Serialization;

	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Thrown when a controller is not found.
	/// </summary>
	[Serializable]
	public class ControllerNotFoundException : ApplicationException
	{
		String area, controller;
	
		public ControllerNotFoundException(String area, String controller) : base(BuildExceptionMessage(area, controller))
		{
		}
		
		public ControllerNotFoundException(String area, String controller, Exception innerException) : base(BuildExceptionMessage(area, controller), innerException)
		{
		}
		
		public ControllerNotFoundException(UrlInfo url) : this(url.Area, url.Controller)
		{
		}

		public ControllerNotFoundException(UrlInfo url, Exception innerException) : this(url.Area, url.Controller, innerException)
		{
		}

		public String Area
		{
			get { return area; }
		}

		public String Controller
		{
			get { return controller; }
		}

		#region Serialization Support
		protected ControllerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.area = info.GetString("rails.area");
			this.controller = info.GetString("rails.controller");
		}
		
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("rails.area", area);
			info.AddValue("rails.controller", controller);
		}
		#endregion

		private static String BuildExceptionMessage( String area, String controller )
		{
			StringBuilder sb = new StringBuilder( "Controller not found." );

			sb.AppendFormat(" Area: '{0}'", area);
			sb.AppendFormat(" Controller Name: '{0}'", controller);
			
			return sb.ToString();
		}
	}
}
