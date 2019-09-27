set identity_insert Customers on

insert into Customers (ID, FName, LName, NIN) 
			  values  (15, N'Ali', N'Zare', '1111111111')
					, (16, N'Amir', N'Zare', '2222222222')
					, (17, N'Sam', N'Zare', '3333333333')
					, (18, N'Mahan', N'Esabat Tabari', '4444444444')
					, (19, N'Shirin', N'Zare', '5555555555')

set identity_insert Customers off








set identity_insert Addresses on

insert into Addresses (ID, CustomerID, Line1) 
			  values  (741952, 15, 'Iran, Tehran, ...')
					, (741953, 16, 'Canada, Toronto, ...')

set identity_insert Addresses off








set identity_insert Orders on

insert into Orders (ID, CustomerID, ReceiveDate, ShippingDate) 
			values (505, 15, p.d('1398/02/07'), p.d('1398/03/24'))
				 , (506, 16, p.d('1398/03/28'),              null)
				 , (507, 19,              null, p.d('1398/04/03'))
				 , (508, 15, p.d('1398/01/16'), p.d('1398/02/09'))
				 , (509, 16, p.d('1398/02/19'), p.d('1398/03/13'))
				 , (510, 19, p.d('1398/03/04'), p.d('1398/04/11'))

set identity_insert Orders off








set identity_insert Shipments on

insert into Shipments(ID, OrderID, PostalCode, Telephone, Cellphone) 
			  values  (53952148, 505, '1234512345', '99999999999', '88888888888')
					, (53952149, 506, '5432154321', '77777777777', '66666666666')

set identity_insert Shipments off








set identity_insert Products on

insert into Products (ID, Name, Manufacturer, Width, Height, Weight, Color, Quality, Kind, Image) 
			  values (6005, N'Laptop Asus N552', N'Asus', 35.6, 9.8, 3.2, 'White', 73, 12, 0x000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F)

set identity_insert Products off








set identity_insert OrderDetails on

insert into OrderDetails (ID, OrderID, ProductID, Amount, Fee) 
			values (987654321, 505, 6005, 17, 1000000)

set identity_insert OrderDetails off