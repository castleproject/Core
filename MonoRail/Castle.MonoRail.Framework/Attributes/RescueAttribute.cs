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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Reflection;

	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Associates a rescue template with a <see cref="Controller"/> or an action 
	/// (method). The rescue is invoked in response to some exception during the 
	/// action processing.
	/// </summary>
	/// <remarks>
	/// The view must exist in the <c>rescues</c> folder in your view folder
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple=true), Serializable]
	public class RescueAttribute : Attribute, IRescueDescriptorBuilder
	{
		private readonly String viewName;
		private readonly Type exceptionType;
		private readonly Type rescueController;
		private readonly MethodInfo rescueMethod;
		
		/// <summary>
		/// Constructs a RescueAttribute with the template name.
		/// </summary>
		/// <param name="viewName">The view to use in the event of error</param>
		public RescueAttribute(String viewName) : this(viewName, typeof(Exception))
		{			
		}

		/// <summary>
		/// Constructs a RescueAttribute with the template name and exception type.
		/// </summary>
		/// <param name="viewName">The view to use in the event of error</param>
		/// <param name="exceptionType">The exception to match</param>
		public RescueAttribute(String viewName, Type exceptionType)
		{
			if (viewName == null || viewName.Length == 0)
			{
				throw new ArgumentNullException("viewName");
			}
			
			if (exceptionType != null && !typeof(Exception).IsAssignableFrom(exceptionType))
			{
				throw new ArgumentException("exceptionType must be a type assignable from Exception");
			}
			
			this.viewName = viewName;
			this.exceptionType = exceptionType;
		}

		/// <summary>
		/// Use a controller to perform any actions on rescue. The view defaults to the rescues views.
		/// </summary>
		/// <param name="rescueController">The controller to use to perform the rescue</param>
		public RescueAttribute(Type rescueController) : this(rescueController, typeof(Exception))
		{
		}

		/// <summary>
		/// Use a controller to perform any actions on rescue. The view defaults to the rescues views.
		/// </summary>
		/// <param name="rescueController">The controller to use to perform the rescue</param>
		/// <param name="exceptionType">The exception to rescue for</param>
		public RescueAttribute(Type rescueController, Type exceptionType) : this(rescueController, null, exceptionType)
		{
		}

		/// <summary>
		/// Use a controller to perform any actions on rescue. The view defaults to the rescues views.
		/// </summary>
		/// <param name="rescueController">The controller to use to perform the rescue</param>
		/// <param name="rescueMethod">The method on the controller to use</param>
		public RescueAttribute(Type rescueController, string rescueMethod)
			: this(rescueController, rescueMethod, typeof(Exception))
		{
		}

		/// <summary>
		/// Use a controller to perform any actions on rescue. The view defaults to the rescues views.
		/// </summary>
		/// <param name="rescueController">The controller to use to perform the rescue</param>
		/// <param name="exceptionType">The exception to rescue for</param>
		/// <param name="rescueMethod">The method on the controller to use</param>
		public RescueAttribute(Type rescueController, string rescueMethod, Type exceptionType)
		{
			if (!typeof(IRescueController).IsAssignableFrom(rescueController) && rescueMethod == null)
			{
				throw new ArgumentException(string.Format("{0} does not implement {1}, and there is no rescueMethod defined. " + 
					"You can either inform a method to use through the 'rescueMethod' parameter or make the " + 
					"controller implement the IRescueController interface" , rescueController.Name, typeof(IRescueController).Name));
			}

			if (!typeof(Controller).IsAssignableFrom(rescueController))
			{
				throw new ArgumentException(string.Format("{0} does not inherit from the Controller class", rescueController.Name));
			}


			this.rescueController = rescueController;
			this.exceptionType = exceptionType;
			this.rescueMethod = (rescueMethod != null ? rescueController.GetMethod(rescueMethod) : null);
		}

		/// <summary>
		/// Gets the view name to use
		/// </summary>
		public String ViewName
		{
			get { return viewName; }
		}
		
		/// <summary>
		/// Gets the exception type
		/// </summary>
		public Type ExceptionType
		{
			get { return exceptionType; }
		}

		/// <summary>
		/// The controller to use for rescue
		/// </summary>
		public Type RescueController
		{
			get { return rescueController; }
		}

		/// <summary>
		/// The method on the rescue controller to use
		/// </summary>
		public MethodInfo RescueMethod
		{
			get { return rescueMethod; }
		}

		/// <summary>
		/// <see cref="IRescueDescriptorBuilder"/> implementation. 
		/// Builds the rescue descriptors.
		/// </summary>
		/// <returns></returns>
		public RescueDescriptor[] BuildRescueDescriptors()
		{
			if (rescueController != null)
			{
				MethodInfo method = (rescueMethod ?? typeof(IRescueController).GetMethod("Rescue"));
				return new RescueDescriptor[] { new RescueDescriptor(rescueController, method, ExceptionType) };
			}

			return new RescueDescriptor[] { new RescueDescriptor(viewName, exceptionType) };
		}
	}
}
