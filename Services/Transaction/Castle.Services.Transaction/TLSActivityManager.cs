namespace Castle.Services.Transaction
{
	using System;
	using System.Threading;

	public class TLSActivityManager : MarshalByRefObject, IActivityManager
	{
		private const string Key = "Castle.Services.Transaction.TLSActivity";

		private object lockObj = new object();
		private static LocalDataStoreSlot dataSlot;

		static TLSActivityManager()
		{
			dataSlot = Thread.AllocateNamedDataSlot(Key);
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
				lock (lockObj)
				{
					Activity activity = (Activity) Thread.GetData(dataSlot);

					if (activity == null)
					{
						activity = new Activity();
						Thread.SetData(dataSlot, activity);
					}

					return activity;
				}
			}
		}
	}
}
