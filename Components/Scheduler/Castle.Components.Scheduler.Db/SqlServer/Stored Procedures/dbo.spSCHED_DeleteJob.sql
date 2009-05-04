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

IF EXISTS (SELECT ID FROM sysobjects WHERE NAME = 'spSCHED_DeleteJob' AND type = 'p')
    DROP PROCEDURE spSCHED_DeleteJob
GO

/******************************************************************************
** Name    : spSCHED_DeleteJob
**
** Summary:
**
**   Deletes a job in the database.
**
** Example:

DECLARE @WasDeleted BIT
EXEC spSCHED_DeleteJob 'TestCluster', 'job', @WasDeleted OUTPUT
SELECT @WasDeleted

** Change History:
**
**   Date:    Author:  Bug #    Description:                           
**   -------- -------- ------   -----------------------------------------------
**   05/20/07 Jeff              Initial implementation.
*******************************************************************************
** Copyright (C) 2007 Castle Project, All Rights Reserved
*******************************************************************************/

CREATE PROCEDURE dbo.spSCHED_DeleteJob
(
	@ClusterName NVARCHAR(200),
	@JobName NVARCHAR(200),
	@WasDeleted BIT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON
	SET LOCK_TIMEOUT 30000
	
	SET @WasDeleted = 0

	BEGIN TRY
		BEGIN TRAN
	
		-- Delete the job if it exists.
		SET ROWCOUNT 1
		DELETE J
			FROM SCHED_Jobs J
			INNER JOIN SCHED_Clusters C ON C.ClusterID = J.ClusterID
			WHERE C.ClusterName = @ClusterName AND J.JobName = @JobName
			
		IF @@ROWCOUNT <> 0
			SET @WasDeleted = 1
	
		COMMIT
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0 ROLLBACK
		EXEC spSCHED_RaiseError 'spSCHED_DeleteJob', 'Could not delete job.'
	END CATCH
END
GO

GRANT EXECUTE ON dbo.spSCHED_DeleteJob TO SchedulerRole
GO
