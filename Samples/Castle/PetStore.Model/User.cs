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

namespace PetStore.Model
{
	using System;
	using System.Collections.Generic;
	using System.Security.Principal;

	using Castle.ActiveRecord;
	using Castle.Components.Validator;
	using NHibernate.Criterion;

	[ActiveRecord("`User`", 
		DiscriminatorColumn = "type", 
		DiscriminatorType = "String", 
		DiscriminatorValue = "user")]
	public class User : IPrincipal, IAggregateRoot
	{
		[PrimaryKey(PrimaryKeyType.GuidComb)]
		public virtual Guid Id { get; protected set; }

		[Property(NotNull = true)]
		[ValidateNonEmpty]
		[ValidateIsUnique]
		public virtual string Login { get; set; }

		[Property(NotNull = true)]
		[ValidateNonEmpty]
		public virtual string Name { get; set; }

		[Property(NotNull = true)]
		[ValidateNonEmpty]
		[ValidateEmail]
		public virtual string Email { get; set; }

		[Property(NotNull = true)]
		[ValidateNonEmpty]
		[ValidateLength(8,20)]
		public virtual string Password { get; set; }

		public bool IsInRole(string role)
		{
			// We do not implement this functionality
			return false;
		}

		public IIdentity Identity
		{
			get { return new GenericIdentity(Name, "castle.authentication"); }
		}
	}
}
