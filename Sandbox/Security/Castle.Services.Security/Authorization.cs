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
