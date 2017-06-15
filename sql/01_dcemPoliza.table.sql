
create schema dcem authorization rol_contaelectr;
go

IF OBJECT_ID('dcem.dcemPoliza', 'U') IS NOT NULL
  DROP TABLE dcem.dcemPoliza;
GO

create table dcem.dcemPoliza (
JRNENTRY	int	not null ,
TRXDATE	datetime not null ,
REFRENCE char(31) null,
nodoTransaccion xml null,
err int not null default 0
PRIMARY KEY (JRNENTRY, TRXDATE)
);
go

IF (@@Error = 0) PRINT 'Creación exitosa de: dcem.dcemPoliza'
ELSE PRINT 'Error en la creación de: dcem.dcemPoliza'
GO

------------------------------------------------------------------------------

--alter table dcem.dcemPoliza add err int not null default 0;


