-------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.DcemFcnDocExtFolio') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnDocExtFolio]
GO

create function [dbo].[DcemFcnDocExtFolio](@ORCTRNUM varchar(21), @ORTRXTYP smallint, @SOURCDOC VARCHAR(11), @SERIES smallint)
returns xml 
as
--Propósito. Documetos Extranjero
--02/2015 jmg Creación
--08/04/15 jcf Replanteo de consulta y parámetros
--29/01/18 jcf Cambia atributos para v1.3
--
begin
 declare @cncp xml;
 WITH XMLNAMESPACES
  (    'http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/AuxiliarFolios' as "RepAux"    )
 select @cncp = (
 
 		SELECT
			rtrim(ltrim(do.docnumbr))					'@NumFactExt', 
			dbo.DcemFcnReplace(rtrim(ltrim(left(do.txrgnnum, 30)))) '@TaxID',
			cast(do.docamnt as numeric(16,2))		'@MontoTotal',
			rtrim(do.codMetodoPago)						'@MetPagoAux',
			rtrim(do.ISOCURRC)						'@Moneda' ,
			cast(do.xchgrate as numeric(19,5))		'@TipCamb' 
		from dbo.dcemFnGetDocumentoOriginal(@ORCTRNUM, @ORTRXTYP, @SOURCDOC) do
		WHERE upper(do.ccode) not in ('MEX', 'MX', '')
			AND @SERIES in (3, 4)
		FOR XML path('RepAux:ComprExt'), type
)
 return @cncp
end

go


IF (@@Error = 0) PRINT 'Creación exitosa de la función: [DcemFcnDocExtFolio]()'
ELSE PRINT 'Error en la creación de la función: [DcemFcnDocExtFolio]()'
GO

