USE [PDCAList]
GO
DROP TABLE Lop, LopHistory, LopMaterial, LopHistoryMaterial, Subukol, Subukolhistory, Uzivatel, Permission, Role, Material, Notifikace, DbError, CustomTableSetting
GO
DROP TABLE UkolVedeni, UkolVedeniHistory, UkolVedeniMaterial, UkolOddeleni, UkolOddeleniHistory, UkolOddeleniMaterial, UkolVzorkovani, UkolVzorkovaniHistory, UkolVzorkovaniMaterial
GO
--drop trigger triggerHistorieLopu, triggerHistorieSubukolu
go
/****** Object:  Table [dbo].[DbError]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbError](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WindowsId] [varchar](50) NULL,
	[Message] [text] NULL,
	[Source] [text] NULL,
	[StackTrace] [text] NULL,
	[Time] [datetime] NULL,
 CONSTRAINT [PK_DbError] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Lop]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Lop](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Action] [int] NOT NULL CONSTRAINT [DF_Lop_Action]  DEFAULT ((0)),
	[Status] [int] NULL,
	[Produkt] [int] NULL,
	[StartDate] [date] NULL CONSTRAINT [DF_Lop_StartDate]  DEFAULT (getdate()),
	[PlannedCloseDate] [date] NULL,
	[CloseDate] [date] NULL,
	[FinishDate] [date] NULL,
	[CheckDate] [date] NULL,
	[LastChangedDate] [datetime] NULL DEFAULT (getdate()),
	[Zadavatel] [int] NULL,
	[Resitel] [int] NULL,
	[Nazev] [varchar](50) NULL,
	[Popis] [varchar](5000) NULL,
	[Komentar] [varchar](5000) NULL DEFAULT (''),
	[DeniedMessage] [varchar](5000) NULL,
	[Investice] [tinyint] NULL CONSTRAINT [DF_Lop_Investice]  DEFAULT ((0)),
	[LessonLearned] [tinyint] NULL CONSTRAINT [DF_Lop_LessonLearned]  DEFAULT ((0)),
	[Deleted] [tinyint] NOT NULL DEFAULT ((0)),
	[Dilna] [varchar](10) NULL,
 CONSTRAINT [PK_Lop] PRIMARY KEY CLUSTERED 
(
	[Id] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) --ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LopHistory]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LopHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Lop] [int] NULL,
	[Action] [int] NULL,
	[Status] [int] NULL,
	[Produkt] [int] NULL,
	[StartDate] [date] NULL,
	[PlannedCloseDate] [date] NULL,
	[CloseDate] [date] NULL,
	[FinishDate] [date] NULL,
	[CheckDate] [date] NULL,
	[LastChangedDate] [datetime] NULL,
	[Zadavatel] [int] NULL,
	[Resitel] [int] NULL,
	[Nazev] [varchar](50) NULL,
	[Popis] [varchar](5000) NULL,
	[Komentar] [varchar](5000) NULL,
	[DeniedMessage] [varchar](5000) NULL,
	[Investice] [tinyint] NULL,
	[LessonLearned] [tinyint] NULL,
	[Dilna] [varchar](10) NULL,
 CONSTRAINT [PK_LopHistory] PRIMARY KEY CLUSTERED 
(
	[Id] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) --ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LopMaterial]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LopMaterial](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Lop] [int] NOT NULL,
	[Produkt] [int] NOT NULL,
	[DateAdded] [date] NULL CONSTRAINT [DF_LopMaterial_2_DateAdded]  DEFAULT (getdate()),
 CONSTRAINT [PK_LopMaterial_2] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_LopMaterial_2_1] UNIQUE NONCLUSTERED 
(
	[Lop] ASC,
	[Produkt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[LopHistoryMaterial]    Script Date: 03.04.2016 16:13:25 ******/
GO
SET ANSI_PADDING ON
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LopHistoryMaterial](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LopHistory] [int] NULL,
	[Produkt] [int] NOT NULL,
	[DateAdded] [date] NULL CONSTRAINT [DF_LopHistoryMaterial_2_DateAdded]  DEFAULT (getdate()),
 CONSTRAINT [PK_LopHistoryMaterial_2] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
