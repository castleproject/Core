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

	public enum EstratoType
	{
		Abstract,
		Reference,
		Survey
	}


	[ActiveRecord(DiscriminatorColumn="type", DiscriminatorType="Int16", DiscriminatorValue="0")]
	public abstract class Estrato : ActiveRecordValidationBase
	{
		private int id;
		private EstratoType type;
		private QuestionContainer container;
		private ISet references = new ListSet();

		public Estrato()
		{
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property("type", Insert=false, Update=false)]
		public EstratoType EstratoType
		{
			get { return type; }
			set { type = value; }
		}

		public QuestionContainer Container
		{
			get { return container != null ? container : ParentEstrato.Container; }
			set { container = value; }
		}

		[HasAndBelongsToMany( typeof(Estrato), Table="EstratoRefEstrato", ColumnRef="ref_estrato_id", ColumnKey="estrato_id" )]
		public ISet ReferencedEstratos
		{
			get { return references; }
			set { references = value; }
		}

		public abstract Estrato ParentEstrato
		{
			get; set;
		}

		public abstract ISet SubEstratos
		{
			get; set;
		}

		public bool IsLeaf
		{
			get { return SubEstratos == null || SubEstratos.Count == 0; }
		}

	}
}
