-------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.dcemFnGetMcp') IS NOT NULL
   DROP FUNCTION dbo.dcemFnGetMcp
GO

create function dbo.dcemFnGetMcp(@VCHRNMBR varchar(21), @bchsourc char(15))
returns table
as
--Propósito. Obtiene las línea del recibo de cobro - pago de MCP
--Requisitos. Debe estar instalado mcp.
--01/03/13 jcf Creación 
--19/02/15 jcf Agrega amounto
--
return
( 	
	select ln.MCPTYPID, ln.NUMBERIE, ln.MEDIOID, ln.LNSEQNBR, med.grupid, med.chekbkid, ln.docnumbr, 
		ct.nomBancoExt,	upper(ct.country) country, 
		ct.codBancoSat bancoOrigenSat, ct.bnkactnm ctaOrigenSat, ln.emidate, ln.LINEAMNT, ln.amounto, cia.cmpnynam beneficiarioSat
	from nfmcp20100	ln			--Líneas de Recibos de cobro [MCPTYPID NUMBERIE LNSEQNBR MEDIOID]
	inner join nfmcp00700 med	--nfmcp_medios_cp_mstr
		on med.medioid = ln.medioid
	left join dynamics..SY01500	cia
		on cia.INTERID = DB_NAME()
	outer apply dbo.dcemFnGetDatosBancarios(med.chekbkid) ct
	where @bchsourc IN ('nfMCP_Cash', 'XRM_Cash')
	and numberie = @VCHRNMBR

	union all	

	select ln.[MCPTYPID], ln.[NUMBERIE], ln.[MEDIOID], ln.[LNSEQNBR], med.grupid, med.chekbkid, ln.docnumbr, 
		ct.nomBancoExt, upper(ct.country) country, 
		ct.codBancoSat bancoOrigenSat, ct.bnkactnm ctaOrigenSat, ln.emidate, ln.LINEAMNT, ln.amounto, ln.titacct beneficiarioSat
	from nfmcp_pm20100 ln		--nfmcp_payment_line_open [MCPTYPID],[NUMBERIE],[MEDIOID],[LNSEQNBR]
	inner join nfmcp00700 med	--nfmcp_medios_cp_mstr
		on med.medioid = ln.medioid
	outer apply dbo.dcemFnGetDatosBancarios(ln.TII_CHEKBKID) ct
	where @bchsourc in ('nfMCP_Payment', 'XXPM_Payment')
	and numberie = @VCHRNMBR
)
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: dcemFnGetMcp()'
ELSE PRINT 'Error en la creación de la función: dcemFnGetMcp()'
GO

--select *
--from nfmcp_pm20100 ln		--nfmcp_payment_line_open [MCPTYPID],[NUMBERIE],[MEDIOID],[LNSEQNBR]

--select *
--from SY04100
