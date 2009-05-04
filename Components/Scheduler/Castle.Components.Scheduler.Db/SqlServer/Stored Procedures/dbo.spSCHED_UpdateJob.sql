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

IF EXISTS (SELECT ID FROM sysobjects WHERE NAME = 'spSCHED_UpdateJob' AND type = 'p')
    DROP PROCEDURE spSCHED_UpdateJob
GO

/******************************************************************************
** Name    : spSCHED_UpdateJob
**
** Summary:
**
**   Updates a job in the database.
**
** Example:

DECLARE @CreationTime DATETIME
SET @CreationTime = GETUTCDATE()
DECLARE @ResultCode INT
EXEC spSCHED_UpdateJob 'TestCluster', 'job', 'renamedJob', 'Test job.', 'job.key', NULL, NULL, @ResultCode OUTPUT
SELECT @ResultCode

** Change History:
**
**   Date:    Author:  Bug #    Description:                           
**   -------- -------- ------   -----------------------------------------------
**   06/01/07 Jeff              Initial implementation.
*******************************************************************************
** Copyright (C) 2007 Castle Project, All Rights Reserved
*******************************************************************************/

CREATE PROCEDURE dbo.spSCHED_UpdateJob
(
	@ClusterName NVARCHAR(200),
	@ExistingJobName NVARCHAR(200),
	@UpdatedJobName NVARCHAR(200),
	@UpdatedJobDescription NVARCHAR(1000),
	@UpdatedJobKey NVARCHAR(200),
	@UpdatedTriggerObject VARBINARY(MAX),
	@UpdatedJobDataObject VARBINARY(MAX),
	@ResultCode INT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON
	SET LOCK_TIMEOUT 30000
	
	DECLARE @JobID INT
	
	DECLARE @JobState_Pending INT
	SET @JobState_Pending = 0
	DECLARE @JobState_Scheduled INT
	SET @JobState_Scheduled = 1
	
	DECLARE @ResultCode_JobUpdated INT
	SET @ResultCode_JobUpdated = 0	
	DECLARE @ResultCode_ExistingJobNotFound INT
	SET @ResultCode_ExistingJobNotFound = -1
	DECLARE @ResultCode_JobWithUpdatedNameExists INT
	SET @ResultCode_JobWithUpdatedNameExists = -2
	
	BEGIN TRY
		BEGIN TRAN
		
		IF @ExistingJobName <> @UpdatedJobName AND EXISTS(
			SELECT 1 FROM SCHED_Jobs J
				INNER JOIN SCHED_Clusters C ON C.ClusterID = J.ClusterID
				WHERE C.ClusterName = @ClusterName AND J.JobName = @UpdatedJobName)
		BEGIN
			IF XACT_STATE() <> 0 ROLLBACK
		
			SET @ResultCode = @ResultCode_JobWithUpdatedNameExists
			RETURN
		END
		
		UPDATE J
			SET JobName = @UpdatedJobName,
				JobDescription = @UpdatedJobDescription,
				JobKey = @UpdatedJobKey,
				TriggerObject = @UpdatedTriggerObject,
				JobDataObject = @UpdatedJobDataObject,
				JobState = CASE JobState WHEN @JobState_Scheduled THEN @JobState_Pending ELSE JobState END,
				Version = Version + 1
			FROM SCHED_Jobs J
			INNER JOIN SCHED_Clusters C ON C.ClusterID = J.ClusterID
			WHERE C.ClusterName = @ClusterName AND J.JobName = @ExistingJobName
				
		IF @@ROWCOUNT = 0
		BEGIN
			IF XACT_STATE() <> 0 ROLLBACK
		
			SET @ResultCode = @ResultCode_ExistingJobNotFound
			RETURN
		END
		
		SET @ResultCode = @ResultCode_JobUpdated
	
		COMMIT
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0 ROLLBACK
		EXEC spSCHED_RaiseError 'spSCHED_UpdateJob', 'Could not update job.'
	END CATCH
END
GO

GRANT EXECUTE ON dbo.spSCHED_UpdateJob TO SchedulerRole
GO
