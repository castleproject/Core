if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Category]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Category]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Product]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Product]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[User]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[User]
GO

CREATE TABLE [dbo].[Category] (
	[id] [int] IDENTITY (1, 1) NOT NULL ,
	[name] [varchar] (25) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[parent_category_id] [int] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Product] (
	[id] [int] IDENTITY (1, 1) NOT NULL ,
	[name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[description] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[pictureFile] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[price] [money] NULL ,
	[category_id] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[User] (
	[id] [int] IDENTITY (1, 1) NOT NULL ,
	[login] [varchar] (15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[password] [varchar] (12) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[name] [varchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[email] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[type] [varchar] (8) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[address] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[city] [varchar] (15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[country] [varchar] (15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[zipcode] [varchar] (8) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[lastAccess] [smalldatetime] NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Category] WITH NOCHECK ADD 
	CONSTRAINT [PK_Category] PRIMARY KEY  CLUSTERED 
	(
		[id]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Product] WITH NOCHECK ADD 
	CONSTRAINT [PK_Product] PRIMARY KEY  CLUSTERED 
	(
		[id]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[User] WITH NOCHECK ADD 
	CONSTRAINT [PK_User] PRIMARY KEY  CLUSTERED 
	(
		[id]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[User] ADD 
	CONSTRAINT [IX_User] UNIQUE  NONCLUSTERED 
	(
		[login]
	)  ON [PRIMARY] 
GO

