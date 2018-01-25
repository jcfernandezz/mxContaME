
IF OBJECT_ID ('dbo.DcemFcnCatalogoXML') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnCatalogoXML]
GO

create function [dbo].[DcemFcnCatalogoXML](@periodid SMALLINT, @year1 SMALLINT)
returns xml 
as
--Propósito. Catalogo de cuentas, corporativas y SAT
--12/2014 jmg Creación
--07/04/15 jcf Ajustes varios
--25/01/18 jcf Modificaciones para versión 1.3
--
begin
 declare @cncp xml;
  --
 WITH XMLNAMESPACES
(
    'http://www.w3.org/2001/XMLSchema-instance' as "xsi",
    'http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/CatalogoCuentas' as "catalogocuentas"
)
 --
 select @cncp = (
	SELECT
		'http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/CatalogoCuentas http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/CatalogoCuentas/CatalogoCuentas_1_3.xsd' '@xsi:schemaLocation',
		'1.3'												'@Version',
		ltrim(rtrim(replace(cia.TAXREGTN, 'RFC ', '')))		'@RFC',
		right( '00' + cast( @periodid AS varchar(2)), 2 )	'@Mes',
		@YEAR1												'@Anio',
		dbo.DcemFcnCatalogoCtasXML()
	FROM DYNAMICS..SY01500 cia
	where cia.INTERID = DB_NAME()
	FOR XML path('catalogocuentas:Catalogo'), type

)
 return @cncp
end


go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: DcemFcnCatalogoXML()'
ELSE PRINT 'Error en la creación de la función: DcemFcnCatalogoXML()'
GO

------------------------------------------------------------------------
--select [dbo].[DcemFcnCatalogoXML](1, 2015)
