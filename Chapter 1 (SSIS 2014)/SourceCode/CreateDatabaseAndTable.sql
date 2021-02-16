CREATE DATABASE [Apress_SSIS_Scripting]
 CONTAINMENT = NONE
USE [Apress_SSIS_Scripting]
GO
CREATE TABLE [dbo].[Customer](
	[CustomerKey] [varchar](50) NOT NULL,
	[GeographyKey] [varchar](50) NULL,
	[CustomerAlternateKey] [varchar](50) NULL,
	[Title] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[MiddleName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[NameStyle] [varchar](50) NULL,
	[BirthDate] [varchar](50) NULL,
	[MaritalStatus] [varchar](50) NULL,
	[Suffix] [varchar](50) NULL,
	[Gender] [varchar](50) NULL,
	[EmailAddress] [varchar](50) NULL,
	[YearlyIncome] [varchar](50) NULL,
	[TotalChildren] [varchar](50) NULL,
	[NumberChildrenAtHome] [varchar](50) NULL,
	[EnglishEducation] [varchar](50) NULL,
	[SpanishEducation] [varchar](50) NULL,
	[FrenchEducation] [varchar](50) NULL,
	[EnglishOccupation] [varchar](50) NULL,
	[SpanishOccupation] [varchar](50) NULL,
	[FrenchOccupation] [varchar](50) NULL,
	[HouseOwnerFlag] [varchar](50) NULL,
	[NumberCarsOwned] [varchar](50) NULL,
	[AddressLine1] [varchar](50) NULL,
	[AddressLine2] [varchar](50) NULL,
	[Phone] [varchar](50) NULL,
	[DateFirstPurchase] [varchar](50) NULL,
	[CommuteDistance] [varchar](50) NULL,
	[SourceFile] [varchar](50) NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[CustomerKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
