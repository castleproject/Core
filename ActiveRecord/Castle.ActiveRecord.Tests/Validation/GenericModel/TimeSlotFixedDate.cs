// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

using Castle.Components.Validator;
namespace Castle.ActiveRecord.Tests.Validation.Model.GenericModel
{
	public abstract class AbstractTimeSlot<t> : ActiveRecordValidationBase
    {
        private int myId;
        private string myName;
        protected int myHour;

        [PrimaryKey(Column = "timeslot_id")]
        public int Id
        {
            get { return myId; }
            set { myId = value; }
        }

        [Property, ValidateNonEmpty, ValidateIsUnique]
        public string Name
        {
            get { return myName; }
            set { myName= value; }
        }

        [Property]
        [ValidateNonEmpty]
        public int Hour
        {
            get { return myHour; }
            set { myHour = value; }
        }
    }

    [ActiveRecord(DiscriminatorValue = "TimeSlotFixedDate")]
    public class TimeSlotFixedDate : AbstractTimeSlot<TimeSlotFixedDate>
    {
        private int myDay;

        [Property]
        [ValidateNonEmpty]
        public int Day
        {
            get { return myDay; }
            set { myDay = value; }
        }
    }

}
