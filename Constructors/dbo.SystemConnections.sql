CREATE TABLE [dbo].[SystemConnections]
(
    [sc_system_1_id] INT NOT NULL,
    [sc_system_2_id] INT NOT NULL,
    [sc_travel_cost] INT NULL,
    PRIMARY KEY ([sc_system_1_id], [sc_system_2_id])
)
