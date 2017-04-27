IF OBJECT_ID ('dbo.DcemFcnPolizas') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnPolizas]
GO

create function [dbo].[DcemFcnPolizas](@periodid SMALLINT, @year1 SMALLINT)
returns xml 
as
--Propósito. Polizas
--12/2014 jmg Creación
--7/4/15 jcf Replanteo de consulta
--
begin
 declare @cncp xml;
   --
 WITH XMLNAMESPACES
(
    'http://www.w3.org/2001/XMLSchema-instance' as "xsi",
    'www.sat.gob.mx/esquemas/ContabilidadE/1_1/PolizasPeriodo' as "PLZ"
)
 --
 select @cncp = (
		SELECT
			'www.sat.gob.mx/esquemas/ContabilidadE/1_1/PolizasPeriodo http://www.sat.gob.mx/esquemas/ContabilidadE/1_1/PolizasPeriodo/PolizasPeriodo_1_1.xsd' as '@xsi:schemaLocation',
			'1.1'												'@Version',
			ltrim(rtrim(replace(cia.TAXREGTN, 'RFC ', '')))		'@RFC',
			right( '00' + cast( @periodid AS varchar(2)), 2 ) 	'@Mes',
			@YEAR1												'@Anio',
			'AF'												'@TipoSolicitud',
			dbo.dcemfcnpoliza(@YEAR1, @periodid)
		FROM DYNAMICS..SY01500 cia
		where cia.INTERID = DB_NAME()
		FOR XML path('PLZ:Polizas'), type
)
 return @cncp
end

go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: DcemFcnPolizas()'
ELSE PRINT 'Error en la creación de la función: DcemFcnPolizas()'
GO
