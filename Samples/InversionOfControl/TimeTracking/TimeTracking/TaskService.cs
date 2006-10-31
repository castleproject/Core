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
	/// Manages "business" checks for
	/// the task entity.
	/// <para>
	/// This is also separation of concerns applied.
	/// The dataaccess layer should not validate
	/// the entities it handles, instead it may expect
	/// that what it gets was already validated.
	/// </para>
	/// </summary>
	public class TaskService
	{
		private readonly ITaskDataAccess taskDataAccess;

		/// <summary>
		/// This is how a component defines that its
		/// dependencies
		/// </summary>
		/// <param name="taskDataAccess">A service that we need 
		/// in order to do our work</param>
		public TaskService(ITaskDataAccess taskDataAccess)
		{
			this.taskDataAccess = taskDataAccess;
		}

		/// <summary>
		/// Performs initial "business rules" checks
		/// before inserting the task
		/// </summary>
		/// <param name="task">Tast to add</param>
		public void Add(Task task)
		{
			// Trivial check
			
			if (task == null)
			{
				throw new ArgumentNullException("task");
			}
			
			// Must have a name
			
			if (task.Name == null || task.Name.Trim() == String.Empty)
			{
				throw new ArgumentException("A Task must have a name");
			}
			
			// Name must be unique
			
			if (taskDataAccess.FindByName(task.Name) != null)
			{
				throw new ArgumentException("The name " + task.Name + " is assigned to an existing task");
			}
			
			// Everything OK, insert it
			
			taskDataAccess.Insert(task);
		}
		
		/// <summary>
		/// Here we have just delegation.
		/// </summary>
		/// <returns>An array of tasks</returns>
		public Task[] FindAll()
		{
			return taskDataAccess.FindAll();
		}
	}
}
