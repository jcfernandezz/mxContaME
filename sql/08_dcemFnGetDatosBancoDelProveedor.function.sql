IF OBJECT_ID ('dbo.dcemFnGetDatosBancoDelProveedor') IS NOT NULL
   DROP FUNCTION dbo.dcemFnGetDatosBancoDelProveedor
GO

create function dbo.dcemFnGetDatosBancoDelProveedor(@CustomerVendor_ID char(15), @ADRSCODE char(15), @SERIES smallint)
returns table
as
--Propósito. Devuelve los datos bancarios del proveedor 
--Requisitos. 
--29/1/16 jcf Creación 
--
return
(
	select left(intlBankAcctNum, 3) codBanco, rtrim(bankname) bankname, rtrim(intlBankAcctNum) intlBankAcctNum, rtrim(custVendCountryCode) custVendCountryCode, series
	from sy06000
	where CustomerVendor_ID = @CustomerVendor_ID
	and ADRSCODE = @ADRSCODE
	and SERIES = @SERIES
)
go


IF (@@Error = 0) PRINT 'Creación exitosa de la función: dcemFnGetDatosBancoDelProveedor()'
ELSE PRINT 'Error en la creación de la función: dcemFnGetDatosBancoDelProveedor()'
GO
-------------------------------------------------------------------------------
--select PARAM1
--from dcemFnGetDatosBancoDelProveedor('NAUT', '-', '-', '-', '-', '-')

