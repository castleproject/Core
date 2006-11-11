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
		private String area, controller;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerNotFoundException"/> class.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		public ControllerNotFoundException(String area, String controller) : base(BuildExceptionMessage(area, controller))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerNotFoundException"/> class.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="innerException">The inner exception.</param>
		public ControllerNotFoundException(String area, String controller, Exception innerException)
			: base(BuildExceptionMessage(area, controller), innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerNotFoundException"/> class.
		/// </summary>
		/// <param name="url">The URL.</param>
		public ControllerNotFoundException(UrlInfo url) : this(url.Area, url.Controller)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerNotFoundException"/> class.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="innerException">The inner exception.</param>
		public ControllerNotFoundException(UrlInfo url, Exception innerException)
			: this(url.Area, url.Controller, innerException)
		{
		}

		/// <summary>
		/// Gets the area name.
		/// </summary>
		/// <value>The area name.</value>
		public String Area
		{
			get { return area; }
		}

		/// <summary>
		/// Gets the controller name.
		/// </summary>
		/// <value>The controller name.</value>
		public String Controller
		{
			get { return controller; }
		}

		#region Serialization Support

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerNotFoundException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected ControllerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			area = info.GetString("rails.area");
			controller = info.GetString("rails.controller");
		}

		/// <summary>
		/// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/>
		/// with information about the exception.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (<see langword="Nothing"/> in Visual Basic).</exception>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("rails.area", area);
			info.AddValue("rails.controller", controller);
		}

		#endregion

		/// <summary>
		/// Builds the exception message.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <returns></returns>
		private static String BuildExceptionMessage(String area, String controller)
		{
			StringBuilder sb = new StringBuilder("Controller not found.");

			sb.AppendFormat(" Area: '{0}'", area);
			sb.AppendFormat(" Controller Name: '{0}'", controller);

			return sb.ToString();
		}
	}
}