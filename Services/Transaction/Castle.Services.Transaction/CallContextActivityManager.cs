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

namespace Castle.Services.Transaction
{
	using System;
	using System.Runtime.Remoting.Messaging;

	public class CallContextActivityManager : MarshalByRefObject, IActivityManager
	{
		private const string Key = "Castle.Services.Transaction.Activity";

		/// <summary>
		/// Initializes a new instance of the <see cref="CallContextActivityManager"/> class.
		/// </summary>
		public CallContextActivityManager()
		{
			CallContext.SetData(Key, null);
		}

		#region MarshalByRefObject

		///<summary>
		///Obtains a lifetime service object to control the lifetime policy for this instance.
		///</summary>
		///
		///<returns>
		///An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"></see> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"></see> property.
		///</returns>
		///
		///<exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception><filterpriority>2</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure" /></PermissionSet>
		public override object InitializeLifetimeService()
		{
			return null;
		}

		#endregion

		/// <summary>
		/// Gets the current activity.
		/// </summary>
		/// <value>The current activity.</value>
		public Activity CurrentActivity
		{
			get
			{
				Activity activity = (Activity) CallContext.GetData(Key);

				if (activity == null)
				{
					activity = new Activity();
					CallContext.SetData(Key, activity);
				}

				return activity;
			}
		}
	}
}
