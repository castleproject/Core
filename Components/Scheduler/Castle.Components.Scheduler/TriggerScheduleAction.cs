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
	/// Specifies the action that the scheduler should perform when the trigger fires.
	/// </summary>
	public enum TriggerScheduleAction
	{
		/// <summary>
		/// The scheduler should do nothing at this time.
		/// The trigger will remain active and may provide a different action when it next fires.
		/// </summary>
		Skip,

		/// <summary>
		/// The scheduler should do nothing at this time.
		/// The trigger has become inactive and will not fire again.
		/// </summary>
		Stop,

		/// <summary>
		/// The scheduler should execute the associated job.
		/// </summary>
		ExecuteJob,

		/// <summary>
		/// The scheduler should delete the job.
		/// </summary>
		DeleteJob
	}
}