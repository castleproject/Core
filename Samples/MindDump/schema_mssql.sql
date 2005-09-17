
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Author]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Author]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Blog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Blog]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Post]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Post]
GO

CREATE TABLE [dbo].[Author] (
	[id] [int] IDENTITY (1, 1) NOT NULL ,
	[name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[login] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[password] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Blog] (
	[id] [int] IDENTITY (1, 1) NOT NULL ,
	[name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[description] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[theme] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[author_id] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Post] (
	[id] [int] IDENTITY (1, 1) NOT NULL ,
	[title] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[contents] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[date] [smalldatetime] NOT NULL ,
	[blog_id] [int] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Author] WITH NOCHECK ADD 
	CONSTRAINT [PK_Author] PRIMARY KEY  CLUSTERED 
	(
		[id]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Blog] WITH NOCHECK ADD 
	CONSTRAINT [PK_Blog] PRIMARY KEY  CLUSTERED 
	(
		[id]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Post] WITH NOCHECK ADD 
	CONSTRAINT [PK_post] PRIMARY KEY  CLUSTERED 
	(
		[id]
	)  ON [PRIMARY] 
GO

