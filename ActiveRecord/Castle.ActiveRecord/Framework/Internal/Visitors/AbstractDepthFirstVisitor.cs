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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;


	public abstract class AbstractDepthFirstVisitor : IVisitor
	{
		private IDictionary visited = new HybridDictionary(100); 

		public void VisitNode(IVisitable visitable)
		{
			if (visitable == null) return;

			visitable.Accept(this);
		}

		public void VisitNodes(IEnumerable nodes)
		{
			foreach(IVisitable visitable in nodes)
			{
				VisitNode(visitable);
			}
		}

		public virtual void VisitModel(ActiveRecordModel model)
		{
			if (!visited.Contains(model))
			{
				visited.Add(model, String.Empty);
			}
			else
			{
				return;
			}

			VisitNodes( model.Ids );
			VisitNode( model.Key );
			VisitNode( model.Version );
			VisitNode( model.Timestamp );
			VisitNodes( model.JoinedClasses );
			VisitNodes( model.Classes );
			VisitNodes( model.Fields );
			VisitNodes( model.Anys );
			VisitNodes( model.Properties );
			VisitNodes( model.OneToOnes );
			VisitNodes( model.BelongsTo );
			VisitNodes( model.HasMany );
			VisitNodes( model.HasAndBelongsToMany );
			VisitNodes( model.HasManyToAny );
			VisitNodes( model.CollectionIDs );
			VisitNodes( model.Hilos );
			VisitNodes( model.Components );
		}

		public virtual void VisitPrimaryKey(PrimaryKeyModel model)
		{
		}


		public virtual void VisitHasManyToAny(HasManyToAnyModel model)
		{
		}

		public virtual void VisitProperty(PropertyModel model)
		{
		}

		public virtual void VisitField(FieldModel model)
		{
		}

		public virtual void VisitAny(AnyModel model)
		{
		}

		public virtual void VisitVersion(VersionModel model)
		{
		}

		public virtual void VisitTimestamp(TimestampModel model)
		{
		}

		public virtual void VisitKey(KeyModel model)
		{
		}

		public virtual void VisitBelongsTo(BelongsToModel model)
		{
		}

		public virtual void VisitHasMany(HasManyModel model)
		{
		}

		public virtual void VisitOneToOne(OneToOneModel model)
		{
		}

		public virtual void VisitHasAndBelongsToMany(HasAndBelongsToManyModel model)
		{
		}

		public virtual void VisitHilo(HiloModel model)
		{
		}

		public virtual void VisitNested(NestedModel model)
		{
			VisitNode(model.Model);
		}

		public virtual void VisitCollectionID(CollectionIDModel model)
		{
			VisitNode(model.Hilo);
		}

		public virtual void VisitHasManyToAnyConfig(HasManyToAnyModel.Config hasManyToAnyConfigModel)
		{
		}
	}
}
