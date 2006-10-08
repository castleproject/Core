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

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
#if DOTNET2
	using NHibernate.Type;

	[ActiveRecord(Lazy=false)]
    public class EnumTestClass : ActiveRecordBase
    {
        private int id;
        private EnumVal _noType;
        private EnumVal _withType;

        [PrimaryKey]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        
        [Property]
        public EnumVal NoColumnType
        {
             get { return _noType; }
             set { _noType = value; }
        }

        [Property(ColumnType = "Castle.ActiveRecord.Framework.Internal.Tests.Model.GenericEnumStringType`1[[Castle.ActiveRecord.Framework.Internal.Tests.Model.EnumVal, Castle.ActiveRecord.Framework.Internal.Tests]], Castle.ActiveRecord.Framework.Internal.Tests")]
        public EnumVal WithColumnType
        {
            get { return _withType; }
            set { _withType = value; }
        }
    }
    
    public enum EnumVal
    {
        One,
        Two
    }

    public class GenericEnumStringType<T> : EnumStringType
    {
        public GenericEnumStringType()
            : base(typeof(T), 30)
        {
        }
    }
#endif
}
