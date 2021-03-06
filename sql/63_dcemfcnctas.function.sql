
IF OBJECT_ID ('dbo.dcemfcnctas') IS NOT NULL
   DROP FUNCTION dbo.[dcemfcnctas]
GO

create function [dbo].[dcemfcnctas](@periodid SMALLINT, @year1 SMALLINT)

returns xml 
as
--Propósito. Acumulacion de Saldos por Cta
--12/2014 jmg Creación
--7/4/15 jcf Replanteo de consulta
--2/2/16 JCF Optimiza consulta
--
begin
 declare @cncp xml;
 WITH XMLNAMESPACES
(
    'http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion' as "BCE"
) 
  select @cncp = 
 (
	  select 
		left(ltrim(rtrim(ct.ACTNUMST)), 100) '@NumCta',
		cast(isnull(sa.saldo, 0) as numeric(19,2))  '@SaldoIni',
		--cast(dbo.DcemFcnObtieneSaldo(a.ACTINDX, @periodid, @year1) as numeric(19,2))  '@SaldoIni',
		cast(a.debitamt as numeric(19,2))    '@Debe',
		cast(a.CRDTAMNT as numeric(19,2))    '@Haber',
		cast(isnull(sa.saldo, 0) + (a.debitamt - a.CRDTAMNT) as numeric(19,2)) '@SaldoFin'
		--cast(dbo.DcemFcnObtieneSaldo(a.ACTINDX, @periodid ,@year1) + (a.debitamt - a.CRDTAMNT) as numeric(19,2)) '@SaldoFin'
	  from dcemvwSaldos a
		inner join GL00105 ct
			on a.ACTINDX = ct.ACTINDX
		outer apply dbo.DcemFcnObtieneSaldoTab(a.ACTINDX, @periodid ,@year1) sa
	 where a.PERIODID = @periodid
	   and a.year1 = @year1
	 order by a.actindx asc, a.YEAR1 asc, a.PERIODID asc
	 --
	 FOR XML path('BCE:Ctas'), type
     )
 return @cncp
end
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: [dcemfcnctas]()'
ELSE PRINT 'Error en la creación de la función: [dcemfcnctas]()'
GO


--select dbo.dcemfcnctas(1, 2016)
