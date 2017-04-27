-------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.dcemFnGetMetodosPagoPM') IS NOT NULL
   DROP FUNCTION dbo.dcemFnGetMetodosPagoPM
GO

create function dbo.dcemFnGetMetodosPagoPM(@pyenttyp smallint)
returns table
as
--Prop�sito. Obtiene m�todos de pago PM
--Requisitos. 
--08/03/13 jcf Creaci�n 
--26/06/14 jcf Modifica m�todo de pago del efectivo
--19/2/15 jcf Modifica c�digo de m�todo de pago
--
return
( 	
	select 
	case when @pyenttyp=3 then 'TRANSFERENCIA' 
		when @pyenttyp=2 then 'TARJETA DE CREDITO'
		when @pyenttyp=0 then 'CHEQUE'
		when @pyenttyp=1 then 'TRANSFERENCIA'
		else ''
	end medioid, 
	case when @pyenttyp=3 then '03' 
		when @pyenttyp=2 then '04'		--TARJETA DE CR�DITO
		when @pyenttyp=0 then '02'		--CHEQUE
		when @pyenttyp=1 then '03'		--TRANSFERENCIA DE FONDOS
		else ''
	end	codMetodoPago
)
go

IF (@@Error = 0) PRINT 'Creaci�n exitosa de la funci�n: dcemFnGetMetodosPagoPM()'
ELSE PRINT 'Error en la creaci�n de la funci�n: dcemFnGetMetodosPagoPM()'
GO

--TEST
--select *
--from dcemFnGetMetodosPagoPM(1)
