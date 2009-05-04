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

namespace Castle.Components.Scheduler
{
	/// <summary>
	/// The trigger condition specifies the reason that the trigger's scheduling method
	/// is being invoked.
	/// </summary>
	public enum TriggerScheduleCondition
	{
		/// <summary>
		/// The scheduler is asking the trigger when it will fire next because
		/// the job has just been created, been updated or completed execution.
		/// </summary>
		Latch,

		/// <summary>
		/// The time last specified as the trigger's schedule time has arrived within
		/// the tolerance established by the trigger's misfire threshold.
		/// </summary>
		Fire,

		/// <summary>
		/// The time last specified as the trigger's schedule time was missed by a duration
		/// in excess of the tolerance established by the trigger's misfire threshold.
		/// </summary>
		Misfire
	}
}