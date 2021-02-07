USE [C:\USERS\ROBER\SOURCE\REPOS\PERT\WPF\DATABASE\PERT.MDF]
GO
/****** Object:  StoredProcedure [dbo].[TRACKING_VERSION]    Script Date: 2/6/2021 8:00:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Robert Nelson
-- Create date: 1/31/2021
-- Description:	Retrieves the current version from changes to PERT
-- =============================================
CREATE PROCEDURE [dbo].[TRACKING_VERSION]
	@ret_value INT OUTPUT
AS
BEGIN
DECLARE @CHANGE_TRACKING_CURRENT_VERSION BIGINT;

EXEC PERT.sys.sp_executesql
  N'SELECT @ReturnValue = CHANGE_TRACKING_CURRENT_VERSION()',
  N'@ReturnValue BIGINT OUTPUT',
  @ReturnValue = @ret_value OUTPUT

END

GO
