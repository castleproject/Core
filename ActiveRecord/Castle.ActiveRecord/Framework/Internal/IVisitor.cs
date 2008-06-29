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
	/// <summary>
	/// For implementign the visitor pattern.
	/// </summary>
	public interface IVisitor
	{
		/// <summary>
		/// Visits the top level of the model.
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitModel(ActiveRecordModel model);

		/// <summary>
		/// Visits the primary key.
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitPrimaryKey(PrimaryKeyModel model);

		/// <summary>
		/// Visits the composite primary key.
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitCompositePrimaryKey(CompositeKeyModel model);

		/// <summary>
		/// Visits the has many to any association
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitHasManyToAny(HasManyToAnyModel model);

		/// <summary>
		/// Visits any.
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitAny(AnyModel model);

		/// <summary>
		/// Visits the property.
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitProperty(PropertyModel model);

		/// <summary>
		/// Visits the field.
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitField(FieldModel model);

		/// <summary>
		/// Visits the component parent reference
		/// </summary>
		/// <param name="referenceModel">The model.</param>
		void VisitNestedParentReference(NestedParentReferenceModel referenceModel);

		/// <summary>
		/// Visits the version.
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitVersion(VersionModel model);

		/// <summary>
		/// Visits the timestamp.
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitTimestamp(TimestampModel model);

		/// <summary>
		/// Visits the key.
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitKey(KeyModel model);

		/// <summary>
		/// Visits the belongs to association
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitBelongsTo(BelongsToModel model);

		/// <summary>
		/// Visits the has many association
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitHasMany(HasManyModel model);

		/// <summary>
		/// Visits the one to one association
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitOneToOne(OneToOneModel model);

		/// <summary>
		/// Visits the has and belongs to many association
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitHasAndBelongsToMany(HasAndBelongsToManyModel model);

		/// <summary>
		/// Visits the hilo strategy
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitHilo(HiloModel model);

		/// <summary>
		/// Visits the nested (component) model
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitNested(NestedModel model);

		/// <summary>
		/// Visits the collection ID.
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitCollectionID(CollectionIDModel model);

		/// <summary>
		/// Visits the has many to any configuration
		/// </summary>
		/// <param name="hasManyToAnyConfigModel">The has many to any config model.</param>
		void VisitHasManyToAnyConfig(HasManyToAnyModel.Config hasManyToAnyConfigModel);

		/// <summary>
		/// Visits the import statement
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitImport(ImportModel model);

		/// <summary>
		/// Visits the dependent object model
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitDependentObject(DependentObjectModel model);

		/// <summary>
		/// Visits the custom composite user type.
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitCompositeUserType(CompositeUserTypeModel model);

		/// <summary>
		/// Visits the joined table configuration.
		/// </summary>
		/// <param name="model">The model.</param>
		void VisitJoinedTable(JoinedTableModel model);
	}
}