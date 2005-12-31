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

namespace Castle.MonoRail.ActiveRecordSupport
{
	using System;

	using Castle.MonoRail.Framework;

	/// <summary>
	/// This attribute tells <see cref="ARSmartDispatcherController" />
	/// to fetches the ActiveRecord based on its Primary Key.
	/// </summary>
	/// <remarks>
	/// The <see cref="ARFetchAttribute"/> only loads an instance
	/// based on the primary key value obtained from <see cref="IRailsEngineContext.Params"/>
	/// <para>For example:</para>
	/// <code>
	/// public class CustomerController : ARSmartDispatcherController
	/// {
	///     public void UpdateCustomerLocation([ARFetch("customer.id")] Customer customer, [ARFetch("location.id")] Location location)
	///     {
	///       customer.Location = location;
	///       customer.Save();
	///       
	///       RedirectToAction("index");
	///     }
	/// }
	/// </code>
	/// The code above assumes that you have the fields 
	/// <c>customer.id</c> and <c>location.id</c> on the form being
	/// submitted. 
	/// </remarks>
	[AttributeUsage(AttributeTargets.Parameter), Serializable]
	public class ARFetchAttribute : Attribute
	{
		private String requestParameterName;
		private bool create, required;
		
		/// <summary>
		/// Constructs an <see cref="ARFetchAttribute"/> 
		/// specifying the parameter name and the create and require behavior
		/// </summary>
		/// <param name="requestParameterName">The parameter name to be read from the request</param>
		/// <param name="create"><c>true</c> if you want an instance even when the record is not found</param>
		/// <param name="required"><c>true</c> if you want an exception if the record is not found</param>
		public ARFetchAttribute(String requestParameterName, bool create, bool required) : base()
		{
			this.requestParameterName = requestParameterName;
			this.create = create;
			this.required = required;
		}

		/// <summary>
		/// Constructs an <see cref="ARFetchAttribute"/> using the
		/// parameter name as the <see cref="ARFetchAttribute.RequestParameterName"/>
		/// </summary>
		public ARFetchAttribute() : this(null, false, false)
		{
		}
		
		/// <summary>
		/// Constructs an <see cref="ARFetchAttribute"/> specifing the
		/// parameter name
		/// <seealso cref="ARFetchAttribute.RequestParameterName"/>
		/// </summary>
		public ARFetchAttribute(String requestParameterName) : this(requestParameterName, false, false)
		{
		}
		
		/// <summary>
		/// Constructs an <see cref="ARFetchAttribute"/> using the
		/// parameter name as the <see cref="ARFetchAttribute.RequestParameterName"/>
		/// and the create and require behavior
		/// </summary>
		/// <param name="create"><c>true</c> if you want an instance even when the record is not found</param>
		/// <param name="require"><c>true</c> if you want an exception if the record is not found</param>
		public ARFetchAttribute(bool create, bool require) : this(null, create, require)
		{
		}
		
		/// <summary>
		/// The parameter name to be read from the request. The parameter value will 
		/// be used as the primary key value to load the target object instance.
		/// </summary>
		public String RequestParameterName
		{
			get { return requestParameterName; }
			set { requestParameterName = value; }
		}

		/// <summary>
		/// When set to <c>true</c> an instance of
		/// the target type will be created if the record 
		/// is not found
		/// </summary>
		public bool Create
		{
			get { return create; }
			set { create = value; }
		}
		
		/// <summary>
		/// When set to <c>true</c> the record must be found
		/// or an exception will be thrown
		/// </summary>
		public bool Required
		{
			get { return required; }
			set { required = value; }
		}
	}
}
