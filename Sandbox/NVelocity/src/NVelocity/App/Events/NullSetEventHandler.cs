namespace NVelocity.App.Events
{
	using System;

	/// <summary>
	/// Lets an app approve / veto writing a log message when RHS of #set() is null.
	/// </summary>
	public delegate void NullSetEventHandler(Object sender, NullSetEventArgs e);
	
	public class NullSetEventArgs : EventArgs
	{
		private Boolean shouldLog = true;
		private String lhs, rhs;

		public NullSetEventArgs(String lhs, String rhs)
		{
			this.lhs = lhs;
			this.rhs = rhs;
		}
		
		/// <summary>
		/// reference literal of left-hand-side of set statement
		/// </summary>
		public String LHS { get { return lhs; } }

		/// <summary>
		/// reference literal of right-hand-side of set statement
		/// </summary>
		public String RHS { get { return rhs; } }

		public Boolean ShouldLog 
		{ 
			get { return shouldLog; } 
			set { shouldLog = value; } 
		}
	}
}