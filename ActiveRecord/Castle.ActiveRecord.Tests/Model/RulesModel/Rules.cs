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

namespace Castle.ActiveRecord.Tests.Model.RulesModel
{
    using System;
    using System.Text;

    /// <summary>
    /// Base class that is not persisted to database
    /// </summary>
    public abstract class RuleBase
    {
        int count;

        /// <summary>
        /// Property persisted to database even though the class itself is not.
        /// </summary>
        [Property]
        public int Count
        {
            get { return count; }
            set { count = value; }
        }
    }

    /// <summary>
    /// Base class that is persisted to database, 
    /// </summary>
    [ActiveRecord(DiscriminatorColumn="discriminator", DiscriminatorValue="0")]
    public abstract class PersistedRule : RuleBase
    {
        int id;

        [PrimaryKey(PrimaryKeyType.Native)]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
    }

    /// <summary>
    /// Another step in the way, this one is not persisted, but has properties that will be persisted
    /// </summary>
    public abstract class EmployeeRule : PersistedRule
    {
        string name;

        [Property]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }

    [ActiveRecord(DiscriminatorValue="2")]
    public class WorkDaysRules : EmployeeRule
    {
        int days;

        [Property]
        public int Days
        {
            get { return days; }
            set { days = value; }
        }
    }
}
