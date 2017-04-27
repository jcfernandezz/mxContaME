IF OBJECT_ID ('dbo.DcemFcnDetFolios') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnDetFolios]
GO

create function [dbo].[DcemFcnDetFolios](@year1 SMALLINT, @periodid SMALLINT)
returns xml 
--Propósito. Nodo Detalle auxiliar de folios
--02/2015 jmg Creación
--8/4/15 jcf Ajuste de llamada a funciones
as
begin
 declare @cncp xml;
 WITH XMLNAMESPACES
(
'www.sat.gob.mx/esquemas/ContabilidadE/1_1/AuxiliarFolios' as "RepAux"    
)
select @cncp = 
(
	select 
		jrnentry				'@NumUnIdenPol',
		cast(trxdate as date)	'@Fecha',
		DBO.DcemFcnDocNacFolio(ORCTRNUM, ORTRXTYP, SOURCDOC, ORMSTRID, SERIES),
		DBO.DcemFcnDocExtFolio(ORCTRNUM, ORTRXTYP, SOURCDOC, SERIES)
	from dbo.dcemvwtransaccion
	where YEAR(trxdate) = @year1
	  and month(trxdate) = @periodid
	FOR XML path('RepAux:DetAuxFol'), type
)
 return @cncp
end

go
IF (@@Error = 0) PRINT 'Creación exitosa de la función: DcemFcnDetFolios()'
ELSE PRINT 'Error en la creación de la función: DcemFcnDetFolios()'
GO
