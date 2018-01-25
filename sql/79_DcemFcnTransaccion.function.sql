IF OBJECT_ID ('dbo.DcemFcnTransaccion') IS NOT NULL
   DROP FUNCTION dbo.DcemFcnTransaccion
GO

create function [dbo].[DcemFcnTransaccion](@year1 SMALLINT, @periodid SMALLINT, @cuenta INTEGER, @JRNENTRY INTEGER, @trxdate datetime)
returns xml 
as
--Propósito. Obtiene nodo Transaccion
--12/2014 jmg Creación
--8/4/15 jcf Modifica parámetros de funciones
--13/6/17 jcf Corrige filtro. No es necesario @cuenta ni @dex_row_id
--
begin
 declare @cncp xml;
  WITH XMLNAMESPACES
(
    'www.sat.gob.mx/esquemas/ContabilidadE/1_1/PolizasPeriodo' as "PLZ"
)

 select @cncp = (
	 SELECT
		left(rtrim(VW.actnumst), 100)          '@NumCta',
		dbo.DcemFcnReplace(rtrim(VW.actdescr)) '@DesCta',
		case when VW.DSCRIPTN = '' then '-' else dbo.DcemFcnReplace(rtrim(VW.DSCRIPTN)) end '@Concepto',
		cast(VW.DEBITAMT as numeric(16,2))    '@Debe',
		cast(VW.CRDTAMNT as numeric(16,2))    '@Haber',
		DBO.DcemFcnDocNac(ORCTRNUM, ORTRXTYP, SOURCDOC, ORMSTRID, SERIES),
		dbo.DcemFcnDocExt(ORCTRNUM, ORTRXTYP, SOURCDOC, SERIES),
		DBO.DcemFcnCheque(ORCTRNUM, ORTRXTYP, SOURCDOC),
		DBO.DcemFcnTransferencia(ORCTRNUM, ORTRXTYP, SOURCDOC),
		DBO.DcemFcnOtro(ORCTRNUM, ORTRXTYP, SOURCDOC)
	from dcemvwtransaccion VW
	where --VW.ACTINDX        = @cuenta
	  VW.JRNENTRY    = @JRNENTRY
	  and VW.trxdate = @trxdate
	  --AND VW.dex_row_id     = @dex_row_id  
FOR XML path('PLZ:Transaccion'), type
)
 return @cncp
end
go
IF (@@Error = 0) PRINT 'Creación exitosa de la función: DcemFcnTransaccion()'
ELSE PRINT 'Error en la creación de la función: DcemFcnTransaccion()'
GO
