----------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.dcemFnGetPMtrx') IS NOT NULL
   DROP FUNCTION dbo.dcemFnGetPMtrx
GO

create function dbo.dcemFnGetPMtrx(@VCHRNMBR varchar(21), @DOCTYPE smallint)
returns table
as
--Propósito. Obtiene datos del doc pm original: 
--			factura, pagos mcp, pagos manuales, pagos anulados, cheques computarizados
--Requisitos. Configurar el código del método de pago en el grupo del medio mcp y en las funciones fngetMetodosPagoXX()
--Requisitos. 
--18/12/14 jcf Creación 
--19/02/15 jcf Agrega pt.isocurrc, pt.xchgrate. Modifica docamnt, codMetodoPago
--25/02/15 jcf Caso de banco extranjero
--7/4/15 jcf Agrega ccode, modifica nombre docnumbr, corrige docdate, beneficiarioSat
--29/1/16 jcf Agrega datos del banco destino de acuerdo a parámetro configurado
--
return
( 	--Documentos PM: factura, misceláneos, pagos, anulación de facturas, cheques computarizados
	select pt.doctype, pt.vchrnmbr, pt.voided, 
		rtrim(isnull(bd.grupid, mp.codMetodoPago)) codMetodoPago,  --indica si es 02 cheque, 03 transf u otro
		rtrim(isnull(bd.docnumbr, pt.docnumbr)) docnumbr,			--num cheque en caso de pago

		rtrim(isnull(bd.bancoOrigenSat, cb.codBancoSat)) bancoOrigenSat, 
		rtrim(isnull(bd.ctaOrigenSat, cb.bnkactnm)) ctaOrigenSat, 
		RTRIM(ISNULL(bd.nomBancoExt, cb.nomBancoExt)) banOriExt,

		case when isnull(bd.emidate, '1/1/1900') = '1/1/1900' then 
				pt.DOCDATE
			else
				bd.emidate
		end docdate, 
		isnull(bd.amounto, isnull(pt.ordocamt, pt.DOCAMNT)) docamnt, 
		case when isnull(bd.beneficiarioSat, '') = '' then
				rtrim(pt.vendname)
			else
				RTRIM(bd.beneficiarioSat)
		end beneficiarioSat,
		pt.txrgnnum, 
		
		case when pa.param1 = '01' then dbo.dcemFnGetSegmentoX(pt.comment1, 1) 
			 when pa.param1 = '02' then eft.codBanco 
			 else '' 
		end bancoDestinoSat, 
		case when pa.param1 = '01' then pt.comment2 
			 when pa.param1 = '02' then eft.intlBankAcctNum 
			 else '' 
		end ctaDestinoSat, 
		case when pa.param1 = '01' then dbo.dcemFnGetSegmentoX(rtrim(pt.comment1), 2) 
			 when pa.param1 = '02' then eft.bankname 
			 else '' 
		end banDesExt, 

		upper(pt.country) country, 
		case when pa.param1 = '01' then upper(pt.ccode) 
			 when pa.param1 = '02' then upper(eft.custVendCountryCode)
			 else '' 
		end ccode, 
		
		mn.ISOCURRC, pt.xchgrate
	from vwPmTransaccionesTodas pt			--[doctype, vchrnmbr]
		outer apply dbo.dcemFnGetMcp(pt.VCHRNMBR, pt.bchsourc) bd
		outer apply dbo.dcemFnGetMetodosPagoPM(pt.pyenttyp) mp
		outer apply dbo.dcemFnGetDatosBancarios(pt.chekbkid) cb
		outer apply dbo.dcemFnParametros('P_DATOSBANCO', '-', '-', '-', '-', '-') pa
		outer apply dbo.dcemFnGetDatosBancoDelProveedor(pt.vendorid, pt.vaddcdpr, 4) eft
		left join DYNAMICS..MC40200 mn
			on mn.curncyid = pt.curncyid
	where pt.VCHRNMBR = @VCHRNMBR
	and pt.DOCTYPE = @DOCTYPE
)
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: dcemFnGetPMtrx()'
ELSE PRINT 'Error en la creación de la función: dcemFnGetPMtrx()'
GO
-----------------------------------------------------------------------------------------------------------------
--select *
--from vwPmTransaccionesTodas
--where VCHRNMBR = 'OP00001951'

--select case when emidate = '1/1/1900' then '9/4/15' end , 
--case when beneficiarioSat = '' then 'x' else 'y' end
--from dbo.dcemFnGetMcp('OP00001951', 'XXPM_Payment   ') bd

