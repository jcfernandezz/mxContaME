-------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.dcemFnGetRMtrx') IS NOT NULL
   DROP FUNCTION dbo.dcemFnGetRMtrx
GO

create function dbo.dcemFnGetRMtrx(@VCHRNMBR varchar(21), @DOCTYPE smallint)
returns table
as
--Propósito. Obtiene datos de trx RM: factura, cobro manual o mcp
--Requisitos. 
--14/01/15 jcf Creación 
--19/02/15 jcf Agrega isocurrc, xchgrate, ororgtrx, bancoOriCountry, bancoDesCountry. Modifica codMetodoPago
--25/02/15 jcf Incluye caso de banco extranjero
--08/04/15 jcf Agrega ccode
--
return
( 
	select pt.RMDTYPAL, pt.DOCNUMBR, pt.VOIDstts, 
		rtrim(isnull(bd.grupid, mp.codMetodoPago)) codMetodoPago,  --indica si es 02 cheque, 03 transf u otro
		rtrim(isnull(bd.docnumbr, pt.cheknmbr)) numCheque,

		rtrim(isnull(bd.bancoOrigenSat, cb.codBancoSat)) bancoDestinoSat,
		rtrim(isnull(bd.ctaOrigenSat, cb.bnkactnm)) ctaDestinoSat , 
		RTRIM(ISNULL(bd.nomBancoExt, cb.nomBancoExt)) banDesExt,

		isnull(bd.emidate, pt.DOCDATE) docdate, 
		isnull(bd.amounto, pt.ororgtrx) docamnt, 
		rtrim(emi.nombre) beneficiarioSat,
		pt.txrgnnum, 

		--actualmente no utilizado por el SAT v2
		pt.bnkbrnch bancoOrigenSat, 
		case when len(rtrim(isnull(mad.USERDEF1, ''))) < 4 then 'no identificado' else rtrim(mad.USERDEF1) end ctaOrigenSat,
		pt.bankname banOriExt,
		--
		
		upper(mad.country) country, upper(mad.ccode) ccode, mn.ISOCURRC, pt.xchgrate
	from dbo.vwRmTransaccionesTodas pt			--[doctype, vchrnmbr]
		outer apply dbo.dcemFnGetMcp(pt.DOCNUMBR, pt.bchsourc) bd
		outer apply dbo.dcemFnGetMetodosPagoRM(pt.cshrctyp) mp
		outer apply dbo.dcemFnGetDatosBancarios(pt.mscschid) cb
		outer apply dbo.fCfdEmisor() emi
		left join rm00102 mad				--rm_customer_mstr_addr [CUSTNMBR ADRSCODE]
			on mad.custnmbr = pt.custnmbr
			and mad.adrscode = pt.prbtadcd
		left join DYNAMICS..MC40200 mn
			on mn.curncyid = pt.curncyid
	where pt.DOCNUMBR = @VCHRNMBR
	and pt.RMDTYPAL = @DOCTYPE
)
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: dcemFnGetRMtrx()'
ELSE PRINT 'Error en la creación de la función: dcemFnGetRMtrx()'
GO

--select *
--from vwRmTransaccionesTodas
--where RMDTYPAL = 9

--select *
--from rm20101
--where RMDTYPAL = 9

