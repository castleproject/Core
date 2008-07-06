// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	/// Connects <see cref="ActiveRecordModel"/> with their parents 
	/// <see cref="ActiveRecordModel"/>
	/// </summary>
	public class GraphConnectorVisitor : AbstractDepthFirstVisitor
	{
		private readonly ActiveRecordModelCollection arCollection;
		
		private ActiveRecordModel currentModel;

		/// <summary>
		/// Initializes a new instance of the <see cref="GraphConnectorVisitor"/> class.
		/// </summary>
		/// <param name="arCollection">The ar collection.</param>
		public GraphConnectorVisitor(ActiveRecordModelCollection arCollection)
		{
			this.arCollection = arCollection;
		}

		/// <summary>
		/// Visits the model.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitModel(ActiveRecordModel model)
		{
			ActiveRecordModel savedModel = currentModel;

			try
			{
				currentModel = model;

				if (model.IsDiscriminatorBase || model.IsJoinedSubClassBase ||
					model.IsDiscriminatorSubClass || model.IsJoinedSubClass)
				{
					foreach(ActiveRecordModel child in arCollection)
					{
						if (IsChildClass(model, child))
						{
							child.IsDiscriminatorSubClass = model.ActiveRecordAtt.DiscriminatorValue != null;
							child.IsJoinedSubClass = child.Key != null;
							child.Parent = model;

							if (child.IsDiscriminatorSubClass)
							{
								// Needed for deep hierarchies
								if (model.Classes.Contains(child) == false)
								{
									// Discriminator subclass
									model.Classes.Add(child);
								}
							} 
							else if (child.IsJoinedSubClass)
							{
								// Needed for deep hierarchies
								if (model.JoinedClasses.Contains(child) == false)
								{
									// Joined subclass
									model.JoinedClasses.Add(child);
								}
							}
						}
					}
				}

				VisitNodes(model.JoinedClasses);
				VisitNodes(model.Classes);

				base.VisitModel(model);

				// They should have been connected by now
				model.CollectionIDs.Clear();
				model.Hilos.Clear();
			}
			finally
			{
				currentModel = savedModel;
			}
		}

		/// <summary>
		/// Visits the nested.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitNested(NestedModel model)
		{
			Type type = model.Property.DeclaringType;

			ActiveRecordModel parent = arCollection[type];

			model.Model.Parent = parent;

			base.VisitNested(model);
		}

		/// <summary>
		/// Visits the collection ID.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitCollectionID(CollectionIDModel model)
		{
			// Attempt to find HasAndBelongsToMany for the property
			
			HasAndBelongsToManyModel hasAndBelModel = FindHasAndBelongsToMany(model.Property);

			if (hasAndBelModel == null)
			{
				throw new ActiveRecordException( String.Format(
					"A CollectionIDAttribute should be used with HasAndBelongsToMany, but we couldn't find it for the property " + 
					currentModel.Type.FullName + "." + model.Property.Name) );
			}

			hasAndBelModel.CollectionID = model;
		}

		/// <summary>
		/// Visits the hilo model
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitHilo(HiloModel model)
		{
			// Attempt to find CollectionID for the property
			
			CollectionIDModel collModel = FindCollectionID(model.Property);

			if (collModel == null)
			{
				throw new ActiveRecordException( String.Format(
					"A HiloAttribute should be used with CollectionIDAttribute, but we couldn't find it for the property " + 
					currentModel.Type.FullName + "." + model.Property.Name) );
			}

			collModel.Hilo = model;
		}

		private CollectionIDModel FindCollectionID(PropertyInfo property)
		{
			foreach(CollectionIDModel model in currentModel.CollectionIDs)
			{
				if (model.Property == property)
				{
					return model;
				}
			}

			return null;
		}

		private HasAndBelongsToManyModel FindHasAndBelongsToMany(PropertyInfo property)
		{
			foreach(HasAndBelongsToManyModel model in currentModel.HasAndBelongsToMany)
			{
				if (model.Property == property)
				{
					return model;
				}
			}

			return null;
		}

		private static bool IsChildClass(ActiveRecordModel model, ActiveRecordModel child)
		{
			// generic check
			if (child.Type.BaseType.IsGenericType)
			{
				if (child.Type.BaseType.GetGenericTypeDefinition() == model.Type) return true;
			}

			// Direct decendant
			if (child.Type.BaseType == model.Type) return true;

			// Not related to each other
			if (!model.Type.IsAssignableFrom(child.Type)) return false;

			// The model is the ancestor of the child, but is it the direct AR ancestor?
			Type arAncestor = child.Type.BaseType;

			while(arAncestor != typeof(object) && ActiveRecordModel.GetModel(arAncestor) == null)
			{
				arAncestor = arAncestor.BaseType;
			}

			return arAncestor == model.Type;
		}
	}
}
