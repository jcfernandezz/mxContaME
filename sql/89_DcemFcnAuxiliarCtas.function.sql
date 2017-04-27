
IF OBJECT_ID ('dbo.DcemFcnAuxiliarCtas') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnAuxiliarCtas]
GO

create function [dbo].[DcemFcnAuxiliarCtas](@periodid SMALLINT, @year1 SMALLINT)
returns xml 
as
--Propósito. Nodo auxiliar de cuentas
--02/2015 jmg Creación
--9/4/15 jcf Replanteo de consulta
--
begin
 declare @cncp xml;
   --
 WITH XMLNAMESPACES
(
    'http://www.w3.org/2001/XMLSchema-instance' as "xsi",
    'www.sat.gob.mx/esquemas/ContabilidadE/1_1/AuxiliarCtas' as "auxiliarctas"

)
 --
 select @cncp = (
	SELECT
		'www.sat.gob.mx/esquemas/ContabilidadE/1_1/AuxiliarCtas http://www.sat.gob.mx/esquemas/ContabilidadE/1_1/AuxiliarCtas/AuxiliarCtas_1_1.xsd' '@xsi:schemaLocation',
		'1.1'												'@Version',
		 ltrim(rtrim( replace(cia.TAXREGTN, 'RFC ', '' )))	'@RFC',
		 right( '00' + cast( @periodid AS varchar(2)), 2 )	'@Mes',
		 dbo.DcemFcnReplace(@YEAR1)							'@Anio',
		'AF'												'@TipoSolicitud',
		dbo.dcemfcnctasblz(@periodid, @YEAR1)
	FROM DYNAMICS..SY01500 cia
	where cia.INTERID = DB_NAME()
	FOR XML path('auxiliarctas:AuxiliarCtas'), type
)
 return @cncp
end
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: [DcemFcnAuxiliarCtas]()'
ELSE PRINT 'Error en la creación de la función: [DcemFcnAuxiliarCtas]()'
GO
