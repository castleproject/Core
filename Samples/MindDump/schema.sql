USE [minddump]
GO

CREATE TABLE [dbo].[Author] (
	[id] [int] IDENTITY (1, 1) NOT NULL ,
	[name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[login] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[password] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Blog] (
	[id] [int] IDENTITY (1, 1) NOT NULL ,
	[author_id] [int] NULL ,
	[name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[description] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[theme] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Post] (
	[id] [int] IDENTITY (1, 1) NOT NULL ,
	[blog_id] [int] NULL ,
	[title] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[contents] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[date] [datetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Author] WITH NOCHECK ADD
	CONSTRAINT [PK_Author] PRIMARY KEY CLUSTERED
	(
		[id]
	) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Blog] WITH NOCHECK ADD
	CONSTRAINT [PK_Blog] PRIMARY KEY CLUSTERED
	(
		[id]
	) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Post] WITH NOCHECK ADD
	CONSTRAINT [PK_Post] PRIMARY KEY CLUSTERED
	(
		[id]
	) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Blog] WITH NOCHECK ADD
	CONSTRAINT [FK_Blog_Author] FOREIGN KEY
	(
		[author_id]
	) REFERENCES [dbo].[Author] (
		[id]
	)
GO

ALTER TABLE [dbo].[Post] WITH NOCHECK ADD
	CONSTRAINT [FK_Post_Blog] FOREIGN KEY
	(
		[blog_id]
	) REFERENCES [dbo].[Blog] (
		[id]
	)
GO
