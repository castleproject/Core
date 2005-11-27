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

namespace PetStore.Service
{
	using System;

	using PetStore.Model;

	/// <summary>
	/// As you can see this service does not imeplement
	/// any service interface. This is OK for components
	/// you dont think you would need to offer more than one
	/// implementation. 
	/// Principles: YAGNI and KISS :-)
	/// </summary>
	public class CategoryService
	{
		private readonly ICategoryDataAccess categoryDataAccess;

		public CategoryService(ICategoryDataAccess categoryDataAccess)
		{
			this.categoryDataAccess = categoryDataAccess;
		}

		public Category[] ObtainCategories()
		{
			return categoryDataAccess.FindAll();
		}

		public Category Find(int category)
		{
			return categoryDataAccess.Find(category);
		}
	}
}
