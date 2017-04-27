IF OBJECT_ID ('dbo.DcemFcnAuxiliarFolios') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnAuxiliarFolios]
GO

create function [dbo].[DcemFcnAuxiliarFolios](@periodid SMALLINT, @year1 SMALLINT)
returns xml 
as
--Propósito. Nodo auxiliar de Folios
--02/2015 jmg Creación
--22/4/15 jcf Replanteo de consulta
--
begin
 declare @cncp xml;
 WITH XMLNAMESPACES
(
    'http://www.w3.org/2001/XMLSchema-instance' as "xsi",
     'www.sat.gob.mx/esquemas/ContabilidadE/1_1/AuxiliarFolios' as "RepAux"    
 )     
select @cncp = 
(
SELECT
	'www.sat.gob.mx/esquemas/ContabilidadE/1_1/AuxiliarFolios http://www.sat.gob.mx/esquemas/ContabilidadE/1_1/AuxiliarFolios/AuxiliarFolios_1_2.xsd' '@xsi:schemaLocation',   
	'1.2'																	'@Version',
	ltrim(rtrim( replace(TAXREGTN, 'RFC ', '' )))							'@RFC',
	right( '00' + cast( @periodid AS varchar(2)), 2 )						'@Mes',
	@YEAR1																	'@Anio',
	'AF'																	'@TipoSolicitud',
	dbo.DcemFcnDetFolios(@YEAR1, @periodid)
FROM DYNAMICS..SY01500 
where INTERID = DB_NAME()
FOR XML path('RepAux:RepAuxFol'), type
)
return @cncp
end

go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: DcemFcnAuxiliarFolios()'
ELSE PRINT 'Error en la creación de la función: DcemFcnAuxiliarFolios()'
GO
-----------------------------------------------------------------------
--select dbo.[DcemFcnAuxiliarFolios](1, 2015)
