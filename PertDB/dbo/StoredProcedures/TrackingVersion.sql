-- =============================================
-- Author:		Robert Nelson
-- Create date: 1/31/2021
-- Description:	Retrieves the current version from changes to Pert
-- =============================================

CREATE PROCEDURE [dbo].[TrackingVersion]
	@ret_value INT OUTPUT
AS
BEGIN
DECLARE @CHANGE_TRACKING_CURRENT_VERSION BIGINT;
EXEC Pert.sys.sp_executesql
  N'SELECT @ReturnValue = CHANGE_TRACKING_CURRENT_VERSION()',
  N'@ReturnValue BIGINT OUTPUT',
  @ReturnValue = @ret_value OUTPUT

END