
select *
from DBO. DCEMVWCONTABILIDAD
where year1 = 2015

select dbo.DCEMFCNCATALOGOXML(1, 2015)

select dbo.DCEMFCNBALANCE(1, 2015)

select dbo.DCEMFCNPOLIZAS(1, 2015)

select dbo.DCEMFCNAUXILIARFOLIOS(1, 2015)

select dbo.DCEMFCNAUXILIARCTAS(1, 2015)

--------------------------------------------------
select *
--update gl set userdef1 = ''
from gl00100 gl
where gl.userdef1 = '.'

select replace (cia.TAXREGTN, 'RFC ', '')TAXREGTN FROM DYNAMICS..SY01500 cia where cia.INTERID = DB_NAME()

select bnkbrnch, * 
--update rm set bnkbrnch = '999'
from rm00101 rm

select bnkactnm, *
update cm set bnkactnm = '10556644'
from cm00100 cm

select bnkbrnch, *
--update bc set bnkbrnch = '002'
from SY04100 bc

select comment1, comment2, ccode, *
--update pm set txrgnnum = 'XEXX010101000' --comment1 = '002', comment2 = '331177'
from pm00200 pm
where txrgnnum = ''

select *
--update ff set mexfoliofiscal = 'ea111111-0628-4638-a1e7-e8e39943e8d1'
from ACA_IETU00400 ff
where ff.mexfoliofiscal like 'jcjcjcjc%'

 select USERDEF1, *
--update A set userdef1 = '102'
 FROM GL00100 A
 where A.USERDEF1 = '.'
 
----------------------------------------------------

select *
from gl20000
where jrnentry = 37492

select *
from dbo.dcemFnGetDocumentoOriginal('OP00001951           ', 6, 'PMPAY')

select *
from dbo.dcemFnGetPMtrx('OP00001896', 6)

update gl set userdef1 = '105.01'
--select userdef1, *
from gl00100 gl
where actindx = 18
--gl.userdef1 != ''
------------------------------------------------------------------

--carga de comprobantes cfdi
select *
from dace.ComprobanteCFDI

select *
from dace.ComprobanteCFDIRelacionado


