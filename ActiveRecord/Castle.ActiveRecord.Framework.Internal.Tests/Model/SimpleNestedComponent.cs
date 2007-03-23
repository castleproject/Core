// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
    [ActiveRecord]
    public class SimpleNestedComponent : ActiveRecordBase
    {
        private int id;

        [PrimaryKey]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private NestedComponent _nested;

        [Nested]
        public NestedComponent Nested
        {
            get
            {
                return this._nested;
            }
            set
            {
                this._nested = value;
            }
        }

    }

    public class NestedComponent
    {
        private SimpleNestedComponent _parent;

        [NestedParentReference]
        public SimpleNestedComponent Parent
        {
            get
            {
                return this._parent;
            }
            set
            {
                this._parent = value;
            }
        }

        private string _nestedProperty = "";
        
        [Property]
        public string NestedProperty
        {
            get
            {
                return this._nestedProperty;
            }
            set
            {
                this._nestedProperty = value;
            }
        }
    }
}
