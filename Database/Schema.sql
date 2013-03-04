USE [KazMap]
GO
/****** Object:  Table [dbo].[Relations]    Script Date: 02/23/2013 21:28:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Relations](
	[RelationId] [bigint] NOT NULL,
 CONSTRAINT [PK_Relations] PRIMARY KEY CLUSTERED 
(
	[RelationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Nodes]    Script Date: 02/23/2013 21:28:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Nodes](
	[Id] [bigint] NOT NULL,
	[Longitude] [float] NULL,
	[Latitude] [float] NULL,
 CONSTRAINT [PK_Nodes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MapInfos]    Script Date: 02/23/2013 21:28:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MapInfos](
	[akey] [varchar](50) NULL,
	[avalue] [varchar](50) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WayNames]    Script Date: 02/23/2013 21:28:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WayNames](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NameId] [bigint] NULL,
	[Value] [varchar](100) NULL,
 CONSTRAINT [PK_WayNames] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TagTypes]    Script Date: 02/23/2013 21:28:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TagTypes](
	[Id] [bigint] NOT NULL,
	[Name] [varchar](100) NULL,
 CONSTRAINT [PK_TagTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Ways]    Script Date: 02/23/2013 21:28:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ways](
	[WayId] [bigint] NOT NULL,
 CONSTRAINT [PK_Ways] PRIMARY KEY CLUSTERED 
(
	[WayId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WayTags]    Script Date: 02/23/2013 21:28:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WayTags](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[WayId] [bigint] NULL,
	[TypeId] [bigint] NULL,
	[Value] [varchar](100) NULL,
 CONSTRAINT [PK_WayTags] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[NodeTags]    Script Date: 02/23/2013 21:28:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NodeTags](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NodeId] [bigint] NULL,
	[TypeId] [bigint] NULL,
	[Value] [varchar](100) NULL,
 CONSTRAINT [PK_NodeTags] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WayNodes]    Script Date: 02/23/2013 21:28:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WayNodes](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[WayId] [bigint] NULL,
	[NodeId] [bigint] NULL,
	[Position] [int] NULL,
 CONSTRAINT [PK_WayNodes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WayNamesReferences]    Script Date: 02/23/2013 21:28:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WayNamesReferences](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NameId] [bigint] NULL,
	[WayId] [bigint] NULL,
 CONSTRAINT [PK_WayNamesReferences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roads]    Script Date: 02/23/2013 21:28:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Roads](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[WayId] [bigint] NULL,
	[Category] [varchar](50) NULL,
	[MaxSpeed] [int] NULL,
 CONSTRAINT [PK_Roads] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RelationTags]    Script Date: 02/23/2013 21:28:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RelationTags](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RelationId] [bigint] NULL,
	[TypeId] [bigint] NULL,
	[Value] [varchar](100) NULL,
 CONSTRAINT [PK_RelationTags] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Intersections]    Script Date: 02/23/2013 21:28:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Intersections](
	[Id] [bigint] NOT NULL,
	[NodeId] [bigint] NULL,
 CONSTRAINT [PK_Intersections] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RelationNodes]    Script Date: 02/23/2013 21:28:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationNodes](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RelationId] [bigint] NULL,
	[ItemId] [bigint] NULL,
	[Position] [int] NULL,
	[TypeId] [bigint] NULL,
 CONSTRAINT [PK_RelationNodes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Path]    Script Date: 02/23/2013 21:28:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Path](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[LeftId] [bigint] NULL,
	[RightId] [bigint] NULL,
	[Weight] [int] NULL,
	[RoadId] [int] NULL,
	[LeftNodeId] [int] NULL,
	[RightNodeId] [int] NULL,
	[Distance] float
 CONSTRAINT [PK_Path] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_Intersection_NODE]    Script Date: 02/23/2013 21:28:00 ******/
