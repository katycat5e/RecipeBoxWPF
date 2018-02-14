USE [master]
GO
 /****** Object:  Database [Cookbook]    Script Date: 6/12/2017 9:30:21 AM ******/
CREATE DATABASE [Cookbook]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Cookbook', FILENAME = N'C:\RecipeBox\Cookbook.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Cookbook_log', FILENAME = N'C:\RecipeBox\Cookbook_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Cookbook] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Cookbook].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Cookbook] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Cookbook] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Cookbook] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Cookbook] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Cookbook] SET ARITHABORT OFF 
GO
ALTER DATABASE [Cookbook] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Cookbook] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Cookbook] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Cookbook] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Cookbook] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Cookbook] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Cookbook] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Cookbook] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Cookbook] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Cookbook] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Cookbook] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Cookbook] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Cookbook] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Cookbook] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Cookbook] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Cookbook] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Cookbook] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Cookbook] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Cookbook] SET  MULTI_USER 
GO
ALTER DATABASE [Cookbook] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Cookbook] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Cookbook] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Cookbook] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [Cookbook] SET DELAYED_DURABILITY = DISABLED 
GO
USE [Cookbook]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 6/12/2017 9:30:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Categories](
	[C_ID] [int] IDENTITY(1,1) NOT NULL,
	[C_Name] [varchar](20) NULL CONSTRAINT [DF_Categories_C_Name]  DEFAULT ('NewCategory'),
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[C_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Images]    Script Date: 10/26/2017 1:28:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Images](
	[IMG_ID] [int] IDENTITY(1,1) NOT NULL,
	[IMG_RecipeID] [int] NULL,
	[IMG_FileName] [varchar](255) NULL,
	[IMG_Data] [varbinary](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[IMG_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Ingredients]    Script Date: 6/20/2017 7:55:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Ingredients](
	[IE_ID] [int] IDENTITY(1,1) NOT NULL,
	[IE_Name] [varchar](40) NULL,
	[IE_Amount] [decimal](9, 3) NULL CONSTRAINT [DF_IngredEntries_IE_Amount]  DEFAULT ((0.000)),
	[IE_Unit] [int] NULL CONSTRAINT [DF_IngredEntries_IE_Unit]  DEFAULT ((1)),
	[IE_RecipeID] [int] NULL CONSTRAINT [DF_Ingredients_IE_RecipeID]  DEFAULT ((0)),
 CONSTRAINT [PK_IngredEntries] PRIMARY KEY CLUSTERED 
(
	[IE_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Recipes]    Script Date: 6/12/2017 9:30:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Recipes](
	[R_ID] [int] IDENTITY(1,1) NOT NULL,
	[R_Name] [varchar](50) NOT NULL CONSTRAINT [DF_Recipes_R_Name]  DEFAULT ('NewRecipe'),
	[R_Description] [varchar](254) NULL CONSTRAINT [DF_Recipes_R_Description]  DEFAULT (''),
	[R_Modified] [datetime] NULL,
	[R_PrepTime] [int] NULL CONSTRAINT [DF_Recipes_R_PrepTime]  DEFAULT ((0)),
	[R_CookTime] [int] NULL CONSTRAINT [DF_Recipes_R_CookTime]  DEFAULT ((0)),
	[R_Steps] [varchar](max) NULL CONSTRAINT [DF_Recipes_R_Steps]  DEFAULT (''),
	[R_Category] [int] NULL CONSTRAINT [DF_Recipes_R_Category]  DEFAULT ((1)),
 CONSTRAINT [PK_Recipes] PRIMARY KEY CLUSTERED 
(
	[R_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Units]    Script Date: 6/12/2017 9:30:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Units](
	[U_ID] [int] IDENTITY(1,1) NOT NULL,
	[U_Name] [varchar](15) NOT NULL,
	[U_Plural] [varchar](16) NULL,
	[U_Abbrev] [varchar](16) NULL,
	[U_Typecode] [int] NULL CONSTRAINT [DF_Unit_U_Typecode]  DEFAULT ((1)),
	[U_Ratio] [float] NULL CONSTRAINT [DF_Unit_U_Ratio]  DEFAULT ((1.0)),
	[U_System] [nchar](3) NULL CONSTRAINT [DF_Units_U_System]  DEFAULT (N'MET'),
 CONSTRAINT [PK_Units] PRIMARY KEY CLUSTERED 
(
	[U_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[SimpleIngredientsView]    Script Date: 6/20/2017 7:55:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[SimpleIngredientsView]
AS
SELECT dbo.Ingredients.IE_Amount, dbo.Units.U_Abbrev, dbo.Ingredients.IE_Name, dbo.Ingredients.IE_RecipeID, dbo.Units.U_ID
FROM     dbo.Ingredients LEFT OUTER JOIN
                  dbo.Units ON dbo.Ingredients.IE_Unit = dbo.Units.U_ID

GO
/****** Object:  View [dbo].[SimpleRecipeView]    Script Date: 6/20/2017 7:55:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[SimpleRecipeView]
AS
SELECT dbo.Recipes.R_Name, dbo.Recipes.R_Description, dbo.Recipes.R_Modified, dbo.Recipes.R_PrepTime, dbo.Recipes.R_CookTime, dbo.Categories.C_Name, dbo.Recipes.R_ID, 
                  dbo.Images.IMG_FileName
FROM     dbo.Images RIGHT OUTER JOIN
                  dbo.Recipes ON dbo.Images.IMG_RecipeID = dbo.Recipes.R_ID LEFT OUTER JOIN
                  dbo.Categories ON dbo.Recipes.R_Category = dbo.Categories.C_ID

GO
ALTER TABLE [dbo].[Images]  WITH NOCHECK ADD  CONSTRAINT [FK_Images_Recipes] FOREIGN KEY([IMG_RecipeID])
REFERENCES [dbo].[Recipes] ([R_ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Images] CHECK CONSTRAINT [FK_Images_Recipes]
GO
ALTER TABLE [dbo].[Ingredients]  WITH CHECK ADD  CONSTRAINT [FK_Ingredients_Recipes] FOREIGN KEY([IE_RecipeID])
REFERENCES [dbo].[Recipes] ([R_ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ingredients] CHECK CONSTRAINT [FK_Ingredients_Recipes]
GO
ALTER TABLE [dbo].[Ingredients]  WITH CHECK ADD  CONSTRAINT [FK_Ingredients_Units] FOREIGN KEY([IE_Unit])
REFERENCES [dbo].[Units] ([U_ID])
GO
ALTER TABLE [dbo].[Ingredients] CHECK CONSTRAINT [FK_Ingredients_Units]
GO
ALTER TABLE [dbo].[Recipes]  WITH CHECK ADD  CONSTRAINT [FK_Recipes_Categories] FOREIGN KEY([R_Category])
REFERENCES [dbo].[Categories] ([C_ID])
ON UPDATE CASCADE
ON DELETE SET DEFAULT
GO
ALTER TABLE [dbo].[Recipes] CHECK CONSTRAINT [FK_Recipes_Categories]
GO
/****** Object:  StoredProcedure [dbo].[RecipeCountQuery]    Script Date: 6/12/2017 9:30:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RecipeCountQuery]
AS
	SET NOCOUNT ON;
SELECT COUNT(*) FROM Recipes
GO
/****** Object:  StoredProcedure [dbo].[GetLastInsertedRecipeID]    Script Date: 10/26/2017 1:28:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetLastInsertedRecipeID]
AS
	SET NOCOUNT ON;
SELECT IDENT_CURRENT('Recipes')
GO
USE [master]
GO
ALTER DATABASE [Cookbook] SET  READ_WRITE 
GO
/****** Setup Default values in tables ******/
USE [Cookbook]
GO
SET IDENTITY_INSERT [dbo].[Units] ON
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (1, N'kilogram', N'kilograms', N'kg', 1, 1, N'MET')
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (2, N'gram', N'grams', N'g', 1, 0.001, N'MET')
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (3, N'milligram', N'milligrams', N'mg', 1, 1.00E-06, N'MET')
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (4, N'pound', N'pounds', N'lb', 1, 0.453592, N'USC')
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (5, N'ounce', N'ounces', N'oz', 1, 0.0283495, N'USC')
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (6, N'liter', N'liters', N'L', 2, 1, N'MET')
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (7, N'milliliter', N'milliliters', N'mL', 2, 0.001, N'MET')
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (8, N'gallon', N'gallons', N'gal', 2, 3.78541, N'USC')
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (9, N'quart', N'quarts', N'qt', 2, 0.946353, N'USC')
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (10, N'pint', N'pints', N'pt', 2, 0.473176, N'USC')
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (11, N'cup', N'cups', N'C', 2, 0.236588, N'USC')
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (12, N'fluid ounce', N'fluid ounces', N'fl oz', 2, 0.0295735, N'USC')
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (13, N'tablespoon', N'tablespoons', N'T', 2, 0.0147868, N'USC')
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (14, N'teaspoon', N'teaspoons', N't', 2, 0.00492892, N'USC')
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (15, N'dozen', N'dozens', N'doz', 3, 12, N'ALL')
GO
INSERT [dbo].[Units] ([U_ID], [U_Name], [U_Plural], [U_Abbrev], [U_Typecode], [U_Ratio], [U_System]) VALUES (16, N'', N'', N'', 3, 1, N'ALL')
GO
SET IDENTITY_INSERT [dbo].[Units] OFF
GO
SET IDENTITY_INSERT [dbo].[Categories] ON
GO
INSERT INTO [dbo].[Categories] (C_ID, C_Name) VALUES (-1, 'All')
GO
INSERT INTO [dbo].[Categories] (C_ID, C_Name) VALUES (1, 'Default')
GO
SET IDENTITY_INSERT [dbo].[Categories] OFF
