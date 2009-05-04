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

IF EXISTS (SELECT ID FROM sysobjects WHERE NAME = 'spSCHED_ListJobNames' AND type = 'p')
    DROP PROCEDURE spSCHED_ListJobNames
GO

/******************************************************************************
** Name    : spSCHED_ListJobNames
**
** Summary:
**
**   Gets a list of the names of all non-deleted jobs.
**
** Example:

EXEC spSCHED_ListJobNames 'TestCluster'

** Change History:
**
**   Date:    Author:  Bug #    Description:                           
**   -------- -------- ------   -----------------------------------------------
**   06/01/07 Jeff              Initial implementation.
*******************************************************************************
** Copyright (C) 2007 Castle Project, All Rights Reserved
*******************************************************************************/

CREATE PROCEDURE dbo.spSCHED_ListJobNames
(
	@ClusterName NVARCHAR(200)
)
AS
BEGIN
	SET NOCOUNT ON
	SET LOCK_TIMEOUT 30000
	
	BEGIN TRY
		SELECT JobName FROM SCHED_Jobs
	END TRY
	BEGIN CATCH
		EXEC spSCHED_RaiseError 'spSCHED_ListJobNames', 'Could not get job names.'
	END CATCH
END
GO

GRANT EXECUTE ON dbo.spSCHED_ListJobNames TO SchedulerRole
GO
 