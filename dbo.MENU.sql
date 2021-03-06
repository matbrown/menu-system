USE [Generic]
GO

/****** Object:  Table [dbo].[MENU]    Script Date: 08/02/2020 15:40:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MENU](
	[MENU_ID] [int] IDENTITY(1,1) NOT NULL,
	[PARENT_MENU_ID] [int] NULL,
	[TITLE] [varchar](100) NOT NULL,
	[DESCRIPTION] [varchar](max) NULL,
	[URL] [varchar](500) NULL,
	[VISIBLE] [bit] NOT NULL,
	[ORDER] [int] NOT NULL,
 CONSTRAINT [PK_MENU_ID] PRIMARY KEY CLUSTERED 
(
	[MENU_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[MENU]  WITH CHECK ADD  CONSTRAINT [FK_PARENT_MENU_ID] FOREIGN KEY([PARENT_MENU_ID])
REFERENCES [dbo].[MENU] ([MENU_ID])
GO

ALTER TABLE [dbo].[MENU] CHECK CONSTRAINT [FK_PARENT_MENU_ID]
GO

