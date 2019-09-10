INSERT INTO Countries
VALUES
	('Spain', 'SPA'),
	('United States of America', 'USA'),
	('United Kingdom', 'UK'),
	('France', 'FRA'),
	('Germany', 'GER'),
	('Portugal', 'PRT')
INSERT INTO PublishingHouses
VALUES
	('Penguin Random House', 3),
	('Edebe', 1),
	('Portuguesse Books', 6),
	('Marvel Comics', 2),
	('Baguette', 4),
	('Angela Merkel & Co.', 5)
INSERT INTO Categories
VALUES
	('Action', 'Punch pim pam'),
	('Fantasy', 'Magic harry potter'),
	('Terror', 'HP Lovecraft was here'),
	('Historic', 'Guess that i have to get a time machine')
INSERT INTO Authors
VALUES
	('Rowling', 1, convert(datetime, '1/1/1999', 103)),
	('Lovecraft', 5, convert(datetime, '1/1/2000', 103)),
	('Terry Pratchett', 3, convert(datetime, '1/1/1958', 103)),
	('Carlos Ruiz Zafón', 1, convert(datetime, '1/1/2005', 103)),
	('Stan Lee', 2, convert(datetime, '1/1/1930', 103))
INSERT INTO Comics
VALUES
	('Colour of Magic', 'Rincewind adventures I', convert(datetime, '5/6/1978', 103), 208, 2, 3),
	('Fantastic Light', 'Rincewind adventures II', convert(datetime, '4/2/1979', 103), 241, 2, 3),
	('La sombra del viento', 'Una novela que no me acuerdo', convert(datetime, '1/9/1985', 103), 203, 4, 2),
	('Iron-Man', 'The man of iron', convert(datetime, '5/8/1987', 103), 55, 1, 4),
	('Call of Cthulhu', 'You cannot sleep', convert(datetime, '5/2/1872', 103), 58, 3, 5)
INSERT INTO Functions
VALUES
	('Creator', 1, 3),
	('Creator', 2, 3),
	('Creator', 5, 2),
	('Creator', 3, 4),
	('Creator', 4, 5),
	('Creator', 4, 1),
	('Drawer', 4, 5),
	('Writer', 1, 3),
	('Inspirator', 1, 2)