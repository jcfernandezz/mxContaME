
IF OBJECT_ID ('dbo.DcemFcnObtieneSaldoTab') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnObtieneSaldoTab]
GO

create function [dbo].[DcemFcnObtieneSaldoTab] (@cta int, @periodid SMALLINT, @year1 SMALLINT) 
returns table
--Propósito. Obtiene acumulado de resumen de cuentas
--2/2/16 jcf Creación
--
as
	return(
		select sum(debitamt) - SUM(crdtamnt) saldo
		  from dcemvwSaldos b
		where ACTINDX = @cta			
		and year1 = @year1
		and PERIODID < @periodid	
	)
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: [DcemFcnObtieneSaldoTab]()'
ELSE PRINT 'Error en la creación de la función: [DcemFcnObtieneSaldoTab]()'
GO


