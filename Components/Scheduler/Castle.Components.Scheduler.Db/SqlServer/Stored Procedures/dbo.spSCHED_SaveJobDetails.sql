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

IF EXISTS (SELECT ID FROM sysobjects WHERE NAME = 'spSCHED_SaveJobDetails' AND type = 'p')
    DROP PROCEDURE spSCHED_SaveJobDetails
GO

/******************************************************************************
** Name    : spSCHED_SaveJobDetails
**
** Summary:
**
**   Saves job details for a named job in a cluster.
**
** Example:

DECLARE @WasSaved BIT
EXEC spSCHED_SaveJobDetails 'TestCluster', 'job', ..., @WasSaved OUTPUT
SELECT @WasSaved

** Change History:
**
**   Date:    Author:  Bug #    Description:                           
**   -------- -------- ------   -----------------------------------------------
**   05/20/07 Jeff              Initial implementation.
*******************************************************************************
** Copyright (C) 2007 Castle Project, All Rights Reserved
*******************************************************************************/

CREATE PROCEDURE dbo.spSCHED_SaveJobDetails
(
	@ClusterName NVARCHAR(200),
	@JobName NVARCHAR(200),
	@JobDescription NVARCHAR(1000),
	@JobKey NVARCHAR(200),
	@TriggerObject VARBINARY(MAX),
	@JobDataObject VARBINARY(MAX),
	@JobState INT,
	@NextTriggerFireTime DATETIME,
	@NextTriggerMisfireThresholdSeconds INT,
	@LastExecutionSchedulerGUID UNIQUEIDENTIFIER,
	@LastExecutionStartTime DATETIME,
	@LastExecutionEndTime DATETIME,
	@LastExecutionSucceeded BIT,
	@LastExecutionStatusMessage NVARCHAR(MAX),
	@Version INT,
	@WasSaved BIT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON
	SET LOCK_TIMEOUT 30000

	SET @WasSaved = 0
	
	BEGIN TRY
		SET ROWCOUNT 1
		UPDATE J
			SET JobDescription = @JobDescription,
				JobKey = @JobKey,
				TriggerObject = @TriggerObject,
				JobDataObject = @JobDataObject,
				JobState = @JobState,
				NextTriggerFireTime = @NextTriggerFireTime,
				NextTriggerMisfireThresholdSeconds = @NextTriggerMisfireThresholdSeconds,
				LastExecutionSchedulerGUID = @LastExecutionSchedulerGUID,
				LastExecutionStartTime = @LastExecutionStartTime,
				LastExecutionEndTime = @LastExecutionEndTime,
				LastExecutionSucceeded = @LastExecutionSucceeded,
				LastExecutionStatusMessage = @LastExecutionStatusMessage,
				Version = @Version + 1
			FROM SCHED_Jobs J
			INNER JOIN SCHED_Clusters C ON C.ClusterID = J.ClusterID
			WHERE C.ClusterName = @ClusterName AND J.JobName = @JobName AND J.Version = @Version
			
		IF @@ROWCOUNT <> 0
			SET @WasSaved = 1
	END TRY
	BEGIN CATCH
		EXEC spSCHED_RaiseError 'spSCHED_SaveJobDetails', 'Could not save job details.'
	END CATCH
END
GO

GRANT EXECUTE ON dbo.spSCHED_SaveJobDetails TO SchedulerRole
GO
