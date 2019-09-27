create schema p
go

create function p.d(@s char(10))
returns date
as
begin
	return parse(@s as date using 'fa-IR')
end
go