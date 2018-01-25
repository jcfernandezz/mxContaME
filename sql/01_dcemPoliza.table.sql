
IF DATABASE_PRINCIPAL_ID('rol_contaelectr') IS NULL
	create role rol_contaelectr;
go

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'dcem')
BEGIN
    EXEC( 'create schema dcem authorization rol_contaelectr;' );
END
go

IF OBJECT_ID('dcem.dcemPoliza', 'U') IS NULL
begin
	create table dcem.dcemPoliza (
	JRNENTRY	int	not null ,
	TRXDATE	datetime not null ,
	REFRENCE char(31) null,
	nodoTransaccion xml null,
	err int not null default 0
	PRIMARY KEY (JRNENTRY, TRXDATE)
	);
end
else
	print 'La tabla ya existe'

go

IF (@@Error = 0) PRINT 'Creación exitosa de: dcem.dcemPoliza'
ELSE PRINT 'Error en la creación de: dcem.dcemPoliza'
GO

------------------------------------------------------------------------------

--alter table dcem.dcemPoliza add err int not null default 0;

