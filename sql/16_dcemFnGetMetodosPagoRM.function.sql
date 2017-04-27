
-------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.dcemFnGetMetodosPagoRM') IS NOT NULL
   DROP FUNCTION dbo.dcemFnGetMetodosPagoRM
GO

create function dbo.dcemFnGetMetodosPagoRM(@pyenttyp smallint)
returns table
as
--Propósito. Obtiene métodos de pago RM
--Requisitos. 
--08/03/13 jcf Creación 
--26/6/14 jcf Modifica método de pago del tipo 1: efectivo
--19/2/15 jcf Modifica código de método de pago
--
return
( 	
	select 
	case when @pyenttyp=3 then 'TRANSFERENCIA' 
		when @pyenttyp=2 then 'TARJETA DE CRÉDITO'
		when @pyenttyp=0 then 'CHEQUE'
		when @pyenttyp=1 then 'TRANSFERENCIA'		--EFECTIVO
		else ''
	end medioid, 
	case when @pyenttyp=3 then '03' 
		when @pyenttyp=2 then '04'		--TARJETA DE CRÉDITO
		when @pyenttyp=0 then '02'		--CHEQUE
		when @pyenttyp=1 then '03'		--TRANSFERENCIA DE FONDOS
		else ''
	end	codMetodoPago
)
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: dcemFnGetMetodosPagoRM()'
ELSE PRINT 'Error en la creación de la función: dcemFnGetMetodosPagoRM()'
GO

--TEST
--select *
--from fnAndinaGetMetodosPagoPM(1)
