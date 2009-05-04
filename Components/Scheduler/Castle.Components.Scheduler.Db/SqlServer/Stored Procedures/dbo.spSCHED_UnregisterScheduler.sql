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

IF EXISTS (SELECT ID FROM sysobjects WHERE NAME = 'spSCHED_UnregisterScheduler' AND type = 'p')
    DROP PROCEDURE spSCHED_UnregisterScheduler
GO

/******************************************************************************
** Name    : spSCHED_UnregisterScheduler
**
** Summary:
**
**   Unregisters a scheduler instance in the database.
**
** Example:

EXEC spSCHED_UnregisterScheduler 'TestCluster', '{88F43A7B-7FE2-4593-B390-40858B5A0CBF}'

** Change History:
**
**   Date:    Author:  Bug #    Description:                           
**   -------- -------- ------   -----------------------------------------------
**   05/20/07 Jeff              Initial implementation.
*******************************************************************************
** Copyright (C) 2007 Castle Project, All Rights Reserved
*******************************************************************************/

CREATE PROCEDURE dbo.spSCHED_UnregisterScheduler
(
	@ClusterName NVARCHAR(200),
	@SchedulerGUID UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT ON
	SET LOCK_TIMEOUT 30000
	
	DECLARE @JobState_Running INT
	SET @JobState_Running = 3
	DECLARE @JobState_Orphaned INT
	SET @JobState_Orphaned = 5
	
	-- Delete the scheduler record.
	BEGIN TRY
		SET ROWCOUNT 1
		DELETE S
			FROM SCHED_Schedulers S
			INNER JOIN SCHED_Clusters C ON C.ClusterID = S.ClusterID
			WHERE C.ClusterName = @ClusterName
				AND S.SchedulerGUID = @SchedulerGUID
	END TRY
	BEGIN CATCH
		EXEC spSCHED_RaiseError 'spSCHED_UnregisterScheduler', 'Could not delete scheduler record.'
		RETURN
	END CATCH
	
	-- Immediately orphan all jobs currently on the scheduler.
	BEGIN TRY
		SET ROWCOUNT 0
		UPDATE SCHED_Jobs
			SET JobState = @JobState_Orphaned
			WHERE JobState = @JobState_Running
				AND LastExecutionSchedulerGUID = @SchedulerGUID
	END TRY
	BEGIN CATCH
		EXEC spSCHED_RaiseError 'spSCHED_UnregisterScheduler', 'Could not orphan scheduler''s running jobs.'
	END CATCH	
END
GO

GRANT EXECUTE ON dbo.spSCHED_UnregisterScheduler TO SchedulerRole
GO
