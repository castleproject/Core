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

IF EXISTS (SELECT ID FROM sysobjects WHERE NAME = 'spSCHED_GetJobDetails' AND type = 'p')
    DROP PROCEDURE spSCHED_GetJobDetails
GO

/******************************************************************************
** Name    : spSCHED_GetJobDetails
**
** Summary:
**
**   Gets job details for a named job in a cluster.
**
** Example:

EXEC spSCHED_GetJobDetails 'TestCluster', 'job'

** Change History:
**
**   Date:    Author:  Bug #    Description:                           
**   -------- -------- ------   -----------------------------------------------
**   05/20/07 Jeff              Initial implementation.
*******************************************************************************
** Copyright (C) 2007 Castle Project, All Rights Reserved
*******************************************************************************/

CREATE PROCEDURE dbo.spSCHED_GetJobDetails
(
	@ClusterName NVARCHAR(200),
	@JobName NVARCHAR(200)
)
AS
BEGIN
	SET NOCOUNT ON
	SET LOCK_TIMEOUT 30000
	
	BEGIN TRY
		SET ROWCOUNT 1
		SELECT J.JobName, J.JobDescription, J.JobKey, J.TriggerObject, J.JobDataObject, J.CreationTime,
			J.JobState, J.NextTriggerFireTime, J.NextTriggerMisfireThresholdSeconds,
			J.LastExecutionSchedulerGUID, J.LastExecutionStartTime, J.LastExecutionEndTime, J.LastExecutionSucceeded, J.LastExecutionStatusMessage,
			J.Version
			FROM SCHED_Jobs J
			INNER JOIN SCHED_Clusters C ON C.ClusterID = J.ClusterID
			WHERE C.ClusterName = @ClusterName AND J.JobName = @JobName
	END TRY
	BEGIN CATCH
		EXEC spSCHED_RaiseError 'spSCHED_GetJobDetails', 'Could not get job details.'
	END CATCH
END
GO

GRANT EXECUTE ON dbo.spSCHED_GetJobDetails TO SchedulerRole
GO
