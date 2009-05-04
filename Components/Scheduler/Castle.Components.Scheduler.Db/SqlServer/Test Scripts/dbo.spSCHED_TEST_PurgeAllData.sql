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

IF EXISTS (SELECT ID FROM sysobjects WHERE NAME = 'spSCHED_TEST_PurgeAllData' AND type = 'p')
    DROP PROCEDURE spSCHED_TEST_PurgeAllData
GO

/******************************************************************************
** Name    : spSCHED_TEST_PurgeAllData
**
** Summary:
**
**   Purges all contents of the scheduling database.
**   This stored procedure should only be deployed in testing environments.
**
** Example:

EXEC spSCHED_TEST_PurgeAllData

** Change History:
**
**   Date:    Author:  Bug #    Description:                           
**   -------- -------- ------   -----------------------------------------------
**   05/20/07 Jeff              Initial implementation.
*******************************************************************************
** Copyright (C) 2007 Castle Project, All Rights Reserved
*******************************************************************************/

CREATE PROCEDURE dbo.spSCHED_TEST_PurgeAllData
AS
BEGIN
	DELETE SCHED_Jobs
	DELETE SCHED_Schedulers
	DELETE SCHED_Clusters
END
GO

GRANT EXECUTE ON dbo.spSCHED_TEST_PurgeAllData TO SchedulerRole
GO