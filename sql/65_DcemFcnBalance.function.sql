IF OBJECT_ID ('dbo.DcemFcnBalance') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnBalance]
GO

create function [dbo].[DcemFcnBalance](@periodid SMALLINT, @year1 SMALLINT)
returns xml 
as
--Propósito. Balance de cuentas para el SAT
--12/2014 jmg Creación
--7/4/15 jcf Corrección de llamada a dcemfcnctas y replanteo de consulta
--
begin
 declare @cncp xml;
  --
 WITH XMLNAMESPACES
(
    'http://www.w3.org/2001/XMLSchema-instance' as "xsi",
    'www.sat.gob.mx/esquemas/ContabilidadE/1_1/BalanzaComprobacion' as "BCE"
)
 --
  select @cncp = (
 
	SELECT
		'www.sat.gob.mx/esquemas/ContabilidadE/1_1/BalanzaComprobacion http://www.sat.gob.mx/esquemas/ContabilidadE/1_1/BalanzaComprobacion/BalanzaComprobacion_1_1.xsd' as '@xsi:schemaLocation',
		'1.1'												'@Version',
		ltrim(rtrim(replace(cia.TAXREGTN, 'RFC ', '')))		'@RFC',
		right( '00' + cast( @periodid AS varchar(2)), 2 )	'@Mes',
		@YEAR1 												'@Anio',
		'N'													'@TipoEnvio',
		dbo.dcemfcnctas(@periodid, @YEAR1)
	FROM DYNAMICS..SY01500 cia
	where cia.INTERID = DB_NAME()
	FOR XML path('BCE:Balanza'), type
)
 return @cncp
end

go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: DcemFcnBalance()'
ELSE PRINT 'Error en la creación de la función: DcemFcnBalance()'
GO

