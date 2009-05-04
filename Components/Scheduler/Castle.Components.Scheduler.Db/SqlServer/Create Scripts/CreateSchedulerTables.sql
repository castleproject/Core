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

/******************************************************************************
** Purpose : Create Scheduler Tables.
**
** Summary:
**
**   This script creates tables used by Castle.Components.Scheduler for SqlServer.
**   All dates used are in UTC.
**
** Change History:
**
**   Date:    Author:  Bug #    Description:                           
**   -------- -------- ------   -----------------------------------------------
**   05/20/07 Jeff              Initial creation.
*******************************************************************************
** Copyright (C) 2007 Castle, All Rights Reserved
*******************************************************************************/

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.SCHED_Jobs') AND type in (N'U'))
	DROP TABLE dbo.SCHED_Jobs
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.SCHED_Schedulers') AND type in (N'U'))
	DROP TABLE dbo.SCHED_Schedulers
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.SCHED_Clusters') AND type in (N'U'))
	DROP TABLE dbo.SCHED_Clusters
GO


/**
 * Create SCHED_Clusters.
 * Names and identifies all scheduler clusters that exist in the Db.
 * Generally there are only a few cluster rows defined in the Db and they persist
 * for the lifetime of the system.
 */
CREATE TABLE [dbo].[SCHED_Clusters]
(
	ClusterID INT IDENTITY
		CONSTRAINT PKC_SCHED_Clusters_ClusterID PRIMARY KEY CLUSTERED,

	ClusterName NVARCHAR(200) NOT NULL
		CONSTRAINT UNQ_SCHED_Clusters_ClusterName UNIQUE NONCLUSTERED
)
GO


/**
 * Create SCHED_Schedulers.
 * Identifies all scheduler instances and the cluster to which
 * they belong in the Db.  One scheduler row is created per active scheduler
 * component (or process) that has a running job.  When the scheduler is terminated
 * its associated records are cleaned up.
 */
CREATE TABLE [dbo].[SCHED_Schedulers]
(
	SchedulerID INT IDENTITY
		CONSTRAINT PKC_SCHED_Schedulers_SchedulerID PRIMARY KEY CLUSTERED,

	ClusterID INT NOT NULL
		CONSTRAINT FK_SCHED_Schedulers_ClusterID__SCHED_Clusters FOREIGN KEY REFERENCES dbo.SCHED_Clusters (ClusterID),

	SchedulerGUID UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT UNQ_SCHED_Schedulers_SchedulerGUID UNIQUE NONCLUSTERED,

	SchedulerName NVARCHAR(200) NOT NULL,

	LastSeen DATETIME NOT NULL
)
GO


/**
 * Create SCHED_Jobs.
 * Tracks the status of all scheduled jobs.
 * Running jobs are identified by the (non-null) scheduler instance to which they
 * are associated.
 */
CREATE TABLE [dbo].[SCHED_Jobs]
(
	JobID INT IDENTITY 
		CONSTRAINT PKC_SCHED_Jobs_JobID PRIMARY KEY CLUSTERED,

	ClusterID INT NOT NULL
		CONSTRAINT FK_SCHED_Jobs_ClusterID__SCHED_Clusters FOREIGN KEY REFERENCES dbo.SCHED_Clusters (ClusterID),

	JobName NVARCHAR(200) NOT NULL,

	JobDescription NVARCHAR(1000) NOT NULL,

	JobKey NVARCHAR(200) NOT NULL,

	TriggerObject VARBINARY(MAX) NULL,
	
	JobDataObject VARBINARY(MAX) NULL,
	
	CreationTime DATETIME NOT NULL,
	
	JobState INT NOT NULL,
	
	NextTriggerFireTime DATETIME NULL,

	NextTriggerMisfireThresholdSeconds INT NULL,

	LastExecutionSchedulerGUID UNIQUEIDENTIFIER NULL,

	LastExecutionStartTime DATETIME NULL,
	
	LastExecutionEndTime DATETIME NULL,
	
	LastExecutionSucceeded BIT NULL,
	
	LastExecutionStatusMessage NVARCHAR(MAX),
	
	Version INT NOT NULL
		CONSTRAINT DF_SCHED_Jobs_Version DEFAULT (0)
	
	CONSTRAINT UNQ_SCHED_Jobs_JobName_ClusterID UNIQUE NONCLUSTERED
	(
		ClusterID ASC,
		JobName ASC
	)
)
GO

CREATE INDEX NDX_SCHED_Jobs_NextTriggerFireTime ON dbo.SCHED_Jobs
(
	NextTriggerFireTime ASC
)
GO

CREATE INDEX NDX_SCHED_Jobs_LastExecutionSchedulerGUID ON dbo.SCHED_Jobs
(
	LastExecutionSchedulerGUID
)
GO
