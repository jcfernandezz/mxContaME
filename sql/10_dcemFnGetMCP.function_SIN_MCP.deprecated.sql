-------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.dcemFnGetMcp') IS NOT NULL
   DROP FUNCTION dbo.dcemFnGetMcp
GO

create function dbo.dcemFnGetMcp(@VCHRNMBR varchar(21), @bchsourc char(15))
returns table
as
--Propósito. En caso de no instalar mcp, debe devolver nulo
--Requisitos. 
--26/12/14 jcf Creación 
--19/02/15 jcf Agrega amounto
--26/8/15 jcf Corrige el tipo de dato
--
return
( 	
	--MTP y GAP no tienen mcp
	select null MCPTYPID, cast(null as varchar(21)) NUMBERIE, cast(null as varchar(21)) MEDIOID, null LNSEQNBR, cast(null as varchar(21)) grupid, cast(null as varchar(15)) chekbkid, 
		cast(null as varchar(21)) docnumbr, 
		cast(null as varchar(31)) nomBancoExt, 
		cast(null as varchar(21)) bancoOrigenSat, cast(null as varchar(15)) ctaOrigenSat, cast(null as datetime) emidate, null LINEAMNT, cast(null as numeric(21,5)) amounto, 
		cast(null as varchar(65)) beneficiarioSat

	--Habilitar la otra función en Getty, Maclean
	-------------------------

)
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: fnAndinaGetMcp()'
ELSE PRINT 'Error en la creación de la función: fnAndinaGetMcp()'
GO

