-------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.dcemFnGetMetodosPagoPM') IS NOT NULL
   DROP FUNCTION dbo.dcemFnGetMetodosPagoPM
GO

create function dbo.dcemFnGetMetodosPagoPM(@chekbkid varchar(15), @pyenttyp smallint, @cardname varchar(15))
returns table
as
--Propósito. Obtiene métodos de pago PM
--Requisitos. 
--08/03/13 jcf Creación 
--26/06/14 jcf Modifica método de pago del efectivo
--19/02/15 jcf Modifica código de método de pago
--29/01/18 jcf Replantea el método de pago. La configuración de medios depende de si la chequera es tratada como una cuenta bancaria CB
--
return
( 	
	select cm.chekbkid, 
		case when left(UPPER(cm.locatnid), 2) = 'CB' then	--ch representa una cuenta bancaria
 				case @pyenttyp  
 					when 0 then '02'					--cheque
 					when 1 then '03'					--transf. electrónica
 					when 2 then left(@cardname,2)
 					when 3 then '03'					--transf. electrónica

					--pago simultáneo
					--when 4 then '03'				--transf. electrónica
 				--	when 5 then '02'				--cheque
 				--	when 6 then left(@cardname,2)	--tarjeta
					else null 
				end
			else									--representa un medio de pago
 				left(Rtrim(cm.locatnid), 2)
		end	codMetodoPago,

		case when left(UPPER(cm.locatnid), 2) = 'CB' then	--ch representa una cuenta bancaria
				case @pyenttyp
					WHEN 0 then 'CHEQUE'
					when 1 then 'TRANSFERENCIA'
					when 2 then 'OTRO'
					when 3 then 'TRANSFERENCIA' 
					else ''
				end
			else								--representa un medio de pago
 				Rtrim(cm.dscriptn)
		end medioid
	from CM00100 cm
	where cm.chekbkid = @chekbkid

)
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: dcemFnGetMetodosPagoPM()'
ELSE PRINT 'Error en la creación de la función: dcemFnGetMetodosPagoPM()'
GO

--TEST
--select *
--from dcemFnGetMetodosPagoPM(1)
-------------------------------------------------------------------------------------