ALTER TABLE [dbo].[Intersections]  WITH CHECK ADD  CONSTRAINT [FK_Intersection_NODE] FOREIGN KEY([NodeId])
REFERENCES [dbo].[Nodes] ([Id])
GO
ALTER TABLE [dbo].[Intersections] CHECK CONSTRAINT [FK_Intersection_NODE]
GO
/****** Object:  ForeignKey [FK_NodeTAG_NODE]    Script Date: 02/23/2013 21:28:00 ******/
ALTER TABLE [dbo].[NodeTags]  WITH CHECK ADD  CONSTRAINT [FK_NodeTAG_NODE] FOREIGN KEY([NodeId])
REFERENCES [dbo].[Nodes] ([Id])
GO
ALTER TABLE [dbo].[NodeTags] CHECK CONSTRAINT [FK_NodeTAG_NODE]
GO
/****** Object:  ForeignKey [FK_NodeTAG_TagType]    Script Date: 02/23/2013 21:28:00 ******/
ALTER TABLE [dbo].[NodeTags]  WITH CHECK ADD  CONSTRAINT [FK_NodeTAG_TagType] FOREIGN KEY([TypeId])
REFERENCES [dbo].[TagTypes] ([Id])
GO
ALTER TABLE [dbo].[NodeTags] CHECK CONSTRAINT [FK_NodeTAG_TagType]
GO
/****** Object:  ForeignKey [FK_Path_Left]    Script Date: 02/23/2013 21:28:00 ******/
ALTER TABLE [dbo].[Path]  WITH CHECK ADD  CONSTRAINT [FK_Path_Left] FOREIGN KEY([LeftId])
REFERENCES [dbo].[Intersections] ([Id])
GO
ALTER TABLE [dbo].[Path] CHECK CONSTRAINT [FK_Path_Left]
GO
/****** Object:  ForeignKey [FK_Path_Right]    Script Date: 02/23/2013 21:28:00 ******/
ALTER TABLE [dbo].[Path]  WITH CHECK ADD  CONSTRAINT [FK_Path_Right] FOREIGN KEY([RightId])
REFERENCES [dbo].[Intersections] ([Id])
GO
ALTER TABLE [dbo].[Path] CHECK CONSTRAINT [FK_Path_Right]
GO
/****** Object:  ForeignKey [FK_RelNODE_Relation]    Script Date: 02/23/2013 21:28:00 ******/
ALTER TABLE [dbo].[RelationNodes]  WITH CHECK ADD  CONSTRAINT [FK_RelNODE_Relation] FOREIGN KEY([RelationId])
REFERENCES [dbo].[Relations] ([RelationId])
GO
ALTER TABLE [dbo].[RelationNodes] CHECK CONSTRAINT [FK_RelNODE_Relation]
GO
/****** Object:  ForeignKey [FK_RelTAG_Relation]    Script Date: 02/23/2013 21:28:00 ******/
ALTER TABLE [dbo].[RelationTags]  WITH CHECK ADD  CONSTRAINT [FK_RelTAG_Relation] FOREIGN KEY([RelationId])
REFERENCES [dbo].[Relations] ([RelationId])
GO
ALTER TABLE [dbo].[RelationTags] CHECK CONSTRAINT [FK_RelTAG_Relation]
GO
/****** Object:  ForeignKey [FK_RelTAG_TagType]    Script Date: 02/23/2013 21:28:00 ******/
ALTER TABLE [dbo].[RelationTags]  WITH CHECK ADD  CONSTRAINT [FK_RelTAG_TagType] FOREIGN KEY([TypeId])
REFERENCES [dbo].[TagTypes] ([Id])
GO
ALTER TABLE [dbo].[RelationTags] CHECK CONSTRAINT [FK_RelTAG_TagType]
GO
/****** Object:  ForeignKey [FK_Road_Way]    Script Date: 02/23/2013 21:28:00 ******/
ALTER TABLE [dbo].[Roads]  WITH CHECK ADD  CONSTRAINT [FK_Road_Way] FOREIGN KEY([WayId])
REFERENCES [dbo].[Ways] ([WayId])
GO
ALTER TABLE [dbo].[Roads] CHECK CONSTRAINT [FK_Road_Way]
GO
/****** Object:  ForeignKey [FK_WayNamesReference_Way]    Script Date: 02/23/2013 21:28:00 ******/
ALTER TABLE [dbo].[WayNamesReferences]  WITH CHECK ADD  CONSTRAINT [FK_WayNamesReference_Way] FOREIGN KEY([WayId])
REFERENCES [dbo].[Ways] ([WayId])
GO
ALTER TABLE [dbo].[WayNamesReferences] CHECK CONSTRAINT [FK_WayNamesReference_Way]
GO
/****** Object:  ForeignKey [FK_WayNODE_NODE]    Script Date: 02/23/2013 21:28:00 ******/
ALTER TABLE [dbo].[WayNodes]  WITH CHECK ADD  CONSTRAINT [FK_WayNODE_NODE] FOREIGN KEY([NodeId])
REFERENCES [dbo].[Nodes] ([Id])
GO
ALTER TABLE [dbo].[WayNodes] CHECK CONSTRAINT [FK_WayNODE_NODE]
GO
/****** Object:  ForeignKey [FK_WayNODE_Way]    Script Date: 02/23/2013 21:28:00 ******/
ALTER TABLE [dbo].[WayNodes]  WITH CHECK ADD  CONSTRAINT [FK_WayNODE_Way] FOREIGN KEY([WayId])
REFERENCES [dbo].[Ways] ([WayId])
GO
ALTER TABLE [dbo].[WayNodes] CHECK CONSTRAINT [FK_WayNODE_Way]
GO
/****** Object:  ForeignKey [FK_WayTAG_TagType]    Script Date: 02/23/2013 21:28:00 ******/
ALTER TABLE [dbo].[WayTags]  WITH CHECK ADD  CONSTRAINT [FK_WayTAG_TagType] FOREIGN KEY([TypeId])
REFERENCES [dbo].[TagTypes] ([Id])
GO
ALTER TABLE [dbo].[WayTags] CHECK CONSTRAINT [FK_WayTAG_TagType]
GO
/****** Object:  ForeignKey [FK_WayTAG_Way]    Script Date: 02/23/2013 21:28:00 ******/
ALTER TABLE [dbo].[WayTags]  WITH CHECK ADD  CONSTRAINT [FK_WayTAG_Way] FOREIGN KEY([WayId])
REFERENCES [dbo].[Ways] ([WayId])
GO
ALTER TABLE [dbo].[WayTags] CHECK CONSTRAINT [FK_WayTAG_Way]
GO
