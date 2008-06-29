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
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Base class for visitors that needs to traverse the entire Active Record Model
	/// </summary>
	public abstract class AbstractDepthFirstVisitor : IVisitor
	{
		private readonly IDictionary<ActiveRecordModel, String> visited = new Dictionary<ActiveRecordModel, String>(100);

		/// <summary>
		/// Visits the node.
		/// </summary>
		/// <param name="visitable">The visitable.</param>
		public void VisitNode(IVisitable visitable)
		{
			if (visitable == null) return;

			visitable.Accept(this);
		}

		/// <summary>
		/// Visits the nodes.
		/// </summary>
		/// <param name="nodes">The nodes.</param>
		public void VisitNodes(IEnumerable nodes)
		{
			foreach(IVisitable visitable in nodes)
			{
				VisitNode(visitable);
			}
		}

		/// <summary>
		/// Visits the model.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitModel(ActiveRecordModel model)
		{
			if (!visited.ContainsKey(model))
			{
				visited.Add(model, String.Empty);
			}
			else
			{
				return;
			}

			VisitNode(model.PrimaryKey);
			VisitNode(model.CompositeKey);
			VisitNode(model.Key);
			VisitNode(model.Version);
			VisitNode(model.Timestamp);
			VisitNodes(model.JoinedClasses);
			VisitNodes(model.JoinedTables);
			VisitNodes(model.BelongsTo);
			VisitNodes(model.Classes);
			VisitNodes(model.Fields);
			VisitNodes(model.Anys);
			VisitNodes(model.Properties);
			VisitNodes(model.OneToOnes);
			VisitNodes(model.HasMany);
			VisitNodes(model.HasAndBelongsToMany);
			VisitNodes(model.HasManyToAny);
			VisitNodes(model.CollectionIDs);
			VisitNodes(model.Hilos);
			VisitNodes(model.Components);
			VisitNodes(model.CompositeUserType);
		}

		/// <summary>
		/// Visits the primary key.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitPrimaryKey(PrimaryKeyModel model)
		{
		}

		/// <summary>
		/// Visits the composite primary key.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitCompositePrimaryKey(CompositeKeyModel model)
		{
		}

		/// <summary>
		/// Visits the has many to any.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitHasManyToAny(HasManyToAnyModel model)
		{
		}

		/// <summary>
		/// Visits the property.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitProperty(PropertyModel model)
		{
		}

		/// <summary>
		/// Visits the field.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitField(FieldModel model)
		{
		}

		/// <summary>
		/// Visits the component parent
		/// </summary>
		/// <param name="referenceModel">The model.</param>
		public virtual void VisitNestedParentReference(NestedParentReferenceModel referenceModel)
		{
		}

		/// <summary>
		/// Visits any.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitAny(AnyModel model)
		{
		}

		/// <summary>
		/// Visits the version.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitVersion(VersionModel model)
		{
		}

		/// <summary>
		/// Visits the timestamp.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitTimestamp(TimestampModel model)
		{
		}

		/// <summary>
		/// Visits the key.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitKey(KeyModel model)
		{
		}

		/// <summary>
		/// Visits the belongs to.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitBelongsTo(BelongsToModel model)
		{
		}

		/// <summary>
		/// Visits the has many.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitHasMany(HasManyModel model)
		{
		}

		/// <summary>
		/// Visits the one to one.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitOneToOne(OneToOneModel model)
		{
		}

		/// <summary>
		/// Visits the has and belongs to many.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitHasAndBelongsToMany(HasAndBelongsToManyModel model)
		{
		}

		/// <summary>
		/// Visits the hilo.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitHilo(HiloModel model)
		{
		}

		/// <summary>
		/// Visits the nested.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitNested(NestedModel model)
		{
			VisitNode(model.Model);
		}

		/// <summary>
		/// Visits the collection ID.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitCollectionID(CollectionIDModel model)
		{
			VisitNode(model.Hilo);
		}

		/// <summary>
		/// Visits the has many to any config.
		/// </summary>
		/// <param name="hasManyToAnyConfigModel">The has many to any config model.</param>
		public virtual void VisitHasManyToAnyConfig(HasManyToAnyModel.Config hasManyToAnyConfigModel)
		{
		}

		/// <summary>
		/// Visits the import.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitImport(ImportModel model)
		{
		}

		/// <summary>
		/// Visits the Dependent Object à
		/// </summary>
		/// <param name="model">The model</param>
		public virtual void VisitDependentObject(DependentObjectModel model)
		{
			VisitNode(model.Model);
		}

		/// <summary>
		/// Visits the custom composite user type.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitCompositeUserType(CompositeUserTypeModel model)
		{
		}

		/// <summary>
		/// Visits the joined table configuration.
		/// </summary>
		/// <param name="model">The model.</param>
		public virtual void VisitJoinedTable(JoinedTableModel model)
		{
		}

		/// <summary>
		/// Guesses the type of the other end.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="propertyType">Type of the property.</param>
		/// <returns></returns>
		public static Type GuessType(Type type, Type propertyType)
		{
			Type otherend = type;

			if (otherend == null)
			{
				// naive guessing of type if not specified
				if (propertyType.IsGenericType)
				{
					Type[] arguments = propertyType.GetGenericArguments();
					
					if (arguments.Length == 1)
					{
						otherend = arguments[0];
					}
				}
			}
			return otherend;
		}
	}
}