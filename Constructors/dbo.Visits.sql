CREATE TABLE [dbo].[Visits]
(
	[v_ship_id] INT NOT NULL, 
    [v_planet_id] INT NOT NULL,
    [v_date] DATETIME NOT NULL,
    PRIMARY KEY ([v_ship_id], [v_planet_id], [v_date])
)
