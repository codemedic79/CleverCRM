USE [master]
GO
/****** Object:  Database [CleverCRM] ******/

CREATE DATABASE [CleverCRM]
GO
ALTER DATABASE [CleverCRM] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [CleverCRM] SET ANSI_NULLS OFF
GO
ALTER DATABASE [CleverCRM] SET ANSI_PADDING OFF
GO
ALTER DATABASE [CleverCRM] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [CleverCRM] SET ARITHABORT OFF
GO
ALTER DATABASE [CleverCRM] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [CleverCRM] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [CleverCRM] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [CleverCRM] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [CleverCRM] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [CleverCRM] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [CleverCRM] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [CleverCRM] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [CleverCRM] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [CleverCRM] SET  DISABLE_BROKER
GO
ALTER DATABASE [CleverCRM] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [CleverCRM] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [CleverCRM] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [CleverCRM] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [CleverCRM] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [CleverCRM] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [CleverCRM] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [CleverCRM] SET RECOVERY SIMPLE
GO
ALTER DATABASE [CleverCRM] SET  MULTI_USER
GO
ALTER DATABASE [CleverCRM] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [CleverCRM] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'CleverCRM', N'ON'
GO
USE [CleverCRM]
GO
/****** Object:  Table [dbo].[Contacts] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contacts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](256) NOT NULL,
	[Company] [nvarchar](256) NULL,
	[LastName] [nvarchar](256) NULL,
	[FirstName] [nvarchar](256) NULL,
	[Phone] [nvarchar](256) NULL,
 CONSTRAINT [PK_Contacts_1] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Opportunities] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Opportunities](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Amount] [money] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ContactId] [int] NOT NULL,
	[StatusId] [int] NOT NULL,
	[CloseDate] [datetime] NOT NULL

 CONSTRAINT [PK_Opportunities] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OpportunityStatuses] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OpportunityStatuses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_OpportunityStatuses] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tasks] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tasks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](256) NULL,
	[OpportunityId] [int] NOT NULL,
	[DueDate] [datetime] NOT NULL,
	[TypeId] [int] NOT NULL,
	[StatusId] [int] NULL,
 CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskStatuses] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskStatuses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](256) NULL,
 CONSTRAINT [PK_TaskStatuses] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskTypes] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_TaskTypes] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
ALTER TABLE [dbo].[Opportunities]  WITH CHECK ADD  CONSTRAINT [FK_Opportunities_Contacts] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contacts] ([Id])
GO
ALTER TABLE [dbo].[Opportunities] CHECK CONSTRAINT [FK_Opportunities_Contacts]
GO
ALTER TABLE [dbo].[Opportunities]  WITH CHECK ADD  CONSTRAINT [FK_Opportunities_OpportunityStatuses] FOREIGN KEY([StatusId])
REFERENCES [dbo].[OpportunityStatuses] ([Id])
GO
ALTER TABLE [dbo].[Opportunities] CHECK CONSTRAINT [FK_Opportunities_OpportunityStatuses]
GO
ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK_Tasks_Opportunities] FOREIGN KEY([OpportunityId])
REFERENCES [dbo].[Opportunities] ([Id])
GO
ALTER TABLE [dbo].[Tasks] CHECK CONSTRAINT [FK_Tasks_Opportunities]
GO
ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK_Tasks_TaskStatuses] FOREIGN KEY([StatusId])
REFERENCES [dbo].[TaskStatuses] ([Id])
GO
ALTER TABLE [dbo].[Tasks] CHECK CONSTRAINT [FK_Tasks_TaskStatuses]
GO
ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK_Tasks_TaskTypes] FOREIGN KEY([TypeId])
REFERENCES [dbo].[TaskTypes] ([Id])
GO
ALTER TABLE [dbo].[Tasks] CHECK CONSTRAINT [FK_Tasks_TaskTypes]
GO
USE [master]
GO
ALTER DATABASE [CleverCRM] SET  READ_WRITE
GO