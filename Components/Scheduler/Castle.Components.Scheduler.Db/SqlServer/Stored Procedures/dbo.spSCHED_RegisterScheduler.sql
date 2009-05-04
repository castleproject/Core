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

IF EXISTS (SELECT ID FROM sysobjects WHERE NAME = 'spSCHED_RegisterScheduler' AND type = 'p')
    DROP PROCEDURE spSCHED_RegisterScheduler
GO

/******************************************************************************
** Name    : spSCHED_RegisterScheduler
**
** Summary:
**
**   Registers a scheduler instance in the database.
**
** Example:

DECLARE @LastSeen DATETIME
SET @LastSeen = GETUTCDATE()
EXEC spSCHED_RegisterScheduler 'TestCluster', '{88F43A7B-7FE2-4593-B390-40858B5A0CBF}', 'This scheduler.', @LastSeen

** Change History:
**
**   Date:    Author:  Bug #    Description:                           
**   -------- -------- ------   -----------------------------------------------
**   05/20/07 Jeff              Initial implementation.
*******************************************************************************
** Copyright (C) 2007 Castle Project, All Rights Reserved
*******************************************************************************/

CREATE PROCEDURE dbo.spSCHED_RegisterScheduler
(
	@ClusterName NVARCHAR(200),
	@SchedulerGUID UNIQUEIDENTIFIER,
	@SchedulerName NVARCHAR(200),
	@LastSeen DATETIME
)
AS
BEGIN
	SET NOCOUNT ON
	SET LOCK_TIMEOUT 30000
	
	DECLARE @ClusterID INT
	DECLARE @SchedulerID INT
	
	-- Create the cluster if needed.
	BEGIN TRY
		BEGIN TRAN
		
		SELECT @ClusterID = ClusterID
			FROM SCHED_Clusters
			WHERE ClusterName = @ClusterName
			
		IF @ClusterID IS NULL
		BEGIN
			INSERT INTO SCHED_Clusters (ClusterName)
				VALUES (@ClusterName)
			SET @ClusterID = SCOPE_IDENTITY()
		END

		COMMIT
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0 ROLLBACK
		EXEC spSCHED_RaiseError 'spSCHED_RegisterScheduler', 'Could not create cluster record.'
	END CATCH
	
	-- Create the scheduler record.
	BEGIN TRY
		BEGIN TRAN
	
		SELECT @SchedulerID = SchedulerID
			FROM SCHED_Schedulers
			WHERE ClusterID = @ClusterID AND SchedulerGUID = @SchedulerGUID
			
		IF @SchedulerID IS NULL
		BEGIN
			INSERT INTO SCHED_Schedulers (ClusterID, SchedulerGUID, SchedulerName, LastSeen)
				VALUES (@ClusterID, @SchedulerGUID, @SchedulerName, @LastSeen)
			SET @SchedulerID = SCOPE_IDENTITY()
		END ELSE BEGIN
			UPDATE SCHED_Schedulers
				SET SchedulerName = @SchedulerName,
					LastSeen = @LastSeen
				WHERE ClusterID = @ClusterID AND SchedulerGUID = @SchedulerGUID
		END
		
		COMMIT
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0 ROLLBACK
		EXEC spSCHED_RaiseError 'spSCHED_RegisterScheduler', 'Could not create or update scheduler record.'
	END CATCH
END
GO

GRANT EXECUTE ON dbo.spSCHED_RegisterScheduler TO SchedulerRole
GO
