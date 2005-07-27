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

	[ActiveRecord(DiscriminatorValue="2")]
	public class SurveyEstrato : Estrato
	{
		public SurveyEstrato()
		{
		}

		public SurveyEstrato(SurveyEstrato parentEstrato) : this(parentEstrato, null)
		{
		}

		public SurveyEstrato(Survey survey) : this(null, survey)
		{
		}

		public SurveyEstrato(SurveyEstrato parentEstrato, Survey survey)
		{
			if (survey == null && parentEstrato == null)
			{
				throw new ArgumentNullException(
					"You must specify either a survey or a parent estrato " + 
					"in order to create an estrato instance");
			}

			this.EstratoType = EstratoType.Survey;

			this.ParentEstrato = parentEstrato;

			if (survey == null)
			{
				this.Container = parentEstrato.Survey;
			}
			else
			{
				this.Container = survey;
			}			
		}

		public Survey Survey
		{
			get { return (Survey) Container; }
			set { Container = value; }
		}

		#region Static methods

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(SurveyEstrato) );
		}

		public static SurveyEstrato Find(int id)
		{
			return (SurveyEstrato) ActiveRecordBase.FindByPrimaryKey( typeof(SurveyEstrato), id );
		}

		public static SurveyEstrato[] FindAll()
		{
			return (SurveyEstrato[]) ActiveRecordBase.FindAll( typeof(SurveyEstrato) );
		}

		#endregion
	}
}
