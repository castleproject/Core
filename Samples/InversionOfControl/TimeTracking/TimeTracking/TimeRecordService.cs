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

namespace TimeTracking
{
	using System;
	using TimeTracking.DataAccess;
	using TimeTracking.Models;

	/// <summary>
	/// As you can see, this service exposes business friendly methods
	/// to the UI operations. 
	/// </summary>
	public class TimeRecordService
	{
		private readonly ITimeRecordDataAccess timeDataAccess;
		
		private EventType currentClockState = EventType.Undefined;
		private TimeRecord currentRecord;

		public TimeRecordService(ITimeRecordDataAccess timeDataAccess)
		{
			this.timeDataAccess = timeDataAccess;
		}

		public void StartClock(Task selectedTask)
		{
			if (selectedTask == null)
			{
				throw new ArgumentNullException("Task cannot be null");
			}
			
			// Creates the record
			
			currentRecord = new TimeRecord();
			
			currentRecord.StartedDate = DateTime.Now;
			currentRecord.Task = selectedTask;
			
			timeDataAccess.Insert(currentRecord);
			
			// Creates the event
			
			TimeRecordEvent timeEvent = new TimeRecordEvent(currentRecord, EventType.Started, DateTime.Now);
			
			timeDataAccess.InsertEvent(timeEvent);
			
			currentClockState = EventType.Started;
		}
		
		public void PauseClock()
		{
			TimeRecordEvent timeEvent = new TimeRecordEvent(currentRecord, EventType.Paused, DateTime.Now);
			
			timeDataAccess.InsertEvent(timeEvent);

			currentClockState = EventType.Paused;
		}

		public void ResumeClock()
		{
			TimeRecordEvent timeEvent = new TimeRecordEvent(currentRecord, EventType.Resumed, DateTime.Now);
			
			timeDataAccess.InsertEvent(timeEvent);

			currentClockState = EventType.Started;
		}

		public void StopClock()
		{
			TimeRecordEvent timeEvent = new TimeRecordEvent(currentRecord, EventType.Stopped, DateTime.Now);
			
			timeDataAccess.InsertEvent(timeEvent);

			currentClockState = EventType.Stopped;
		}
		
		public bool CanStartClock
		{
			get { return currentClockState == EventType.Undefined || 
			             currentClockState == EventType.Stopped; }
		}

		public bool CanPauseClock
		{
			get { return currentClockState != EventType.Undefined && 
			             currentClockState != EventType.Paused; }
		}

		public bool CanResumeClock
		{
			get { return currentClockState == EventType.Paused; }
		}

		public bool CanStopClock
		{
			get { return currentClockState != EventType.Undefined && 
			             currentClockState != EventType.Stopped; }
		}

		public TimeRecord CurrentRecord
		{
			get { return currentRecord; }
		}
		
		public EventType CurrentClockState
		{
			get { return currentClockState; }
		}
	}
}
