
IF OBJECT_ID ('dbo.DcemFcnDetalleAux') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnDetalleAux]
GO

create function [dbo].[DcemFcnDetalleAux](@periodid SMALLINT, @year1 SMALLINT, @ACTINDX int)
returns xml 
as
--Propósito. Detalle
--02/2015 jmg Creación
--09/04/15 jcf Cambia a group by debido a posible repetición
--29/01/18 jcf Cambia atributos para v1.3
--
begin
 declare @cncp xml;
   WITH XMLNAMESPACES
(
    'http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/AuxiliarCtas' as "AuxiliarCtas"
)

 select @cncp = (
	select 
		cast(trxdate as date)				'@Fecha',
		jrnentry							'@NumUnIdenPol',
		dbo.DcemFcnReplace(rtrim(refrence))	'@Concepto',
		sum(cast(DEBITAMT as numeric(19,2)))'@Debe',
		sum(cast(CRDTAMNT as numeric(19,2)))'@Haber'
	from dbo.dcemvwtransaccion 
	where YEAR(trxdate) = @year1
	  and month(trxdate) = @periodid
	  and actindx = @ACTINDX
	 group by cast(trxdate as date), JRNENTRY, refrence
	FOR XML path('AuxiliarCtas:DetalleAux'), type
)
 return @cncp
end
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: DcemFcnDetalleAux()'
ELSE PRINT 'Error en la creación de la función: DcemFcnDetalleAux()'
GO