-- Copyright 2004-2009 Castle Project - http://www.castleproject.org/
-- 
-- Licensed under the Apache License, Version 2.0 (the "License");
-- you may not use this file except in compliance with the License.
-- You may obtain a copy of the License at
-- 
--     http://www.apache.org/licenses/LICENSE-2.0
-- 
-- Unless required by applicable law or agreed to in writing, software
-- distributed under the License is distributed on an "AS IS" BASIS,
-- WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
-- See the License for the specific language governing permissions and
-- limitations under the License.

IF EXISTS (SELECT ID FROM sysobjects WHERE NAME = 'spSCHED_GetNextJobToProcess' AND type = 'p')
    DROP PROCEDURE spSCHED_GetNextJobToProcess
GO

/******************************************************************************
** Name    : spSCHED_GetNextJobToProcess
**
** Summary:
**
**   Gets job details for a named job in a cluster.
**
** Example:

DECLARE @TimeBasis DATETIME
SET @TimeBasis = GETUTCDATE()
DECLARE @NextTriggerFireTime DATETIME
EXEC spSCHED_GetNextJobToProcess 'TestCluster', '{88F43A7B-7FE2-4593-B390-40858B5A0CBF}', @TimeBasis, 60, @NextTriggerFireTime OUTPUT
SELECT @NextTriggerFireTime

** Change History:
**
**   Date:    Author:  Bug #    Description:                           
**   -------- -------- ------   -----------------------------------------------
**   05/20/07 Jeff              Initial implementation.
*******************************************************************************
** Copyright (C) 2007 Castle Project, All Rights Reserved
*******************************************************************************/

CREATE PROCEDURE dbo.spSCHED_GetNextJobToProcess
(
	@ClusterName NVARCHAR(200),
	@SchedulerGUID UNIQUEIDENTIFIER,
	@TimeBasis DATETIME,
	@SchedulerExpirationTimeInSeconds INT, 
	@NextTriggerFireTime DATETIME OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON
	SET LOCK_TIMEOUT 30000
	
	DECLARE @JobState_Pending INT
	SET @JobState_Pending = 0
	DECLARE @JobState_Scheduled INT
	SET @JobState_Scheduled = 1
	DECLARE @JobState_Triggered INT
	SET @JobState_Triggered = 2
	DECLARE @JobState_Running INT
	SET @JobState_Running = 3
	DECLARE @JobState_Completed INT
	SET @JobState_Completed = 4
	DECLARE @JobState_Orphaned INT
	SET @JobState_Orphaned = 5
	
	SET @NextTriggerFireTime = NULL
	
	-- Trigger any scheduled jobs whose time has passed.
	BEGIN TRY
		BEGIN TRAN
		
		UPDATE J
			SET JobState = @JobState_Triggered
			FROM SCHED_Jobs J
			INNER JOIN SCHED_Clusters C ON C.ClusterID = J.ClusterID
			WHERE C.ClusterName = @ClusterName
				AND J.JobState = @JobState_Scheduled
				AND (J.NextTriggerFireTime IS NULL OR J.NextTriggerFireTime <= @TimeBasis)
				
		COMMIT
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0 ROLLBACK
		EXEC spSCHED_RaiseError 'spSCHED_GetNextJobToProcess', 'Could not update triggered jobs.'
		RETURN
	END CATCH
	
	-- Purge schedulers that have expired.
	BEGIN TRY
		BEGIN TRAN
	
		DECLARE @LapsedExpirationTime DATETIME
		SET @LapsedExpirationTime = DATEADD(second, - @SchedulerExpirationTimeInSeconds, @TimeBasis)

		DELETE S
			FROM SCHED_Schedulers S
			INNER JOIN SCHED_Clusters C ON C.ClusterID = S.ClusterID
			WHERE C.ClusterName = @ClusterName
				AND S.LastSeen < @LapsedExpirationTime
				
		COMMIT
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0 ROLLBACK
		EXEC spSCHED_RaiseError 'spSCHED_GetNextJobToProcess', 'Could not delete expired schedulers.'
		RETURN
	END CATCH	

	-- Orphan any running jobs whose schedulers have expired.
	BEGIN TRY
		BEGIN TRAN
	
		UPDATE J
			SET JobState = @JobState_Orphaned,
				LastExecutionEndTime = @TimeBasis
			FROM SCHED_Jobs J
			INNER JOIN SCHED_Clusters C ON C.ClusterID = J.ClusterID
			LEFT OUTER JOIN SCHED_Schedulers S ON S.SchedulerGUID = J.LastExecutionSchedulerGUID
			WHERE C.ClusterName = @ClusterName
				AND J.JobState = @JobState_Running
				AND S.SchedulerID IS NULL
				
		COMMIT
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0 ROLLBACK
		EXEC spSCHED_RaiseError 'spSCHED_GetNextJobToProcess', 'Could not orphan jobs whose schedulers have expired.'
		RETURN
	END CATCH

	-- Get the next job to process.	
	BEGIN TRY
		SELECT TOP 1
			J.JobName, J.JobDescription, J.JobKey, J.TriggerObject, J.JobDataObject, J.CreationTime,
			J.JobState, J.NextTriggerFireTime, J.NextTriggerMisfireThresholdSeconds,
			J.LastExecutionSchedulerGUID, J.LastExecutionStartTime, J.LastExecutionEndTime, J.LastExecutionSucceeded, J.LastExecutionStatusMessage,
			J.Version
			FROM SCHED_Jobs J
			INNER JOIN SCHED_Clusters C ON C.ClusterID = J.ClusterID
			WHERE C.ClusterName = @ClusterName
				AND J.JobState IN (@JobState_Pending, @JobState_Triggered, @JobState_Completed, @JobState_Orphaned)
				
		IF @@ROWCOUNT <> 0
			RETURN

		SELECT TOP 1 @NextTriggerFireTime = ISNULL(J.NextTriggerFireTime, @TimeBasis)
			FROM SCHED_Jobs J
			INNER JOIN SCHED_Clusters C ON C.ClusterID = J.ClusterID
			WHERE C.ClusterName = @ClusterName
				AND J.JobState = @JobState_Scheduled
			ORDER BY J.NextTriggerFireTime ASC	
	END TRY
	BEGIN CATCH
		EXEC spSCHED_RaiseError 'spSCHED_GetNextJobToProcess', 'Could not get job details for next job to process.'
	END CATCH
END
GO

GRANT EXECUTE ON dbo.spSCHED_GetNextJobToProcess TO SchedulerRole
GO
