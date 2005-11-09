alter table Book  drop constraint FK7C80C0CCEB61E439
drop table Publisher
drop table Book
create table Publisher (
  Id INT IDENTITY NOT NULL,
   Name NVARCHAR(255) null,
   primary key (Id)
)
create table Book (
  Id INT IDENTITY NOT NULL,
   Name NVARCHAR(255) null,
   Author NVARCHAR(255) null,
   publisher_id INT null,
   primary key (Id)
)
alter table Book  add constraint FK7C80C0CCEB61E439 foreign key (publisher_id) references Publisher 
