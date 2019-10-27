CREATE TABLE [dbo].[Mines]
(
	[m_ship_id] INT NOT NULL,
    [m_asteroid_id] INT NOT NULL,
    [m_amount] INT NOT NULL,
    [m_date] DATETIME NOT NULL,
    PRIMARY KEY ([m_ship_id], [m_asteroid_id], [m_date])
)
