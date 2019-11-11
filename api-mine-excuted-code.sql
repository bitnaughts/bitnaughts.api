--api/mine?asteroid=11&ship=0&amount=20

-- Mining alway part of an asteroid
UPDATE dbo.Asteroids SET a_size = 6.59 WHERE a_asteroid_id = 11;
UPDATE dbo.Ships SET sp_data = 1.00 WHERE sp_ship_id = 0;

-- Mining an asteroid completely
DELETE FROM dbo.Asteroids WHERE a_asteroid_id = 12
UPDATE dbo.Ships SET sp_data = 0.20 WHERE sp_ship_id = 0