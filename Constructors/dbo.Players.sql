CREATE TABLE [dbo].[Players]
(
	[py_player_id] INT PRIMARY KEY,
    [py_current_session] INT NULL,
    [py_name] VARCHAR(50) NULL,
    [py_password] VARCHAR(50) NULL,
    [py_balance] INT NULL
)