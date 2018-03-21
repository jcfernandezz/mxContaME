IF OBJECT_ID ('dbo.dcemFnGetDatosBancoDelProveedor') IS NOT NULL
   DROP FUNCTION dbo.dcemFnGetDatosBancoDelProveedor
GO

create function dbo.dcemFnGetDatosBancoDelProveedor(@CustomerVendor_ID char(15), @ADRSCODE char(15), @SERIES smallint, @COMMENT1 char(30), @COMMENT2 char(30))
returns table
as
--Propósito. Devuelve los datos bancarios del proveedor 
--Requisito. 
--29/01/16 jcf Creación 
--29/01/18 jcf Los datos bancarios del proveedor dependen del parámetro P_DATOSBANCO
--
return
(
	select left(eft.intlBankAcctNum, 3) codBanco, rtrim(eft.bankname) bankname, rtrim(eft.intlBankAcctNum) intlBankAcctNum, rtrim(eft.custVendCountryCode) custVendCountryCode, eft.series,
		case when pa.param1 = '01' then dbo.dcemFnGetSegmentoX(@COMMENT1, 1) 
			 when pa.param1 = '02' then left(eft.intlBankAcctNum, 3) 
			 else '' 
		end bancoSat, 
		case when pa.param1 = '01' then @COMMENT2 
			 when pa.param1 = '02' then rtrim(eft.intlBankAcctNum)
			 else '' 
		end cuentaSat, 
		case when pa.param1 = '01' then dbo.dcemFnGetSegmentoX(rtrim(@COMMENT1), 2) 
			 when pa.param1 = '02' then rtrim(eft.bankname)
			 else '' 
		end nomBancoExtranjero
	from sy06000 eft
		outer apply dbo.dcemFnParametros('P_DATOSBANCO', '-', '-', '-', '-', '-') pa
	where eft.CustomerVendor_ID = @CustomerVendor_ID
	and eft.ADRSCODE = @ADRSCODE
	and eft.SERIES = @SERIES
)
go


IF (@@Error = 0) PRINT 'Creación exitosa de la función: dcemFnGetDatosBancoDelProveedor()'
ELSE PRINT 'Error en la creación de la función: dcemFnGetDatosBancoDelProveedor()'
GO
-------------------------------------------------------------------------------
--select PARAM1
--from dcemFnGetDatosBancoDelProveedor('NAUT', '-', '-', '-', '-', '-')

