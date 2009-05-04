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

namespace Castle.Components.Scheduler.JobStores
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using Utilities;

	/// <summary>
	/// Abstract base class for an ADO.Net based job store Data Access Object using
	/// stored procedures.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The database schema and stored procedures must be deployed to the database
	/// manually or by some other means before the job store is used.
	/// For enhanced security, the database user specified in the connection
	/// string should only be a member of the SchedulerRole (thus having EXECUTE
	/// permission to the stored procedures but no direct access to the tables.)
	/// </para>
	/// </remarks>
	public abstract class AdoNetJobStoreDao : IJobStoreDao
	{
		private readonly string connectionString;
		private readonly string parameterPrefix;

		private enum CreateJobConflictActionCode
		{
			Ignore = 0,
			Replace = 1,
			Update = 2
		}

		private enum CreateJobResultCode
		{
			JobCreated = 0,
			JobReplaced = 1,
			JobUpdated = 2,
			JobWithSameNameExists = -1
		}

		private enum UpdateJobResultCode
		{
			JobUpdated = 0,
			ExistingJobNotFound = -1,
			JobWithUpdatedNameExists = -2,
		}

		/// <summary>
		/// Creates an ADO.Net based job store DAO.
		/// </summary>
		/// <param name="connectionString">The connection string</param>
		/// <param name="parameterPrefix">The stored procedure parameter prefix, if any</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="connectionString"/> or <paramref name="parameterPrefix"/> is null</exception>
		protected AdoNetJobStoreDao(string connectionString, string parameterPrefix)
		{
			if (connectionString == null)
				throw new ArgumentNullException("connectionString");
			if (parameterPrefix == null)
				throw new ArgumentNullException("parameterPrefix");

			this.connectionString = connectionString;
			this.parameterPrefix = parameterPrefix;
		}

		/// <summary>
		/// Gets the connection string.
		/// </summary>
		public string ConnectionString
		{
			get { return connectionString; }
		}

		/// <summary>
		/// Registers a scheduler.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="schedulerGuid">The scheduler GUID</param>
		/// <param name="schedulerName">The scheduler name, never null</param>
		/// <param name="lastSeenUtc">The time the scheduler was last seen</param>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		public virtual void RegisterScheduler(string clusterName, Guid schedulerGuid, string schedulerName,
		                                      DateTime lastSeenUtc)
		{
			try
			{
				using (IDbConnection connection = CreateConnection())
				{
					IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_RegisterScheduler");

					AddInputParameter(command, "ClusterName", DbType.String, clusterName);
					AddInputParameter(command, "SchedulerGUID", DbType.Guid, schedulerGuid);
					AddInputParameter(command, "SchedulerName", DbType.String, schedulerName);
					AddInputParameter(command, "LastSeen", DbType.DateTime, lastSeenUtc);

					connection.Open();
					command.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				throw new SchedulerException("The job store was unable to register a scheduler instance in the database.", ex);
			}
		}

		/// <summary>
		/// Unregisters a scheduler and orphans all of its jobs.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="schedulerGuid">The scheduler GUID</param>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		public virtual void UnregisterScheduler(string clusterName, Guid schedulerGuid)
		{
			try
			{
				using (IDbConnection connection = CreateConnection())
				{
					IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_UnregisterScheduler");

					AddInputParameter(command, "ClusterName", DbType.String, clusterName);
					AddInputParameter(command, "SchedulerGUID", DbType.Guid, schedulerGuid);

					connection.Open();
					command.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				throw new SchedulerException("The job store was unable to unregister a scheduler instance in the database.", ex);
			}
		}

		/// <summary>
		/// Creates a job in the database.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="jobSpec">The job specification, never null</param>
		/// <param name="creationTimeUtc">The job creation time</param>
		/// <param name="conflictAction">The action to take if a conflict occurs</param>
		/// <returns>True if the job was created or updated, false if a conflict occurred
		/// and no changes were made</returns>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		public virtual bool CreateJob(string clusterName, JobSpec jobSpec, DateTime creationTimeUtc,
		                              CreateJobConflictAction conflictAction)
		{
			try
			{
				using (IDbConnection connection = CreateConnection())
				{
					IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_CreateJob");

					CreateJobConflictActionCode conflictActionCode;
					switch (conflictAction)
					{
						default:
						case CreateJobConflictAction.Ignore:
						case CreateJobConflictAction.Throw:
							conflictActionCode = CreateJobConflictActionCode.Ignore;
							break;

						case CreateJobConflictAction.Replace:
							conflictActionCode = CreateJobConflictActionCode.Replace;
							break;

						case CreateJobConflictAction.Update:
							conflictActionCode = CreateJobConflictActionCode.Update;
							break;
					}

					AddInputParameter(command, "ClusterName", DbType.String, clusterName);
					AddInputParameter(command, "JobName", DbType.String, jobSpec.Name);
					AddInputParameter(command, "JobDescription", DbType.String, jobSpec.Description);
					AddInputParameter(command, "JobKey", DbType.String, jobSpec.JobKey);
					AddInputParameter(command, "TriggerObject", DbType.Binary,
					                  DbUtils.MapObjectToDbValue(DbUtils.SerializeObject(jobSpec.Trigger)));
					AddInputParameter(command, "JobDataObject", DbType.Binary,
					                  DbUtils.MapObjectToDbValue(DbUtils.SerializeObject(jobSpec.JobData)));
					AddInputParameter(command, "CreationTime", DbType.DateTime, creationTimeUtc);
					AddInputParameter(command, "ConflictActionCode", DbType.Int32, conflictActionCode);
					IDbDataParameter resultCodeParam = AddOutputParameter(command, "ResultCode", DbType.Int32);

					connection.Open();
					command.ExecuteNonQuery();

					CreateJobResultCode resultCode = (CreateJobResultCode) resultCodeParam.Value;
					switch (resultCode)
					{
						case CreateJobResultCode.JobCreated:
						case CreateJobResultCode.JobReplaced:
						case CreateJobResultCode.JobUpdated:
							return true;

						case CreateJobResultCode.JobWithSameNameExists:
							if (conflictAction == CreateJobConflictAction.Throw)
								throw new SchedulerException(String.Format(CultureInfo.CurrentCulture,
								                                           "Job '{0}' already exists.", jobSpec.Name));
							return false;

						default:
							throw new SchedulerException(String.Format(CultureInfo.CurrentCulture,
							                                           "spSCHED_CreateJob returned unrecognized result code '{0}'.",
							                                           resultCode));
					}
				}
			}
			catch (Exception ex)
			{
				throw new SchedulerException("The job store was unable to create a job in the database.", ex);
			}
		}

		/// <summary>
		/// Updates an existing job.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="existingJobName">The name of the existing job to update</param>
		/// <param name="updatedJobSpec">The updated job specification</param>
		/// <exception cref="SchedulerException">Thrown if an error occurs or if the job does not exist</exception>
		public virtual void UpdateJob(string clusterName, string existingJobName, JobSpec updatedJobSpec)
		{
			try
			{
				using (IDbConnection connection = CreateConnection())
				{
					IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_UpdateJob");

					AddInputParameter(command, "ClusterName", DbType.String, clusterName);
					AddInputParameter(command, "ExistingJobName", DbType.String, existingJobName);
					AddInputParameter(command, "UpdatedJobName", DbType.String, updatedJobSpec.Name);
					AddInputParameter(command, "UpdatedJobDescription", DbType.String, updatedJobSpec.Description);
					AddInputParameter(command, "UpdatedJobKey", DbType.String, updatedJobSpec.JobKey);
					AddInputParameter(command, "UpdatedTriggerObject", DbType.Binary,
					                  DbUtils.MapObjectToDbValue(DbUtils.SerializeObject(updatedJobSpec.Trigger)));
					AddInputParameter(command, "UpdatedJobDataObject", DbType.Binary,
					                  DbUtils.MapObjectToDbValue(DbUtils.SerializeObject(updatedJobSpec.JobData)));
					IDbDataParameter resultCodeParam = AddOutputParameter(command, "ResultCode", DbType.Int32);

					connection.Open();
					command.ExecuteNonQuery();

					UpdateJobResultCode resultCode = (UpdateJobResultCode) resultCodeParam.Value;
					switch (resultCode)
					{
						case UpdateJobResultCode.JobUpdated:
							return;

						case UpdateJobResultCode.ExistingJobNotFound:
							throw new SchedulerException(String.Format(CultureInfo.CurrentCulture,
							                                           "Job '{0}' does not exist so it cannot be updated.", existingJobName));

						case UpdateJobResultCode.JobWithUpdatedNameExists:
							throw new SchedulerException(String.Format(CultureInfo.CurrentCulture,
							                                           "Cannot rename job '{0}' to '{1}' because there already exists another job with the new name.",
							                                           existingJobName, updatedJobSpec.Name));

						default:
							throw new SchedulerException(String.Format(CultureInfo.CurrentCulture,
							                                           "spSCHED_UpdateJob returned unrecognized result code '{0}'.",
							                                           resultCode));
					}
				}
			}
			catch (Exception ex)
			{
				throw new SchedulerException("The job store was unable to update a job in the database.", ex);
			}
		}

		/// <summary>
		/// Deletes the job with the specified name.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="jobName">The job name, never null</param>
		/// <returns>True if a job was actually deleted</returns>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		public virtual bool DeleteJob(string clusterName, string jobName)
		{
			try
			{
				using (IDbConnection connection = CreateConnection())
				{
					IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_DeleteJob");

					AddInputParameter(command, "ClusterName", DbType.String, clusterName);
					AddInputParameter(command, "JobName", DbType.String, jobName);
					IDbDataParameter wasDeletedParam = AddOutputParameter(command, "WasDeleted", DbType.Boolean);

					connection.Open();
					command.ExecuteNonQuery();

					return (bool) wasDeletedParam.Value;
				}
			}
			catch (Exception ex)
			{
				throw new SchedulerException("The job store was unable to delete a job in the database.", ex);
			}
		}

		/// <summary>
		/// Gets details for the named job.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="jobName">The job name, never null</param>
		/// <returns>The job details, or null if none was found</returns>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		public virtual VersionedJobDetails GetJobDetails(string clusterName, string jobName)
		{
			try
			{
				using (IDbConnection connection = CreateConnection())
				{
					IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_GetJobDetails");

					AddInputParameter(command, "ClusterName", DbType.String, clusterName);
					AddInputParameter(command, "JobName", DbType.String, jobName);

					connection.Open();

					VersionedJobDetails jobDetails;
					using (IDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
					{
						jobDetails = reader.Read() ? BuildJobDetailsFromResultSet(reader) : null;
					}

					return jobDetails;
				}
			}
			catch (Exception ex)
			{
				throw new SchedulerException("The job store was unable to get job details from the database.", ex);
			}
		}

		/// <summary>
		/// Saves details for the job.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="jobDetails">The job details, never null</param>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		public virtual void SaveJobDetails(string clusterName, VersionedJobDetails jobDetails)
		{
			try
			{
				using (IDbConnection connection = CreateConnection())
				{
					IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_SaveJobDetails");

					AddInputParameter(command, "ClusterName", DbType.String, clusterName);

					AddInputParameter(command, "JobName", DbType.String, jobDetails.JobSpec.Name);
					AddInputParameter(command, "JobDescription", DbType.String, jobDetails.JobSpec.Description);
					AddInputParameter(command, "JobKey", DbType.String, jobDetails.JobSpec.JobKey);
					AddInputParameter(command, "TriggerObject", DbType.Binary,
					                  DbUtils.MapObjectToDbValue(DbUtils.SerializeObject(jobDetails.JobSpec.Trigger)));

					AddInputParameter(command, "JobDataObject", DbType.Binary,
					                  DbUtils.MapObjectToDbValue(DbUtils.SerializeObject(jobDetails.JobSpec.JobData)));
					AddInputParameter(command, "JobState", DbType.Int32, jobDetails.JobState);
					AddInputParameter(command, "NextTriggerFireTime", DbType.DateTime,
					                  DbUtils.MapNullableToDbValue(jobDetails.NextTriggerFireTimeUtc));
					int? nextTriggerMisfireThresholdSeconds = jobDetails.NextTriggerMisfireThreshold.HasValue
					                                          	?
					                                          		(int?) jobDetails.NextTriggerMisfireThreshold.Value.TotalSeconds
					                                          	: null;
					AddInputParameter(command, "NextTriggerMisfireThresholdSeconds", DbType.Int32,
					                  DbUtils.MapNullableToDbValue(nextTriggerMisfireThresholdSeconds));

					JobExecutionDetails execution = jobDetails.LastJobExecutionDetails;
					AddInputParameter(command, "LastExecutionSchedulerGUID", DbType.Guid,
					                  execution != null ? (object) execution.SchedulerGuid : DBNull.Value);
					AddInputParameter(command, "LastExecutionStartTime", DbType.DateTime,
					                  execution != null ? (object) execution.StartTimeUtc : DBNull.Value);
					AddInputParameter(command, "LastExecutionEndTime", DbType.DateTime,
					                  execution != null ? DbUtils.MapNullableToDbValue(execution.EndTimeUtc) : DBNull.Value);
					AddInputParameter(command, "LastExecutionSucceeded", DbType.Boolean,
					                  execution != null ? (object) execution.Succeeded : DBNull.Value);
					AddInputParameter(command, "LastExecutionStatusMessage", DbType.String,
					                  execution != null ? (object) execution.StatusMessage : DBNull.Value);

					AddInputParameter(command, "Version", DbType.Int32, jobDetails.Version);

					IDbDataParameter wasSavedParam = AddOutputParameter(command, "WasSaved", DbType.Boolean);

					connection.Open();
					command.ExecuteNonQuery();

					bool wasSaved = (bool) wasSavedParam.Value;
					if (!wasSaved)
						throw new ConcurrentModificationException(
							String.Format("Job '{0}' does not exist or was concurrently modified in the database.",
							              jobDetails.JobSpec.Name));

					jobDetails.Version += 1;
				}
			}
			catch (Exception ex)
			{
				throw new SchedulerException("The job store was unable to save job details to the database.", ex);
			}
		}

		/// <summary>
		/// Gets the next job to process for the specified scheduler.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="schedulerGuid">The scheduler GUID</param>
		/// <param name="timeBasisUtc">The UTC time to consider as "now"</param>
		/// <param name="nextTriggerFireTimeUtc">Set to the UTC next trigger fire time, or null if there are
		/// no triggers currently scheduled to fire</param>
		/// <param name="schedulerExpirationTimeInSeconds">The scheduler expiration time in seconds, always greater than zero</param>
		/// <returns>The details of job to process or null if none</returns>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		public virtual VersionedJobDetails GetNextJobToProcess(string clusterName, Guid schedulerGuid, DateTime timeBasisUtc,
		                                                       int schedulerExpirationTimeInSeconds,
		                                                       out DateTime? nextTriggerFireTimeUtc)
		{
			try
			{
				using (IDbConnection connection = CreateConnection())
				{
					IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_GetNextJobToProcess");

					AddInputParameter(command, "ClusterName", DbType.String, clusterName);
					AddInputParameter(command, "SchedulerGUID", DbType.Guid, schedulerGuid);
					AddInputParameter(command, "TimeBasis", DbType.DateTime, timeBasisUtc);
					AddInputParameter(command, "SchedulerExpirationTimeInSeconds", DbType.Int32, schedulerExpirationTimeInSeconds);
					IDbDataParameter nextTriggerFireTimeParam = AddOutputParameter(command, "NextTriggerFireTime", DbType.DateTime);

					connection.Open();

					VersionedJobDetails jobDetails;
					using (IDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
					{
						jobDetails = reader.Read() ? BuildJobDetailsFromResultSet(reader) : null;
					}

					nextTriggerFireTimeUtc =
						DateTimeUtils.AssumeUniversalTime(DbUtils.MapDbValueToNullable<DateTime>(nextTriggerFireTimeParam.Value));
					return jobDetails;
				}
			}
			catch (Exception ex)
			{
				throw new SchedulerException(
					"The job store was unable to get job details for the next job to process from the database.", ex);
			}
		}

		/// <summary>
		/// Gets the names of all jobs.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <returns>The names of all jobs</returns>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		public string[] ListJobNames(string clusterName)
		{
			try
			{
				using (IDbConnection connection = CreateConnection())
				{
					IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_ListJobNames");

					AddInputParameter(command, "ClusterName", DbType.String, clusterName);

					connection.Open();

					List<string> jobNames = new List<string>();
					using (IDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
					{
						while (reader.Read())
							jobNames.Add(reader.GetString(0));
					}

					return jobNames.ToArray();
				}
			}
			catch (Exception ex)
			{
				throw new SchedulerException("The job store was unable to get the list of job names from the database.", ex);
			}
		}

		/// <summary>
		/// Builds a job details object from the result set returned by the spSCHED_GetJobDetails
		/// and spSCHED_GetNextJob stored procedures.
		/// </summary>
		/// <param name="reader">The reader for the result set</param>
		/// <returns>The job details object</returns>
		protected virtual VersionedJobDetails BuildJobDetailsFromResultSet(IDataReader reader)
		{
			string jobName = reader.GetString(0);
			string jobDescription = reader.GetString(1);
			string jobKey = reader.GetString(2);
			Trigger trigger = (Trigger) DbUtils.DeserializeObject(DbUtils.MapDbValueToObject<byte[]>(reader.GetValue(3)));

			JobData jobData = (JobData) DbUtils.DeserializeObject(DbUtils.MapDbValueToObject<byte[]>(reader.GetValue(4)));
			DateTime creationTimeUtc = DateTimeUtils.AssumeUniversalTime(reader.GetDateTime(5));
			JobState jobState = (JobState) reader.GetInt32(6);
			DateTime? nextTriggerFireTimeUtc =
				DateTimeUtils.AssumeUniversalTime(DbUtils.MapDbValueToNullable<DateTime>(reader.GetValue(7)));
			int? nextTriggerMisfireThresholdSeconds = DbUtils.MapDbValueToNullable<int>(reader.GetValue(8));
			TimeSpan? nextTriggerMisfireThreshold = nextTriggerMisfireThresholdSeconds.HasValue
			                                        	?
			                                        		new TimeSpan(0, 0, nextTriggerMisfireThresholdSeconds.Value)
			                                        	: (TimeSpan?) null;

			Guid? lastExecutionSchedulerGuid = DbUtils.MapDbValueToNullable<Guid>(reader.GetValue(9));
			DateTime? lastExecutionStartTimeUtc =
				DateTimeUtils.AssumeUniversalTime(DbUtils.MapDbValueToNullable<DateTime>(reader.GetValue(10)));
			DateTime? lastExecutionEndTimeUtc =
				DateTimeUtils.AssumeUniversalTime(DbUtils.MapDbValueToNullable<DateTime>(reader.GetValue(11)));
			bool? lastExecutionSucceeded = DbUtils.MapDbValueToNullable<bool>(reader.GetValue(12));
			string lastExecutionStatusMessage = DbUtils.MapDbValueToObject<string>(reader.GetValue(13));

			int version = reader.GetInt32(14);

			JobSpec jobSpec = new JobSpec(jobName, jobDescription, jobKey, trigger);
			jobSpec.JobData = jobData;

			VersionedJobDetails details = new VersionedJobDetails(jobSpec, creationTimeUtc, version);
			details.JobState = jobState;
			details.NextTriggerFireTimeUtc = nextTriggerFireTimeUtc;
			details.NextTriggerMisfireThreshold = nextTriggerMisfireThreshold;

			if (lastExecutionSchedulerGuid.HasValue && lastExecutionStartTimeUtc.HasValue)
			{
				JobExecutionDetails execution = new JobExecutionDetails(lastExecutionSchedulerGuid.Value,
				                                                        lastExecutionStartTimeUtc.Value);
				execution.EndTimeUtc = lastExecutionEndTimeUtc;
				execution.Succeeded = lastExecutionSucceeded.GetValueOrDefault();
				execution.StatusMessage = lastExecutionStatusMessage == null ? "" : lastExecutionStatusMessage;

				details.LastJobExecutionDetails = execution;
			}

			return details;
		}

		/// <summary>
		/// Creates a database connection.
		/// </summary>
		/// <returns>The database connection</returns>
		protected abstract IDbConnection CreateConnection();

		/// <summary>
		/// Creates a command to invoke the specified stored procedure.
		/// </summary>
		/// <param name="connection">The Db connection</param>
		/// <param name="spName">The stored procedure name</param>
		/// <returns>The Db command</returns>
		protected virtual IDbCommand CreateStoredProcedureCommand(IDbConnection connection, string spName)
		{
			IDbCommand command = connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = spName;
			return command;
		}

		/// <summary>
		/// Creates a generic parameter and adds it to a command.
		/// </summary>
		/// <param name="command">The command</param>
		/// <param name="name">The parameter name</param>
		/// <param name="type">The parameter value type</param>
		/// <returns>The parameter</returns>
		protected virtual IDbDataParameter AddParameter(IDbCommand command, string name, DbType type)
		{
			IDbDataParameter parameter = command.CreateParameter();
			parameter.ParameterName = parameterPrefix + name;
			parameter.DbType = type;

			command.Parameters.Add(parameter);
			return parameter;
		}

		/// <summary>
		/// Creates an input parameter and adds it to a command.
		/// </summary>
		/// <param name="command">The command</param>
		/// <param name="name">The parameter name</param>
		/// <param name="type">The parameter value type</param>
		/// <param name="value">The value of the parameter</param>
		/// <returns>The parameter</returns>
		protected IDbDataParameter AddInputParameter(IDbCommand command, string name, DbType type, object value)
		{
			IDbDataParameter parameter = AddParameter(command, name, type);
			parameter.Direction = ParameterDirection.Input;
			parameter.Value = value;
			return parameter;
		}

		/// <summary>
		/// Creates an output parameter and adds it to a command.
		/// </summary>
		/// <param name="command">The command</param>
		/// <param name="name">The parameter name</param>
		/// <param name="type">The parameter value type</param>
		/// <returns>The parameter</returns>
		protected IDbDataParameter AddOutputParameter(IDbCommand command, string name, DbType type)
		{
			IDbDataParameter parameter = AddParameter(command, name, type);
			parameter.Direction = ParameterDirection.Output;
			return parameter;
		}
	}
}