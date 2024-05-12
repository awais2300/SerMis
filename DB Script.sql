create table People
(
Id		int primary key identity not null,
Name		varchar(500),
City		varchar(200),
Age		int,
Flag		int,
updated_date	datetime default(getdate())
)