using System;

namespace Castle.MonoRail.Framework.Attributes
{
	/// <summary>
	/// 
	/// </summary>
	public enum ParamStore
	{
		QueryString,
		Form,
		Params
	}

	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage( AttributeTargets.Parameter, AllowMultiple=false, Inherited=true )]
	public class DataBindAttribute : Attribute
	{
		private string _Prefix		= string.Empty;
		private ParamStore _From	= ParamStore.Params;

		public DataBindAttribute()
		{
		}

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public ParamStore From
		{
			get { return _From; }
			set { _From = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Prefix
		{
			get { return _Prefix; }
			set { _Prefix = value; }
		}

		#endregion
	}
}
