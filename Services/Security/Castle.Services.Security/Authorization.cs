namespace Castle.Services.Security
{
    using System;

    /// <summary>
	/// Summary description for Authorization.
	/// </summary>
	public struct Authorization
	{
		public Authorization(bool authorized)
		{
            this._authorized = authorized;
            this._reason = "";
		}

	    private bool _authorized;

	    public bool Authorized
	    {
	        get { return this._authorized; }
	    }

	    private String _reason;

	    public String Reason
	    {
	        get { return this._reason; }
	    }
	}
}
