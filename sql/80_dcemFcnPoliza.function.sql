	-------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.DcemFcnPoliza') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnPoliza]
GO

create function [dbo].[DcemFcnPoliza](@year1 SMALLINT, @periodid SMALLINT)
returns xml 
as
--Propósito. Póliza
--12/2014 jmg Creación
--
begin
 declare @cncp xml;
  WITH XMLNAMESPACES
(
    'www.sat.gob.mx/esquemas/ContabilidadE/1_1/PolizasPeriodo' as "PLZ"
)

 select @cncp = (
	 select 
		--case sourcdoc
		--when 'DG' then '3'
		--when 'PMVPY' then '2'
		--when 'MM' then '3'
		--when 'BBF' then '3'
		--when 'SJ' then '1'
		--when 'CRJ' then '1'
		--when 'RECVG' then '2'
		--when 'PMVVR' then '2'
		--when 'P/L' then '3'
		--when 'APL' then '3'
		--when 'PMTRX' then '2'
		--when 'PMPAY' then '2'
		--when 'FAADJ' then '3'
		--when 'PORET' then '2'
		--when 'RMJ' then '1'
		--end '@Tipo',
		jrnentry							'@NumUnIdenPol',
		cast(trxdate as date)				'@Fecha',
		dbo.DcemFcnReplace(rtrim(refrence)) '@Concepto',
		dbo.dcemfcntransaccion(@year1, @periodid, ACTINDX, jrnentry, dex_row_id)
	from dbo.dcemvwtransaccion
	where YEAR(trxdate) = @year1
	  and month(trxdate) = @periodid
FOR XML path('PLZ:Poliza'), type
)
 return @cncp
end
go
IF (@@Error = 0) PRINT 'Creación exitosa de la función: DcemFcnPoliza()'
ELSE PRINT 'Error en la creación de la función: DcemFcnPoliza()'
GO
