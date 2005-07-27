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

	public enum ContainerType
	{
		Abstract,
		Repository,
		Survey
	}

	[ActiveRecord(DiscriminatorColumn="type", DiscriminatorType="Int16", DiscriminatorValue="0")]
	public class QuestionContainer : ActiveRecordValidationBase
	{
		private int id;
		private ContainerType type;
		private ISet estratos = new ListSet();

		public QuestionContainer()
		{
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property("type", Insert=false, Update=false)]
		public ContainerType Type
		{
			get { return type; }
			set { type = value; }
		}

		[HasMany( typeof(Estrato), Inverse=true)]
		public ISet Estratos
		{
			get { return estratos; }
			set { estratos = value; }
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(QuestionContainer) );
		}

		public static QuestionContainer Find(int id)
		{
			return (QuestionContainer) ActiveRecordBase.FindByPrimaryKey( typeof(QuestionContainer), id );
		}

		public override bool Equals(object obj)
		{
			QuestionContainer rhs = obj as QuestionContainer;
			if (rhs != null)
			{
				return rhs.id == id;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return id.GetHashCode();
		}
	}
}
