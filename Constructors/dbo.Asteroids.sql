CREATE TABLE [dbo].[Asteroids]
(
	[a_asteroid_id] INT PRIMARY KEY,
    [a_system_id] INT NULL,
    [a_radius] DECIMAL(5,2) NULL,
    [a_offset] DECIMAL(5,2) NULL,
    [a_size] INT NULL,
    [a_density] INT NULL,
    [a_composition] VARCHAR(20) NULL,
    [a_is_mineable] BOOLEAN NULL,
    [a_is_regenerating] BOOLEAN NULL,
    [a_seed] INT NULL
)
