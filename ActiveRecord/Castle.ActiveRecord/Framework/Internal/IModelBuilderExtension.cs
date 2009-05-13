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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Gives a chance to external frameworks to plug into 
	/// the AR model builder process. Particularly useful to 
	/// inspect attributes and conventions outside the AR domain.
	/// </summary>
	public interface IModelBuilderExtension
	{
		/// <summary>
		/// Gives implementors a chance to process the class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="model">The model.</param>
		void ProcessClass(Type type, ActiveRecordModel model);

		/// <summary>
		/// Gives implementors a chance to process the property.
		/// </summary>
		/// <param name="pi">The property info reflection object.</param>
		/// <param name="model">The model.</param>
		void ProcessProperty(PropertyInfo pi, ActiveRecordModel model);

		/// <summary>
		/// Gives implementors a chance to process the field.
		/// </summary>
		/// <param name="fi">The field info reflection object.</param>
		/// <param name="model">The model.</param>
		void ProcessField(FieldInfo fi, ActiveRecordModel model);

		/// <summary>
		/// Gives implementors a chance to process the BelongsTo.
		/// </summary>
		/// <param name="pi">The property info reflection object.</param>
		/// <param name="belongsToModel">The belongs to model.</param>
		/// <param name="model">The model.</param>
		void ProcessBelongsTo(PropertyInfo pi, BelongsToModel belongsToModel, ActiveRecordModel model);

		/// <summary>
		/// Gives implementors a chance to process the HasMany.
		/// </summary>
		/// <param name="pi">The property info reflection object.</param>
		/// <param name="hasManyModel">The has many model.</param>
		/// <param name="model">The model.</param>
		void ProcessHasMany(PropertyInfo pi, HasManyModel hasManyModel, ActiveRecordModel model);

		/// <summary>
		/// Gives implementors a chance to process the HasManyToAny.
		/// </summary>
		/// <param name="pi">The property info reflection object.</param>
		/// <param name="hasManyModel">The has many model.</param>
		/// <param name="model">The model.</param>
		void ProcessHasManyToAny(PropertyInfo pi, HasManyToAnyModel hasManyModel, ActiveRecordModel model);

		/// <summary>
		/// Gives implementors a chance to process the HasAndBelongsToMany.
		/// </summary>
		/// <param name="pi">The property info reflection object.</param>
		/// <param name="hasAndBelongManyModel">The has and belong many model.</param>
		/// <param name="model">The model.</param>
		void ProcessHasAndBelongsToMany(PropertyInfo pi, HasAndBelongsToManyModel hasAndBelongManyModel, ActiveRecordModel model);
	}
}
