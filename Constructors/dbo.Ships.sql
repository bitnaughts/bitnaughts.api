CREATE TABLE [dbo].[Ships]
(
	[sp_ship_id] INT PRIMARY KEY,
    [sp_player_id] INT NULL,
    [sp_name] VARCHAR(50) NULL,
    [sp_health] INT NULL,
    [sp_position_x] DECIMAL(5,2) NULL,
    [sp_position_y] DECIMAL(5,2) NULL
)