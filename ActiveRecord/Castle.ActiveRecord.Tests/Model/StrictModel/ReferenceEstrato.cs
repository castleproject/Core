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

namespace Castle.ActiveRecord.Tests.Model.StrictModel
{
	using System;

	using Iesi.Collections;


	[ActiveRecord(DiscriminatorValue="1")]
	public class ReferenceEstrato : Estrato
	{
		public ReferenceEstrato()
		{
		}

		public ReferenceEstrato(ReferenceEstrato parentEstrato) : this(parentEstrato, null)
		{
		}

		public ReferenceEstrato(Repository repository) : this(null, repository)
		{
		}

		public ReferenceEstrato(ReferenceEstrato parentEstrato, Repository repository)
		{		
			if (repository == null && parentEstrato == null)
			{
				throw new ArgumentNullException(
					"You must specify either a repository or a parent estrato " + 
					"in order to create an estrato instance");
			}

			this.EstratoType = EstratoType.Survey;

			this.ParentEstrato = parentEstrato;

			if (repository == null)
			{
				this.Container = parentEstrato.Repository;
			}
			else
			{
				this.Container = repository;
			}
		}

		public Repository Repository
		{
			get { return (Repository) Container; }
			set { Container = value; }
		}

		#region Static methods

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(ReferenceEstrato) );
		}

		public static ReferenceEstrato Find(int id)
		{
			return (ReferenceEstrato) ActiveRecordBase.FindByPrimaryKey( typeof(ReferenceEstrato), id );
		}

		public static ReferenceEstrato[] FindAll()
		{
			return (ReferenceEstrato[]) ActiveRecordBase.FindAll( typeof(ReferenceEstrato) );
		}

		#endregion
	}
}
