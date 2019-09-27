create table dbo.Customers
(
	ID int not null identity(1, 1),
	FName nvarchar(40),
	LName nvarchar(40),
	NIN char(10),
	FirstPartnerID int,
	SecondPartnerID int,
	RV rowversion,

	constraint PK_Customer primary key (ID),
	constraint Fk_Customer_Customer1 foreign key (FirstPartnerID) references Customers(ID),
	constraint Fk_Customer_Customer2 foreign key (SecondPartnerID) references Customers(ID),
)
go




create table dbo.Addresses
(
	ID int not null identity(1, 1),
	CustomerID int not null,
	Line1 nvarchar(250), 
	Line2 nvarchar(250), 
	RV rowversion,

	constraint PK_Address primary key (ID),
	constraint Fk_Address_Customer foreign key (CustomerID) references Customers(ID),
	constraint UK_Address_Customer unique (CustomerID)
)
go




create table dbo.Orders
(
	ID bigint not null identity(1, 1),
	CustomerID int,
	ReceiveDate date, 
	ShippingDate date, 
	RV rowversion,

	constraint PK_Order primary key (ID),
	constraint Fk_Order_Customer foreign key (CustomerID) references Customers(ID),
)
go




create table dbo.Shipments
(
	ID bigint not null identity(1, 1),
	OrderID bigint,
	PostalCode char(10), 
	Telephone char(11), 
	Cellphone char(11), 
	RV rowversion,

	constraint PK_Shipment primary key (ID),
	constraint Fk_Shipment_Order foreign key (OrderID) references Orders(ID),
	constraint UK_Shipment_Order unique (OrderID)
)
go




create table dbo.Products
(
    ID smallint not null identity(1, 1),
    Name nvarchar(70),
    Manufacturer nvarchar(70),
    Length real,
    Width real,
    Height real,
    Weight real,
    Color varchar(40),
    Power smallint,
    EnergyEfficiency nvarchar(100),
    Material nvarchar(100),
    Quality smallint,
    Kind smallint,
    Image varbinary(1000),
    RV RowVersion,

	constraint PK_Product primary key (ID),
)
go   




create table dbo.OrderDetails
(
    ID bigint not null identity(1, 1),
    OrderID bigint not null,
    ProductID smallint,
    Amount float,
    Fee bigint,
    RV RowVersion,

	constraint PK_OrderDetail primary key (ID),
	constraint Fk_OrderDetail_Order foreign key (OrderID) references Orders(ID),
	constraint Fk_OrderDetail_Product foreign key (ProductID) references Products(ID),
)
go