/*WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_LopHistoryMaterial_2_1] UNIQUE NONCLUSTERED 
(
	[LopHistory] ASC,
	[Produkt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] */


GO
/****** Object:  Table [dbo].[Material]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Material](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Druh] [varchar](20) NOT NULL,
	[PN] [varchar](50) NULL,
	[Popis] [varchar](50) NULL,
	[PopisSap] [varchar](50) NULL,
	[SapCislo] [varchar](50) NULL,
	[VedouciProjektu] [varchar](50) NULL,
	[ZodpovednostZaKvalitu] [varchar](50) NULL,
	[Metrologie] [varchar](50) NULL,
	[Baleni] [varchar](50) NULL,
 CONSTRAINT [PK_Material] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Notifikace]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notifikace](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Uzivatel] [int] NOT NULL,
	[Text] [text] NOT NULL,
	[Seen] [tinyint] NULL CONSTRAINT [DF_Table_1_Read]  DEFAULT ((0)),
	[Created] [datetime] NULL CONSTRAINT [DF_Notifikace_Created]  DEFAULT (getdate()),
 CONSTRAINT [PK_Notifikace] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Permission]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[Permission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WindowsId] [varchar](50) NOT NULL,
	[Role] [int] NOT NULL,
 CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Role]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Identificator] [varchar](20) NOT NULL,
	[Popis] [text] NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SubUkol]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SubUkol](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Lop] [int] NOT NULL,
	[Action] [int] NULL,
	[Status] [int] NULL,
	[StartDate] [date] NULL CONSTRAINT [DF_SubUkol_StartDate]  DEFAULT (getdate()),
	[PlannedCloseDate] [date] NULL,
	[CloseDate] [date] NULL,
	[FinishDate] [date] NULL,
	[CheckDate] [date] NULL,
	[LastChangedDate] [datetime] NULL DEFAULT (getdate()),
	[Zadavatel] [int] NULL,
	[Resitel] [int] NULL,
	[Nazev] [varchar](50) NULL,
	[Popis] [varchar](5000) NULL,
	[Komentar] [varchar](5000) NULL DEFAULT (''),
	[DeniedMessage] [varchar](5000) NULL,
	[Deleted] [tinyint] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_SubUkol] PRIMARY KEY CLUSTERED 
(
	[Id] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) --ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SubUkolHistory]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SubUkolHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubUkol] [int] NOT NULL,
	[Action] [int] NULL,
	[Status] [int] NULL,
	[StartDate] [date] NULL,
	[PlannedCloseDate] [date] NULL,
	[CloseDate] [date] NULL,
	[FinishDate] [date] NULL,
	[CheckDate] [date] NULL,
	[LastChangedDate] [datetime] NULL,
	[Zadavatel] [int] NULL,
	[Resitel] [int] NULL,
	[Nazev] [varchar](50) NULL,
	[Popis] [varchar](5000) NULL,
	[Komentar] [varchar](5000) NULL DEFAULT (''),
	[DeniedMessage] [varchar](5000) NULL,
 CONSTRAINT [PK_SubUkolHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) --ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UkolOddeleni]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UkolOddeleni](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Action] [int] NOT NULL,
	[Status] [int] NULL,
	[Zadavatel] [int] NOT NULL,
	[Produkt] [int] NULL,
	[Nazev] [varchar](50) NULL,
	[Popis] [text] NULL,
	[Komentar] [text] NULL,
	[Resitel] [int] NOT NULL,
	[DateStart] [date] NOT NULL,
	[DatePlannedClose] [date] NULL,
	[DateDeadline] [date] NULL,
	[Poznamka] [text] NULL,
	[DateFinish] [date] NULL,
	[DateCheck] [date] NULL,
	[DateLastChanged] [datetime] NULL,
	[LessonLearned] [tinyint] NULL,
	[Deleted] [tinyint] NOT NULL,
 CONSTRAINT [PK_UkolOddeleni] PRIMARY KEY CLUSTERED 
(
	[Id] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UkolOddeleniHistory]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UkolOddeleniHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UkolOddeleni] [int] NULL,
	[Action] [int] NOT NULL,
	[Status] [int] NULL,
	[Zadavatel] [int] NULL,
	[Produkt] [int] NULL,
	[Nazev] [varchar](50) NULL,
	[Popis] [text] NULL,
	[Komentar] [text] NULL,
	[Resitel] [int] NULL,
	[DateStart] [date] NOT NULL,
	[DatePlannedClose] [date] NULL,
	[DateDeadline] [date] NULL,
	[Poznamka] [text] NULL,
	[DateFinish] [date] NULL,
	[DateCheck] [date] NULL,
	[DateLastChanged] [datetime] NULL,
	[LessonLearned] [tinyint] NULL,
	[Deleted] [tinyint] NOT NULL,
 CONSTRAINT [PK_UkolOddeleniHistory] PRIMARY KEY CLUSTERED 
(
	[Id] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UkolOddeleniMaterial]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UkolOddeleniMaterial](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Ukol] [int] NOT NULL,
	[Produkt] [int] NOT NULL,
	[DateAdded] [date] NULL,
 CONSTRAINT [PK_UkolOddeleniMaterial_2] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_UkolOddeleniMaterial_2_1] UNIQUE NONCLUSTERED 
(
	[Ukol] ASC,
	[Produkt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UkolVedeni]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UkolVedeni](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Action] [int] NOT NULL,
	[Status] [int] NULL,
	[Zadavatel] [int] NOT NULL,
	[Nazev] [varchar](50) NULL,
	[Popis] [text] NULL,
	[Komentar] [text] NULL DEFAULT (''),
	[Resitel] [int] NOT NULL,
	[DateStart] [date] NOT NULL DEFAULT (getdate()),
	[DatePlannedClose] [date] NULL,
	[DateDeadline] [date] NULL,
	[Poznamka] [text] NULL,
	[DateFinish] [date] NULL,
	[DateCheck] [date] NULL,
	[DateLastChanged] [datetime] NULL DEFAULT (getdate()),
	[LessonLearned] [tinyint] NULL,
	[Deleted] [tinyint] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_UkolVedeni] PRIMARY KEY CLUSTERED 
(
	[Id] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UkolVedeniHistory]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UkolVedeniHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UkolVedeni] [int] NULL,
	[Action] [int] NOT NULL,
	[Status] [int] NULL,
	[Zadavatel] [int] NULL,
	[Nazev] [varchar](50) NULL,
	[Popis] [text] NULL,
	[Komentar] [text] NULL DEFAULT (''),
	[Resitel] [int] NULL,
	[DateStart] [date] NOT NULL,
	[DatePlannedClose] [date] NULL,
	[DateDeadline] [date] NULL,
	[Poznamka] [text] NULL,
	[DateFinish] [date] NULL,
	[DateCheck] [date] NULL,
	[DateLastChanged] [datetime] NULL,
	[LessonLearned] [tinyint] NULL,
	[Deleted] [tinyint] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_UkolVedeniHistory] PRIMARY KEY CLUSTERED 
(
	[Id] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UkolVedeniMaterial]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UkolVedeniMaterial](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Ukol] [int] NOT NULL,
	[Produkt] [int] NOT NULL,
	[DateAdded] [date] NULL CONSTRAINT [DF_UkolVedeniMaterial_2_DateAdded]  DEFAULT (getdate()),
 CONSTRAINT [PK_UkolVedeniMaterial_2] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_UkolVedeniMaterial_2_1] UNIQUE NONCLUSTERED 
(
	[Ukol] ASC,
	[Produkt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UkolVzorkovani]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UkolVzorkovani](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Action] [int] NOT NULL,
	[Status] [int] NULL,
	[Zadavatel] [int] NOT NULL,
	[Produkt] [int] NULL,
	[Nazev] [varchar](50) NULL,
	[Popis] [text] NULL,
	[Komentar] [text] NULL,
	[Resitel] [int] NOT NULL,
	[DateStart] [date] NOT NULL,
	[DatePlannedClose] [date] NULL,
	[DateDeadline] [date] NULL,
	[Poznamka] [text] NULL,
	[DateFinish] [date] NULL,
	[DateCheck] [date] NULL,
	[DateLastChanged] [datetime] NULL,
	[LessonLearned] [tinyint] NULL,
	[Deleted] [tinyint] NOT NULL,
 CONSTRAINT [PK_UkolVzorkovani] PRIMARY KEY CLUSTERED 
(
	[Id] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UkolVzorkovaniHistory]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UkolVzorkovaniHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UkolVzorkovani] [int] NULL,
	[Action] [int] NOT NULL,
	[Status] [int] NULL,
	[Zadavatel] [int] NULL,
	[Produkt] [int] NULL,
	[Nazev] [varchar](50) NULL,
	[Popis] [text] NULL,
	[Komentar] [text] NULL,
	[Resitel] [int] NULL,
	[DateStart] [date] NOT NULL,
	[DatePlannedClose] [date] NULL,
	[DateDeadline] [date] NULL,
	[Poznamka] [text] NULL,
	[DateFinish] [date] NULL,
	[DateCheck] [date] NULL,
	[DateLastChanged] [datetime] NULL,
	[LessonLearned] [tinyint] NULL,
	[Deleted] [tinyint] NOT NULL,
 CONSTRAINT [PK_UkolVzorkovaniHistory] PRIMARY KEY CLUSTERED 
(
	[Id] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UkolVzorkovaniMaterial]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UkolVzorkovaniMaterial](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Ukol] [int] NOT NULL,
	[Produkt] [int] NOT NULL,
	[DateAdded] [date] NULL,
 CONSTRAINT [PK_UkolVzorkovaniMaterial_2] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_UkolVzorkovaniMaterial_2_1] UNIQUE NONCLUSTERED 
(
	[Ukol] ASC,
	[Produkt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Uzivatel]    Script Date: 03.04.2016 16:13:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Uzivatel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WindowsId] [varchar](50) NOT NULL,
	[OsCislo] [varchar](50) NOT NULL,
	[Jmeno] [varchar](50) NULL,
	[Prijmeni] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Usek] [varchar](50) NULL,
	[Oddeleni] [varchar](50) NULL,
 CONSTRAINT [PK_Uzivatel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [Action]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Action] ON [dbo].[Lop]
(
	[Action] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [LessonLearned]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [LessonLearned] ON [dbo].[Lop]
(
	[LessonLearned] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [StartDate]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [StartDate] ON [dbo].[Lop]
(
	[StartDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Status]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Status] ON [dbo].[Lop]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Zadavatel]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Zadavatel] ON [dbo].[Lop]
(
	[Zadavatel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Dilna]    Script Date: 06.05.2016 13:51:00 ******/
