use Dairy_Collector;
CREATE TABLE [dbo].[tblDWebTemp](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[CCode] [int] NOT NULL,
	[FarmerCode] [varchar](20) NOT NULL,
	[OldCode] [varchar](20) NOT NULL,
	[Parent] [varchar](20) NOT NULL,
	[DateOf] [varchar](40) NOT NULL,
	[TimeOf] [varchar](10) NOT NULL,
	[SNF] [decimal](5, 1) NOT NULL,
	[FAT] [decimal](5, 1) NOT NULL,
	[Qty] [decimal](10, 3) NOT NULL,
	[SavedTime] [date] default CAST(GETDATE() AS DATE)
);