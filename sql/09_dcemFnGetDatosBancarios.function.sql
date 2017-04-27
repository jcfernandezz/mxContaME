-------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.dcemFnGetDatosBancarios') IS NOT NULL
   DROP FUNCTION dbo.dcemFnGetDatosBancarios
GO

create function dbo.dcemFnGetDatosBancarios(@chekbkid varchar(15))
returns table
as
--Prop�sito. Obtiene datos de la cuenta y el banco
--Requisitos. 
--24/12/15 jcf Creaci�n 
--29/1/16 jcf Agrega par�metro para seleccionar el c�digo del banco
--
return
( 	
	select cb.CHEKBKID, cb.bnkactnm, bk.bankid, bk.bankname, bk.country, 
		case when pa.param1 = '01' then bk.bnkbrnch
			 when pa.param1 = '02' then bk.trnstnbr 
			 else '' 
		end codBancoSat, 
		case when bk.bnkbrnch = '999' or bk.trnstnbr = '999' then
			bk.BANKNAME
		else
			null
		end nomBancoExt
	from cm00100 cb
		left join SY04100 bk		--bank	
			on bk.BANKID = cb.BANKID
		outer apply dbo.dcemFnParametros('P_DATOSBANCO', '-', '-', '-', '-', '-') pa
	where cb.CHEKBKID = @chekbkid

)
go

IF (@@Error = 0) PRINT 'Creaci�n exitosa de la funci�n: dcemFnGetDatosBancarios()'
ELSE PRINT 'Error en la creaci�n de la funci�n: dcemFnGetDatosBancarios()'
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
