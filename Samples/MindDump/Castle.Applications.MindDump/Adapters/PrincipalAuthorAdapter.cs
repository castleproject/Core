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

namespace Castle.Applications.MindDump.Adapters
{
	using System;
	using System.Security.Principal;

	using Castle.Applications.MindDump.Model;


	public class PrincipalAuthorAdapter : IPrincipal, IIdentity
	{
		private Author _author;

		public PrincipalAuthorAdapter(Author author)
		{
			_author = author;
		}

		public Author Author
		{
			get { return _author; }
		}

		#region IPrincipal

		public bool IsInRole(String role)
		{
			return false;
		}

		public IIdentity Identity
		{
			get { return this; }
		}

		#endregion

		public string Name
		{
			get { return _author.Name; }
		}

		public string AuthenticationType
		{
			get { return "Castle"; }
		}

		public bool IsAuthenticated
		{
			get { return true; }
		}
	}
}
