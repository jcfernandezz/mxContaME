-------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.dcemFnGetDatosBancarios') IS NOT NULL
   DROP FUNCTION dbo.dcemFnGetDatosBancarios
GO

create function dbo.dcemFnGetDatosBancarios(@chekbkid varchar(15))
returns table
as
--Propósito. Obtiene datos de la cuenta y el banco
--Requisitos. 
--24/12/15 jcf Creación 
--29/01/16 jcf Agrega parámetro para seleccionar el código del banco
--26/01/18 jcf Rediseña query
--
return
( 	
	select cb.CHEKBKID, --cb.bnkactnm, 
		case when isnull(cb.EFTBANKACCT, '') != '' then cb.EFTBANKACCT 
			else cb.bnkactnm
		end bnkactnm,
		cb.bankid, cb.bankname, cb.country, 
		left(cb.BANKID, 3) codBancoSat, 
		case when left(cb.BANKID, 3) = '999' then
			cb.BANKNAME
		else
			null
		end nomBancoExt
	from dbo.cmFnGetDatosDeChequera(@chekbkid) cb
)
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: dcemFnGetDatosBancarios()'
ELSE PRINT 'Error en la creación de la función: dcemFnGetDatosBancarios()'
GO

--select *
--from cm00100 cb

--select *
--from dbo.dcemFnGetDatosBancarios('AMEX           ')

--select *
--from dbo.dcemFnGetDatosBancarios(
--select *
--from vwPmTransaccionesTodas
--where bchsourc in (
----'PM_Trxent'    ,  
--'XXPM_Trxent'   
----'Rcvg Trx Entry' ,
----'PM_Payment'     
----'XXPM_Payment'   ,
----'Ret Trx Entry'  
----'nfMCP_Payment'
--  )
