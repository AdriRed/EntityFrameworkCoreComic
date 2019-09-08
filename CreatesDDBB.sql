INSERT INTO Categories
VALUES
	('Action', 'Punch pim pam'),
	('Fantasy', 'Magic harry potter'),
	('Terror', 'HP Lovecraft was here'),
	('Historic', 'Guess that i have to get a time machine')
INSERT INTO Authors
VALUES
	('Rowling', 'Spain', convert(datetime, '1/1/1999', 103)),
	('Lovecraft', 'Austria', convert(datetime, '1/1/2000', 103)),
	('Terry Pratchett', 'England', convert(datetime, '1/1/1958', 103)),
	('Carlos Ruiz Zafón', 'Spain', convert(datetime, '1/1/2005', 103)),
	('Stan Lee', 'USA', convert(datetime, '1/1/1930', 103))
INSERT INTO Comics
VALUES
	('Colour of Magic', 'Rincewind adventures I', convert(datetime, '5/6/1978', 103), 208, 2),
	('Fantastic Light', 'Rincewind adventures II', convert(datetime, '4/2/1979', 103), 241, 2),
	('La sombra del viento', 'Una novela que no me acuerdo', convert(datetime, '1/9/1985', 103), 203, 4),
	('Iron-Man', 'The man of iron', convert(datetime, '5/8/1987', 103), 55, 1),
	('Call of Cthulhu', 'You cannot sleep', convert(datetime, '5/2/1872', 103), 58, 3)
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