CREATE TABLE [dbo].[SessionHistory]
(
	[sh_session_id] INT PRIMARY KEY,
    [sh_player_id] INT NULL,
    [sh_login] DATETIME NULL,
    [sh_logout] DATETIME NULL
)