CREATE NONCLUSTERED INDEX [Dilna] ON [dbo].[Lop]
(
	[Dilna] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Lop]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Lop] ON [dbo].[LopHistory]
(
	[Lop] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_LopMaterial_2]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [IX_LopMaterial_2] ON [dbo].[LopMaterial]
(
	[Lop] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [Druh]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Druh] ON [dbo].[Material]
(
	[Druh] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [PN]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [PN] ON [dbo].[Material]
(
	[PN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [SapCislo]    Script Date: 03.04.2016 16:13:25 ******/
CREATE UNIQUE NONCLUSTERED INDEX [SapCislo] ON [dbo].[Material]
(
	[SapCislo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Created]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Created] ON [dbo].[Notifikace]
(
	[Created] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Seen]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Seen] ON [dbo].[Notifikace]
(
	[Seen] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Uziatel]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Uziatel] ON [dbo].[Notifikace]
(
	[Uzivatel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [WindowsId]    Script Date: 03.04.2016 16:13:25 ******/
CREATE UNIQUE NONCLUSTERED INDEX [WindowsId] ON [dbo].[Permission]
(
	[WindowsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [Identificator]    Script Date: 03.04.2016 16:13:25 ******/
CREATE UNIQUE NONCLUSTERED INDEX [Identificator] ON [dbo].[Role]
(
	[Identificator] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Lop]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Lop] ON [dbo].[SubUkol]
(
	[Lop] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [SubUkol]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [SubUkol] ON [dbo].[SubUkolHistory]
(
	[SubUkol] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Action]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Action] ON [dbo].[UkolOddeleni]
(
	[Action] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [StartDate]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [StartDate] ON [dbo].[UkolOddeleni]
(
	[DateStart] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Status]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Status] ON [dbo].[UkolOddeleni]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Zadavatel]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Zadavatel] ON [dbo].[UkolOddeleni]
(
	[Zadavatel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Parrent]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Parrent] ON [dbo].[UkolOddeleniHistory]
(
	[UkolOddeleni] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Parrent]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Parrent] ON [dbo].[UkolOddeleniMaterial]
(
	[Ukol] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Action]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Action] ON [dbo].[UkolVedeni]
