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

namespace Castle.Facilities.NHibernateIntegration
{
	using System;

#if DOTNET2
	using System.Collections.Generic;
#endif

	/// <summary>
	/// Summary description for IGenericDao.
	/// </summary>
	/// <remarks>
	/// Contributed by Steve Degosserie <steve.degosserie@vn.netika.com>
	/// Ported to .net2 by Ernst Naezer <ernst@auxilium.nl>
	/// </remarks>
#if DOTNET2
    public interface IGenericDao<T>
    {
        T[] FindAll();

        T[] FindAll(int firstRow, int maxRows);

        T FindById(object id);

        T Create(T instance);

        void Update(T instance);

        void Delete(T instance);

        void DeleteAll();

        void Save(T instance);
    }
#else
	/// <summary>
	/// Summary description for IGenericDao.
	/// </summary>
	/// <remarks>
	/// Contributed by Steve Degosserie <steve.degosserie@vn.netika.com>
	/// </remarks>
	public interface IGenericDao
	{
		Array FindAll(Type type);
		Array FindAll(Type type, int firstRow, int maxRows);

		object FindById(Type type, object id);

		object Create(object instance);

		void Update(object instance);

		void Delete(object instance);

		void DeleteAll(Type type);

		void Save(object instance);
	}
#endif
}