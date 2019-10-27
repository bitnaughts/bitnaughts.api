CREATE TABLE [dbo].[Planets]
(
	[p_planet_id] INT PRIMARY KEY, 
    [p_system_id] INT NULL,
    [p_radius] DECIMAL(5,2) NULL,
    [p_offset] DECIMAL(5,2) NULL,
    [p_size] INT NULL,
    [p_density] INT NULL,
    [p_composition] VARCHAR(20) NULL,
    [p_is_habitable] BOOL NULL,
    [p_is_inhabited] BOOL NULL,
    [p_kardashev_level] INT NULL,
    [p_economy_type] VARCHAR(10) NULL,
    [p_seed] INT NULL
)