(
	[Action] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [StartDate]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [StartDate] ON [dbo].[UkolVedeni]
(
	[DateStart] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Status]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Status] ON [dbo].[UkolVedeni]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Zadavatel]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Zadavatel] ON [dbo].[UkolVedeni]
(
	[Zadavatel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Parrent]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Parrent] ON [dbo].[UkolVedeniHistory]
(
	[UkolVedeni] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Parrent]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Parrent] ON [dbo].[UkolVedeniMaterial]
(
	[Ukol] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Action]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Action] ON [dbo].[UkolVzorkovani]
(
	[Action] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [StartDate]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [StartDate] ON [dbo].[UkolVzorkovani]
(
	[DateStart] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Status]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Status] ON [dbo].[UkolVzorkovani]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Zadavatel]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Zadavatel] ON [dbo].[UkolVzorkovani]
(
	[Zadavatel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Parrent]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Parrent] ON [dbo].[UkolVzorkovaniHistory]
(
	[UkolVzorkovani] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Parrent]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Parrent] ON [dbo].[UkolVzorkovaniMaterial]
(
	[Ukol] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [Oddeleni]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Oddeleni] ON [dbo].[Uzivatel]
(
	[Oddeleni] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [PersonalNumber]    Script Date: 03.04.2016 16:13:25 ******/
CREATE UNIQUE NONCLUSTERED INDEX [PersonalNumber] ON [dbo].[Uzivatel]
(
	[OsCislo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [Usek]    Script Date: 03.04.2016 16:13:25 ******/
CREATE NONCLUSTERED INDEX [Usek] ON [dbo].[Uzivatel]
(
	[Usek] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [WinId]    Script Date: 03.04.2016 16:13:25 ******/
CREATE UNIQUE NONCLUSTERED INDEX [WinId] ON [dbo].[Uzivatel]
(
	[WindowsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DbError] ADD  DEFAULT (getdate()) FOR [Time]
GO
ALTER TABLE [dbo].[UkolOddeleni] ADD  DEFAULT ('') FOR [Komentar]
GO
ALTER TABLE [dbo].[UkolOddeleni] ADD  DEFAULT (getdate()) FOR [DateStart]
GO
ALTER TABLE [dbo].[UkolOddeleni] ADD  DEFAULT (getdate()) FOR [DateLastChanged]
GO
ALTER TABLE [dbo].[UkolOddeleni] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[UkolOddeleniHistory] ADD  DEFAULT ('') FOR [Komentar]
GO
ALTER TABLE [dbo].[UkolOddeleniHistory] ADD  DEFAULT (getdate()) FOR [DateStart]
GO
ALTER TABLE [dbo].[UkolOddeleniHistory] ADD  DEFAULT (getdate()) FOR [DateLastChanged]
GO
ALTER TABLE [dbo].[UkolOddeleniHistory] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[UkolOddeleniMaterial] ADD  CONSTRAINT [DF_UkolOddeleniMaterial_2_DateAdded]  DEFAULT (getdate()) FOR [DateAdded]
GO
ALTER TABLE [dbo].[UkolVzorkovani] ADD  DEFAULT ('') FOR [Komentar]
GO
ALTER TABLE [dbo].[UkolVzorkovani] ADD  DEFAULT (getdate()) FOR [DateStart]
GO
ALTER TABLE [dbo].[UkolVzorkovani] ADD  DEFAULT (getdate()) FOR [DateLastChanged]
GO
ALTER TABLE [dbo].[UkolVzorkovani] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[UkolVzorkovaniHistory] ADD  DEFAULT ('') FOR [Komentar]
GO
ALTER TABLE [dbo].[UkolVzorkovaniHistory] ADD  DEFAULT (getdate()) FOR [DateStart]
GO
ALTER TABLE [dbo].[UkolVzorkovaniHistory] ADD  DEFAULT (getdate()) FOR [DateLastChanged]
GO
ALTER TABLE [dbo].[UkolVzorkovaniHistory] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[UkolVzorkovaniMaterial] ADD  CONSTRAINT [DF_UkolVzorkovaniMaterial_2_DateAdded]  DEFAULT (getdate()) FOR [DateAdded]
GO

--TRIGGERY :3
CREATE TRIGGER triggerHistorieLopu
ON Lop
FOR UPDATE
AS
BEGIN
	SET NOCOUNT ON; --Nhibernate očekává změnu jednoho řádku.. a tady se změní dva ;)
	IF EXISTS (SELECT * FROM DELETED)
	--NEBO SE MĚNILA HISTORIE PRODUKTŮ
	BEGIN
	IF (SELECT Action FROM DELETED) != (SELECT Action FROM INSERTED) 
	OR (SELECT Status FROM DELETED) != (SELECT Status FROM INSERTED)
	OR (SELECT Produkt FROM DELETED) != (SELECT Produkt FROM INSERTED)
	OR (SELECT StartDate FROM DELETED) != (SELECT StartDate FROM INSERTED)
	OR (SELECT PlannedCloseDate FROM DELETED) != (SELECT PlannedCloseDate FROM INSERTED)
	OR (SELECT CloseDate FROM DELETED) != (SELECT CloseDate FROM INSERTED)
	OR (SELECT FinishDate FROM DELETED) != (SELECT FinishDate FROM INSERTED)
	OR (SELECT CheckDate FROM DELETED) != (SELECT CheckDate FROM INSERTED)
	OR (SELECT LastChangedDate FROM DELETED) != (SELECT LastChangedDate FROM INSERTED)
	OR (SELECT Zadavatel FROM DELETED) != (SELECT Zadavatel FROM INSERTED)
	OR (SELECT Resitel FROM DELETED) != (SELECT Resitel FROM INSERTED)
	OR (SELECT Nazev FROM DELETED) != (SELECT Nazev FROM INSERTED)
	OR (SELECT Popis FROM DELETED) != (SELECT Popis FROM INSERTED)
	OR (SELECT Komentar FROM DELETED) != (SELECT Komentar FROM INSERTED)
	OR (SELECT DeniedMessage FROM DELETED) != (SELECT DeniedMessage FROM INSERTED)
	OR (SELECT Investice FROM DELETED) != (SELECT Investice FROM INSERTED)
	OR (SELECT LessonLearned FROM DELETED) != (SELECT LessonLearned FROM INSERTED)
	OR (SELECT Dilna FROM DELETED) != (SELECT Dilna FROM INSERTED) 
	--nebo se změnily produkty
	OR (SELECT COUNT(*) FROM LopHistoryMaterial WHERE LopHistory IS NULL OR LopHistory = -1) > 0
		BEGIN
			INSERT INTO LopHistory 
			(Lop,Action,Status,Produkt,StartDate,PlannedCloseDate,CloseDate,
			FinishDate,CheckDate,LastChangedDate,Zadavatel,Resitel,Nazev,Popis,Komentar,
			DeniedMessage,Investice,LessonLearned,Dilna)   
			SELECT Id,Action,Status,Produkt,StartDate,PlannedCloseDate,CloseDate,
			FinishDate,CheckDate,LastChangedDate,Zadavatel,Resitel,Nazev,Popis,Komentar,
			DeniedMessage,Investice,LessonLearned,Dilna 
			FROM DELETED;
		END
	END
	
	SET NOCOUNT OFF; --A teď zase zapnout počítání
END;
go
CREATE TRIGGER triggerHistorieSubukolu
ON SubUkol
FOR UPDATE
AS
BEGIN
	SET NOCOUNT ON; --Nhibernate očekává změnu jednoho řádku.. a tady se změní dva ;)

	IF EXISTS (SELECT * FROM DELETED) --POKUD SE NĚCO UPDATOVALO
	--OR --NEBO SE MĚNILA HISTORIE PRODUKTŮ
	--(SELECT COUNT(*) FROM SubUkolHistoryMaterial WHERE SubUkolHistory = NULL OR SubUkolHistory = -1) > 0
	BEGIN
	IF (SELECT Action FROM DELETED) != (SELECT Action FROM INSERTED) 
	OR (SELECT Status FROM DELETED) != (SELECT Status FROM INSERTED)
	OR (SELECT StartDate FROM DELETED) != (SELECT StartDate FROM INSERTED)
	OR (SELECT PlannedCloseDate FROM DELETED) != (SELECT PlannedCloseDate FROM INSERTED)
	OR (SELECT CloseDate FROM DELETED) != (SELECT CloseDate FROM INSERTED)
	OR (SELECT FinishDate FROM DELETED) != (SELECT FinishDate FROM INSERTED)
	OR (SELECT CheckDate FROM DELETED) != (SELECT CheckDate FROM INSERTED)
	OR (SELECT LastChangedDate FROM DELETED) != (SELECT LastChangedDate FROM INSERTED)
	OR (SELECT Zadavatel FROM DELETED) != (SELECT Zadavatel FROM INSERTED)
	OR (SELECT Resitel FROM DELETED) != (SELECT Resitel FROM INSERTED)
	OR (SELECT Nazev FROM DELETED) != (SELECT Nazev FROM INSERTED)
	OR (SELECT Popis FROM DELETED) != (SELECT Popis FROM INSERTED)
	OR (SELECT Komentar FROM DELETED) != (SELECT Komentar FROM INSERTED)
	OR (SELECT DeniedMessage FROM DELETED) != (SELECT DeniedMessage FROM INSERTED)
	--OR (SELECT Deleted FROM DELETED) != (SELECT Deleted FROM INSERTED)
		BEGIN
			INSERT INTO SubUkolHistory 
			(SubUkol,Action,Status,StartDate,PlannedCloseDate,CloseDate,
			FinishDate,CheckDate,LastChangedDate,Zadavatel,Resitel,Nazev,Popis,Komentar,
			DeniedMessage)   
			SELECT Id,Action,Status,StartDate,PlannedCloseDate,CloseDate,
			FinishDate,CheckDate,LastChangedDate,Zadavatel,Resitel,Nazev,Popis,Komentar,
			DeniedMessage
			FROM DELETED;
		END
	END
	SET NOCOUNT OFF; --A teď zase zapnout počítání
END;

GO
CREATE TRIGGER triggerHistorieLopMaterialu
ON LopMaterial
FOR INSERT, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	--IF EXISTS (SELECT * FROM LopHistory WHERE Lop = (SELECT Lop FROM INSERTED))
	--BEGIN
		IF EXISTS (SELECT * FROM INSERTED) --když řádky k lopu přidáváme, tak v historii je nechceme
		BEGIN	--proto si je označíme -1kou
			--A TEĎ VLOŽÍME I INSERTED ŘÁDEK S -1 A PAK HO SMAŽEME
			INSERT INTO LopHistoryMaterial (LopHistory,Produkt,DateAdded)
			VALUES (-1, (SELECT Produkt FROM INSERTED), (SELECT DateAdded FROM INSERTED))
		END
		--POKUD SE DILÍTOVALO TAK TENTO ŘÁDEK K HISTORII PŘIPÍŠEME .. hodíme si tam hodnotu null ;)
		ELSE IF EXISTS (SELECT * FROM DELETED)
		BEGIN
			INSERT INTO LopHistoryMaterial (LopHistory, Produkt, DateAdded) VALUES (NULL, (SELECT Produkt FROM Deleted), (SELECT DateAdded FROM Deleted));
		END
	--END
	SET NOCOUNT OFF; 
END

GO
CREATE TRIGGER triggerPresypaniMaterialuLop
ON LopHistory
FOR INSERT
AS
BEGIN
	--Je zapotřebí přesypat materiály k historii...
	--Ale může tam být hodnota s NULL nebo -1 v Historii
	--Pokud je tam NULL, tak ten null přepíšeme na IDčko nově vzniklé historie
	--Pokud je tam -1 tak tento řádek smažeme a ani ho nevložíme z ostatních

	--samozřejmě jsme si z předchozího triggru zapsali jen čerstvě přidané nebo čerstvě odebrané
	--produkty.. takže tam přisypeme násleudjícím kódem všechny ostatní..
	--ale ne ty, kde je -1 :P
	--KOPIE VŠECH ŘÁDKŮ KTERÉ JEŠTĚ NEJSOU ZKOPÍROVÁNY	
		DECLARE @Pr INT
		DECLARE @Da DATE
		DECLARE @Rm CURSOR
		SET @Rm = CURSOR FOR 
			SELECT LopMaterial.Produkt, LopMaterial.DateAdded
			FROM LopMaterial WHERE Lop = (SELECT Lop FROM INSERTED);
		OPEN @Rm
		FETCH NEXT FROM @Rm INTO @Pr,@Da
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF (NOT EXISTS(SELECT * FROM LopHistoryMaterial --pokud již takový produkt není zapsán s -1kou tak ho přidáme
			WHERE LopHistory = -1 AND Produkt = @Pr))
			BEGIN
				INSERT INTO LopHistoryMaterial (LopHistory,Produkt,DateAdded)
				VALUES (NULL, @Pr, @Da)
			END
			FETCH NEXT FROM @Rm INTO @Pr,@Da
		END
		CLOSE @Rm
		DEALLOCATE @Rm
		--a všechny nULL přepsat na ID
		UPDATE LopHistoryMaterial SET LopHistory = (SELECT Id FROM INSERTED) WHERE LopHistory is NULL --nebo @@IDENTITY
		DELETE LopHistoryMaterial WHERE LopHistory = -1
		
END

select * from lop
select * from LopMaterial
select * from LopHistory
select * from LopHistoryMaterial



/****** Object:  Table [dbo].[CustomTableSetting]    Script Date: 16.05.2016 15:44:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomTableSetting](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Uzivatel] [int] NOT NULL,
	[Tabulka] [varchar](50) NOT NULL,
	[Sloupec] [varchar](50) NOT NULL,
	[Zobrazit] [tinyint] NOT NULL CONSTRAINT [DF_CustomTableSetting_Zobrazit]  DEFAULT ((1)),
 CONSTRAINT [PK_CustomTableSetting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Table]    Script Date: 16.05.2016 15:44:11 ******/
CREATE NONCLUSTERED INDEX [IX_Table] ON [dbo].[CustomTableSetting]
(
	[Tabulka] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_User]    Script Date: 16.05.2016 15:44:11 ******/
CREATE NONCLUSTERED INDEX [IX_User] ON [dbo].[CustomTableSetting]
(
	[Uzivatel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [Uzivatel-Tabulka-Sloupec]    Script Date: 16.05.2016 15:44:11 ******/
CREATE UNIQUE NONCLUSTERED INDEX [Uzivatel-Tabulka-Sloupec] ON [dbo].[CustomTableSetting]
(
	[Uzivatel] ASC,
	[Tabulka] ASC,
	[Sloupec] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
