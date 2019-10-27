CREATE TABLE [dbo].[CombatHistory]
(
	[ch_combat_id] INT PRIMARY KEY,
    [ch_ship_1_id] INT NULL,
    [ch_ship_2_id] INT NULL,
    [ch_date] DATETIME NULL
)