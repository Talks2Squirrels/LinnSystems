USE [LinnSystemsConsignment]
GO
/****** Object:  User [LinnSystems]    Script Date: 07/12/2016 10:27:58 ******/
CREATE USER [LinnSystems] FOR LOGIN [LinnSystems] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [LinnSystems]
GO
/****** Object:  UserDefinedTableType [dbo].[ItemTable]    Script Date: 07/12/2016 10:27:58 ******/
CREATE TYPE [dbo].[ItemTable] AS TABLE(
	[TempPackageID] [int] NOT NULL,
	[ItemCode] [nvarchar](50) NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitWeight] [decimal](18, 2) NOT NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[PackageTable]    Script Date: 07/12/2016 10:27:58 ******/
CREATE TYPE [dbo].[PackageTable] AS TABLE(
	[TempPackageID] [int] NOT NULL,
	[PackageWidth] [decimal](18, 2) NOT NULL,
	[PackageHeight] [decimal](18, 2) NOT NULL,
	[PackageLength] [decimal](18, 2) NOT NULL,
	[PackageType] [nvarchar](50) NULL,
	PRIMARY KEY CLUSTERED 
(
	[TempPackageID] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[consignment]    Script Date: 07/12/2016 10:27:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[consignment](
	[ConsignmentId] [int] IDENTITY(1,1) NOT NULL,
	[ConsignmentDate] [smalldatetime] NOT NULL,
	[Address1] [nvarchar](max) NOT NULL,
	[Address2] [nvarchar](max) NULL,
	[Address3] [nvarchar](max) NULL,
	[City] [nvarchar](150) NOT NULL,
	[PhoneNumber] [nvarchar](30) NULL,
	[CountryISO2] [nvarchar](3) NOT NULL,
	[PostCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_consignment] PRIMARY KEY CLUSTERED 
(
	[ConsignmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[item]    Script Date: 07/12/2016 10:27:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[item](
	[ItemId] [int] IDENTITY(1,1) NOT NULL,
	[PackageId] [int] NOT NULL,
	[ItemCode] [nvarchar](50) NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitWeight] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_item] PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[package]    Script Date: 07/12/2016 10:27:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[package](
	[PackageId] [int] IDENTITY(1,1) NOT NULL,
	[ConsignmentId] [int] NOT NULL,
	[PackageWidth] [decimal](18, 2) NOT NULL,
	[PackageHeight] [decimal](18, 2) NOT NULL,
	[PackageDepth] [decimal](18, 2) NOT NULL,
	[PackageType] [nvarchar](50) NULL,
 CONSTRAINT [PK_package] PRIMARY KEY CLUSTERED 
(
	[PackageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[item]  WITH CHECK ADD  CONSTRAINT [FK_item_package] FOREIGN KEY([PackageId])
REFERENCES [dbo].[package] ([PackageId])
GO
ALTER TABLE [dbo].[item] CHECK CONSTRAINT [FK_item_package]
GO
ALTER TABLE [dbo].[package]  WITH CHECK ADD  CONSTRAINT [FK_package_consignment] FOREIGN KEY([ConsignmentId])
REFERENCES [dbo].[consignment] ([ConsignmentId])
GO
ALTER TABLE [dbo].[package] CHECK CONSTRAINT [FK_package_consignment]
GO
/****** Object:  StoredProcedure [dbo].[AddConsignment]    Script Date: 07/12/2016 10:27:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[AddConsignment] 
	-- Add the parameters for the stored procedure here
	@address1 nvarchar(MAX),
	@address2 nvarchar(MAX),
	@address3 nvarchar(MAX),
	@city nvarchar(150),
	@phoneNumber nvarchar(30),
	@countryCode nvarchar(3),
	@postCode nvarchar(50),
	@consignmentDate smalldatetime,
	@packageList AS PackageTable READONLY,
	@itemList AS ItemTable READONLY
AS
BEGIN
	DECLARE @consignmentID int

	--package vars
	DECLARE @TempPackageID int
	DECLARE @PackageId int
	DECLARE @PackageWidth decimal(18,2)
	DECLARE @PackageHeight decimal(18,2)
	DECLARE @PackageDepth decimal(18,2)
	DECLARE @PackageType nvarchar(50)

	--item Vars
	DECLARE @ItemTempPackageID int
	DECLARE @ItemCode nvarchar(50)
	DECLARE @Quantity int
	DECLARE @UnitWeight decimal(18,2)

	--create the assignment based on the details sent in
	INSERT INTO consignment (Address1, Address2, Address3, City, PostCode, CountryISO2, ConsignmentDate, PhoneNumber)
	VALUES(@address1, @address2, @address3, @city, @postCode, @countryCode, @consignmentDate, @phoneNumber)

	SET @consignmentID = SCOPE_IDENTITY()

	--loop the package rows in the table and create those retrieve ID from db
	DECLARE packageCursor CURSOR READ_ONLY
	FOR
	SELECT *
	FROM @packageList

	OPEN packageCursor

	FETCH NEXT FROM packageCursor INTO
	@TempPackageID, @PackageWidth, @PackageHeight, @PackageDepth, @PackageType

	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO package (ConsignmentId, PackageWidth, PackageHeight, PackageDepth, PackageType)
		VALUES(@consignmentID, @PackageWidth, @PackageHeight, @PackageDepth, @PackageType)

		--get the new packageID created from the insert
		SET @PackageId = SCOPE_IDENTITY()

		--loop the item rows in the item table based on when the @tempPackageID is the same
		DECLARE itemCursor CURSOR READ_ONLY
		FOR SELECT *
		FROM @itemList
		WHERE TempPackageID = @TempPackageID

		
		--for each package row in the table loop the item list where the packageid is the same
		OPEN itemCursor

		FETCH NEXT FROM itemCursor INTO
		@ItemTempPackageID, @ItemCode, @Quantity, @UnitWeight

		WHILE @@FETCH_STATUS = 0
		BEGIN
			--IF @ItemTempPackageID = @TempPackageID
			--BEGIN
			/*SELECT @ItemCode = ItemCode, @Quantity = Quantity, @UnitWeight = UnitWeight
			FROM @itemList
			WHERE TempPackageID = @TempPackageID*/

			/*SELECT @ItemCode = ItemCode, @Quantity = Quantity, @UnitWeight = UnitWeight
			FROM @itemList*/

			INSERT INTO item (PackageId, ItemCode, Quantity, UnitWeight)
			VALUES (@PackageId, @ItemCode, @Quantity, @UnitWeight)
			--END

			FETCH NEXT FROM itemCursor
			INTO @ItemTempPackageID, @ItemCode, @Quantity, @UnitWeight
		END

		CLOSE itemCursor
		DEALLOCATE itemCursor

		FETCH NEXT FROM packageCursor
		INTO @TempPackageID, @PackageWidth, @PackageHeight, @PackageDepth, @PackageType
	END

	CLOSE packageCursor
	DEALLOCATE packageCursor
END

GO
/****** Object:  StoredProcedure [dbo].[GetConsignment]    Script Date: 07/12/2016 10:27:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetConsignment]
	-- Add the parameters for the stored procedure here
	@consignmentID int
AS
BEGIN
	SELECT *
	FROM consignment
	WHERE ConsignmentId = @consignmentID

	SELECT PackageId, ConsignmentId, PackageWidth, PackageHeight, PackageDepth, PackageType, (SELECT SUM(UnitWeight) FROM item WHERE PackageId = p.PackageId) AS TotalWeight
	FROM package p
	WHERE ConsignmentId = @consignmentID
	ORDER BY TotalWeight DESC

	SELECT ItemId, i.PackageId, ItemCode, Quantity, UnitWeight
	FROM item i
	INNER JOIN package p ON p.PackageId = i.PackageId
	WHERE p.ConsignmentId = @consignmentID
END

GO
