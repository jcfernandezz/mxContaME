-------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.DcemFcnDocNac') IS NOT NULL
   DROP FUNCTION dbo.DcemFcnDocNac
GO

create function [dbo].[DcemFcnDocNac]
(
@ORCTRNUM varchar(21), 
@ORTRXTYP smallint, 
@SOURCDOC VARCHAR(11),
@ORMSTRID varchar(31),
@series smallint
)
returns xml 
as
--Propósito. Facturas de compra o venta nacionales. Cobros.
--02/2015 jmg Creación
--07/04/15 jcf Replanteo de consulta. Debe obtener resultados sólo si es factura PM, POP, SOP
--25/01/18 jcf Agrega Cobros
--	
begin
 declare @cncp xml;
   WITH XMLNAMESPACES
(
    'http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/PolizasPeriodo' as "PLZ"
)

 select @cncp = (
		SELECT
			rtrim(left(docu.MexFolioFiscal, 36)) '@UUID_CFDI',
			cast(docu.docamnt as numeric(16,2))  '@MontoTotal',
			dbo.DcemFcnReplace(docu.txrgnnum)    '@RFC',
			rtrim(docu.ISOCURRC)				 '@Moneda',
			cast(docu.xchgrate as numeric(19,5)) '@TipCamb'
		from dbo.dcemFnGetDocumentoOriginal (@ORCTRNUM, @ORTRXTYP, @SOURCDOC) docu 
		where (
			(@SERIES = 4
			  AND @SOURCDOC in ( 'PMTRX', 'RECVG')
			  and @ORTRXTYP <= 5)
			  or
			(@SERIES = 3
			  AND @SOURCDOC = 'SJ'
			  and @ORTRXTYP <= 7)
			  or
			(@SERIES = 3
			  AND @SOURCDOC = 'CRJ'
			  and @ORTRXTYP = 9)
		  )
		  and upper(docu.ccode) in ('MEX', 'MX', '')
		  and docu.MexFolioFiscal is not null

		FOR XML path('PLZ:CompNal'), type
)
 return @cncp
end
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: DcemFcnDocNac()'
ELSE PRINT 'Error en la creación de la función: DcemFcnDocNac()'
GO

----------------------------------------------------

--sp_columns gl20000

--SELECT
--	CASE @sourcdoc  
--    WHEN 'PMTRX' THEN 
--       rtrim(left(docu2.MexFolioFiscal, 36))
--    WHEN 'RECVG' THEN 
--       rtrim(left(docu3.MexFolioFiscal, 36))
--    END                                  UUIDF_CFDI,      
--    dbo.DcemFcnReplace(pm.txrgnnum)      RFC, 
--    cast(docu.docamnt as numeric(16,2))  MontoTotal ,
--    rtrim(docu.ISOCURRC)				 Moneda ,
--    cast(docu.xchgrate as numeric(19,5)) TipCamb
--from pm00200 pm 
--     outer apply dbo.dcemFnGetDocumentoOriginal (@ORCTRNUM, @ORTRXTYP, @SOURCDOC) docu 
--     outer apply dbo.DcemFcnGetFolioFiscalDeFacturaPM (@ORCTRNUM, @ORTRXTYP) docu2      
--     outer apply dbo.DcemFcnGetFolioFiscalDeFacturaPOP (@ORCTRNUM, @ORTRXTYP) docu3      
--where PM.VENDORID = @ORMSTRID
--  AND @SERIES = 4
--  AND PM.VNDCLSID <> 'EXTRANJERO' 
--  AND @SOURCDOC IN ('RECVG', 'PMTRX')
  
--UNION ALL

--SELECT
--	CASE @sourcdoc  
--    WHEN 'SJ' THEN 
--       dbo.DcemFcnReplace(left(docu3.foliofiscal, 36))
--    END UUIDF_CFDI,      
--    dbo.DcemFcnReplace(pm.txrgnnum)         RFC,
--    cast(docu.docamnt as numeric(16,2))     MontoTotal,
--    rtrim(docu.ISOCURRC)					Moneda,
--    cast(docu.xchgrate as numeric(19,5))    TipCamb
--from rm00101 pm 
--     outer apply dbo.dcemFnGetDocumentoOriginal (@ORCTRNUM, @ORTRXTYP, @SOURCDOC) docu 
--     outer apply dbo.DcemFcnGetFolioFiscalDeFacturaSOP (@ORCTRNUM, @ORTRXTYP) docu3      
--where PM.custnmbr = @ORMSTRID
--  AND @SERIES = 3
--  AND PM.CUSTCLAS <> 'EXTRANJERO' 
--  AND @SOURCDOC IN ('SJ')
