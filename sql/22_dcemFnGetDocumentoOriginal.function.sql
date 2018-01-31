-------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.dcemFnGetDocumentoOriginal') IS NOT NULL
   DROP FUNCTION dbo.dcemFnGetDocumentoOriginal
GO

create function dbo.dcemFnGetDocumentoOriginal(@VCHRNMBR varchar(21), @DOCTYPE smallint, @SOURCEDOC varchar(11))
returns table
as
--Propósito. Obtiene los comprobantes originales de facturas, pagos y cobros
--Requisitos. 
--18/12/14 jcf Creación 
--19/02/15 jcf Agrega curncyid, xchgrate, bancoOriCountry, bancoDesCountry. Modifica codMetodoPago
--07/04/15 jcf Agrega ccode, modifica origenDoc, agrega facturas pop
--
return
( 
	--Documentos PM      
	select cast('PM' as varchar(3)) origenDoc, 
		pt.doctype, pt.vchrnmbr, pt.voided, 
		pt.codMetodoPago,					--indica si es 02 cheque, 03 transf u otro
		pt.docnumbr,
		pt.bancoOrigenSat, pt.ctaOrigenSat, pt.banOriExt, 
		pt.docdate, 
		pt.docamnt, 
		pt.beneficiarioSat,	rtrim(pt.txrgnnum) txrgnnum, 
		pt.bancoDestinoSat, pt.ctaDestinoSat, pt.banDesExt, 
		pt.country, pt.ccode, pt.ISOCURRC, pt.xchgrate,
		rtrim(ff.MexFolioFiscal) MexFolioFiscal
	from dbo.dcemFnGetPMtrx(@VCHRNMBR, @DOCTYPE ) pt
		left join ACA_IETU00400 ff
         on ff.DOCTYPE = @DOCTYPE 
           and ff.VCHRNMBR = @VCHRNMBR
	where @SOURCEDOC in ('PMPAY', 'PMCHK', 'PMTRX') --'PMVVR', 'PMVPY', 

	union all
	--pago simultáneo en factura. 
	select cast('PS' as varchar(3)) origenDoc, 
		pt.doctype, pt.vchrnmbr, pt.voided, 
		mp.codMetodoPago, 
		pt.docnumbr, 
		cb.codBancoSat, cb.bnkactnm, cb.nomBancoExt,
		pt.DOCDATE, 
		isnull(pt.ordocamt, pt.DOCAMNT) DOCAMNT, 
		pt.vendname, rtrim(pt.txrgnnum) txrgnnum, 
		
		dbp.bancoSat, dbp.cuentaSat, dbp.nomBancoExtranjero, 

		upper(pt.country) country, upper(pt.ccode) ccode, mn.ISOCURRC, pt.xchgrate,
		null MexFolioFiscal
	from dbo.vwPmTransaccionesTodas pt			--[doctype, vchrnmbr]
		inner join dbo.tii_vwPmAplicadas ap
			on ap.vchrnmbr = pt.vchrnmbr
			and ap.doctype = pt.doctype
			and pt.bchsourc = 'PM_Trxent'
		outer apply dbo.dcemFnGetMetodosPagoPM(pt.chekbkid, pt.pyenttyp, pt.cardname) mp  
		outer apply dbo.dcemFnGetDatosBancarios(pt.chekbkid) cb
		outer apply dbo.dcemFnGetDatosBancoDelProveedor(pt.vendorid, pt.vaddcdpr, 4, pt.comment1, pt.comment2) dbp
		left join DYNAMICS..MC40200 mn
			on mn.curncyid = pt.curncyid
	where @SOURCEDOC = 'PMTRX'
	and ap.aptvchnm = @VCHRNMBR
	and ap.aptodcty = @DOCTYPE
	
	union all

	--facturas POP
	select cast('POP' as varchar(3)) origenDoc, 
		pt.doctype, pt.vchrnmbr, pt.voided, 
		null codMetodoPago, 
		pt.docnumbr, 
		'na' codBancoSat, 'na' bnkactnm, 'na' nomBancoExt,
		pt.DOCDATE, 
		isnull(pt.ordocamt, pt.DOCAMNT) DOCAMNT, 
		pt.vendname, rtrim(pt.txrgnnum) txrgnnum, 
	
		'na' bancoDestinoSat, 
		pt.comment2 ctaDestinoSat, 
		'na' banDesExt, 

		upper(pt.country) country, upper(pt.ccode) ccode, mn.ISOCURRC, pt.xchgrate,
		rtrim(ff.MexFolioFiscal) MexFolioFiscal
	from vwPopRecepcionesHdr pr				--facturas pop
		left join vwPmTransaccionesTodas pt		--[doctype, vchrnmbr]
			on pr.VCHRNMBR = pt.VCHRNMBR
			and pt.DOCTYPE = 1					--invoice
		left join DYNAMICS..MC40200 mn
			on mn.curncyid = pt.curncyid
		left join ACA_IETU00400 ff
			on ff.DOCTYPE = pt.DOCTYPE
			and ff.VCHRNMBR = pt.VCHRNMBR
	where @SOURCEDOC = 'RECVG'
	and pr.POPRCTNM = @VCHRNMBR

	union all
	--documentos RM 
	select cast('RM' as varchar(3)) origenDoc, 
		rm.RMDTYPAL, rm.DOCNUMBR, rm.VOIDstts, 
		rm.codMetodoPago, 
		rm.numCheque, 
		rm.bancoOrigenSat, rm.ctaOrigenSat, rm.banOriExt,
		rm.docdate, 
		rm.docamnt, 
		rm.beneficiarioSat,	rtrim(isnull(ff.rfc, rm.txrgnnum)) txrgnnum,
		rm.bancoDestinoSat, rm.ctaDestinoSat , rm.banDesExt, 
		rm.country, rm.ccode, rm.ISOCURRC, rm.xchgrate,
		rtrim(ff.foliofiscal)
	from dbo.dcemFnGetRMtrx(@VCHRNMBR, @DOCTYPE) rm
		outer apply dbo.DcemFnGetFolioFiscalDeDocumento (@VCHRNMBR, @DOCTYPE) ff      
	where @SOURCEDOC in ('CRJ', 'SJ')	--'RMJ', 
	--and rm.RMDTYPAL = 9

	union all
	--cobros simultáneos. 
	SELECT cast('CS' as varchar(3)) origenDoc, 
		rm.RMDTYPAL, rm.DOCNUMBR, rm.VOIDstts, 
		rm.codMetodoPago, 
		rm.numCheque, 
		rm.bancoOrigenSat, rm.ctaOrigenSat, rm.banOriExt,
		rm.docdate, 
		rm.docamnt, 
		rm.beneficiarioSat,	rtrim(isnull(ff.rfc, rm.txrgnnum)) txrgnnum,
		rm.bancoDestinoSat, rm.ctaDestinoSat , rm.banDesExt, 
		rm.country, rm.ccode, rm.ISOCURRC, rm.xchgrate,
		rtrim(ff.foliofiscal)
	from dbo.vwRmTrxAplicadas ap
		inner join vwRmTransaccionesTodas ti		--a
			on ti.docnumbr = ap.aptodcnm
			and ti.rmdtypal = ap.aptodcty
			and ti.cashamnt <> 0					--factura pagada simultáneamente
		cross apply dbo.dcemFnGetRMtrx(ap.APFRDCNM, ap.APFRDCTY) rm
		outer apply dbo.DcemFnGetFolioFiscalDeDocumento (ap.APFRDCNM, ap.APFRDCTY) ff      
	where @SOURCEDOC = 'SJ'
	and ap.APTODCNM = @VCHRNMBR
	and ap.aptodcty = @DOCTYPE
	and ap.APFRDCTY = 9						--pago
	and rm.bchsourc = 'Sales Entry'
	
	union all
	--Transferencia entre chequeras
	select cast('CF' as varchar(3)) origenDoc,
		0, cmxfrnum, 0, 
		'03', cmxfrnum, 
		bo.codBancoSat, bo.bnkactnm, bo.nomBancoExt,
		tf.cmxftdate, tb.origamt,
		cia.cmpnynam, rtrim(cia.taxregtn) taxregtn,
		bd.codBancoSat, bd.bnkactnm, bd.nomBancoExt,
		'MEXICO', 'MEX', mn.ISOCURRC, tb.xchgrate,
		NULL foliofiscal
	from cm20600 tf
		inner join cm20200 tb
			on tb.cmtrxnum = tf.cmxfrnum
			and tb.chekbkid = tf.cmfrmchkbkid
		left join DYNAMICS..MC40200 mn
			on mn.curncyid = tb.curncyid
		left join dynamics..SY01500	cia
			on cia.INTERID = DB_NAME()
		outer apply dbo.dcemFnGetDatosBancarios(tf.cmfrmchkbkid) bo
		outer apply dbo.dcemFnGetDatosBancarios(tf.cmchkbkid) bd
	where convert(varchar(21), tf.Xfr_Record_Number) = @VCHRNMBR
	and @SOURCEDOC = 'CMXFR'
	
	--union all
	----Transacciones bancarias. No incluye cobros con cheque, ni efectivo
	--select cast('CT' as varchar(2)) origenDoc,
	--	CMTrxType, CMTrxNum, voided, 
	--	case when CMTrxType = 3 then 'CHEQUE'
	--		else 'OTROS'
	--	end, 
	--	CHEKBKID, '999', '', '', '' 
	--from cm20200
	--where convert(varchar(21), CMRECNUM) = @VCHRNMBR
	--and @SOURCEDOC = 'CMTRX'
	--and CMTrxType in (3, 4, 5, 6)	--3:pago con ch, 4: retiro, 5:iaj, 6:daj
	
)
go



IF (@@Error = 0) PRINT 'Creación exitosa de la función: dcemFnGetDocumentoOriginal()'
ELSE PRINT 'Error en la creación de la función: dcemFnGetDocumentoOriginal()'
GO
-------------------------------------------------------------------------------------------------
--TEST
--select *
--from dbo.dcemFnGetDocumentoOriginal('OP00000688           ', 6, 'PMPAY')

--select *
--from dbo.dcemFnGetDocumentoOriginal('PYMNT000000000191    ', 9, 'RMJ')

