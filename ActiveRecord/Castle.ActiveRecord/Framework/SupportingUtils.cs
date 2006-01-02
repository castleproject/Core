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

namespace Castle.ActiveRecord.Framework
{
	using System;
	using System.Collections;

	using NHibernate;

	/// <summary>
	/// Usefull for external frameworks
	/// </summary>
	public abstract class SupportingUtils
	{
		public static IList FindAll( Type type )
		{
			ISession session = ActiveRecordBase._holder.CreateSession( type );

			try
			{
				ICriteria criteria = session.CreateCriteria( type );

				return criteria.List();
			}
			finally
			{
				ActiveRecordBase._holder.ReleaseSession(session);
			}
		}

		public static object FindByPK( Type type, object id )
		{
			return FindByPK(type, id, true);
		}

		public static object FindByPK( Type type, object id, bool throwOnNotFound )
		{
			ISession session = ActiveRecordBase._holder.CreateSession( type );

			try
			{
				return session.Load( type, id );
			}
			catch(ObjectNotFoundException ex)
			{
				if (throwOnNotFound)
				{
					String message = String.Format("Could not find {0} with id {1}", type.Name, id);
					throw new NotFoundException(message, ex);
				}

				return null;
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException("Could not perform Load (Find by PK) for " + type.Name, ex);
			}
			finally
			{
				ActiveRecordBase._holder.ReleaseSession(session);
			}
		}
	}
}
