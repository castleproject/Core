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
	using System.Collections.Generic;
	using System.Reflection;

	/// <summary>
	/// Dispatches the extension invocations to the inner extension list.
	/// </summary>
	public sealed class ModelBuilderExtensionComposite : IModelBuilderExtension
	{
		private readonly IList<IModelBuilderExtension> extensions;

		/// <summary>
		/// Initializes a new instance of the <see cref="ModelBuilderExtensionComposite"/> class.
		/// </summary>
		/// <param name="extensions">The extensions.</param>
		public ModelBuilderExtensionComposite(IList<IModelBuilderExtension> extensions)
		{
			this.extensions = extensions;
		}

		/// <summary>
		/// Dispatches the call to the extensions.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="model">The model.</param>
		public void ProcessClass(Type type, ActiveRecordModel model)
		{
			foreach(IModelBuilderExtension extension in extensions)
			{
				extension.ProcessClass(type, model);
			}
		}

		/// <summary>
		/// Dispatches the call to the extensions.
		/// </summary>
		/// <param name="pi">The property info reflection object.</param>
		/// <param name="model">The model.</param>
		public void ProcessProperty(PropertyInfo pi, ActiveRecordModel model)
		{
			foreach(IModelBuilderExtension extension in extensions)
			{
				extension.ProcessProperty(pi, model);
			}
		}

		/// <summary>
		/// Dispatches the call to the extensions.
		/// </summary>
		/// <param name="fi">The field info reflection object.</param>
		/// <param name="model">The model.</param>
		public void ProcessField(FieldInfo fi, ActiveRecordModel model)
		{
			foreach(IModelBuilderExtension extension in extensions)
			{
				extension.ProcessField(fi, model);
			}
		}

		/// <summary>
		/// Dispatches the call to the extensions.
		/// </summary>
		/// <param name="pi">The property info reflection object.</param>
		/// <param name="belongsToModel">The belongs to model.</param>
		/// <param name="model">The model.</param>
		public void ProcessBelongsTo(PropertyInfo pi, BelongsToModel belongsToModel, ActiveRecordModel model)
		{
			foreach(IModelBuilderExtension extension in extensions)
			{
				extension.ProcessBelongsTo(pi, belongsToModel, model);
			}
		}

		/// <summary>
		/// Dispatches the call to the extensions.
		/// </summary>
		/// <param name="pi">The property info reflection object.</param>
		/// <param name="hasManyModel">The has many model.</param>
		/// <param name="model">The model.</param>
		public void ProcessHasMany(PropertyInfo pi, HasManyModel hasManyModel, ActiveRecordModel model)
		{
			foreach(IModelBuilderExtension extension in extensions)
			{
				extension.ProcessHasMany(pi, hasManyModel, model);
			}
		}

		/// <summary>
		/// Dispatches the call to the extensions.
		/// </summary>
		/// <param name="pi">The property info reflection object.</param>
		/// <param name="hasManyModel">The has many model.</param>
		/// <param name="model">The model.</param>
		public void ProcessHasManyToAny(PropertyInfo pi, HasManyToAnyModel hasManyModel, ActiveRecordModel model)
		{
			foreach(IModelBuilderExtension extension in extensions)
			{
				extension.ProcessHasManyToAny(pi, hasManyModel, model);
			}
		}

		/// <summary>
		/// Dispatches the call to the extensions.
		/// </summary>
		/// <param name="pi">The property info reflection object.</param>
		/// <param name="hasAndBelongManyModel">The has and belong many model.</param>
		/// <param name="model">The model.</param>
		public void ProcessHasAndBelongsToMany(PropertyInfo pi, HasAndBelongsToManyModel hasAndBelongManyModel,
		                                       ActiveRecordModel model)
		{
			foreach(IModelBuilderExtension extension in extensions)
			{
				extension.ProcessHasAndBelongsToMany(pi, hasAndBelongManyModel, model);
			}
		}
	}
}