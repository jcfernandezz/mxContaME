

select sourcdoc, *
from vwFINAsientosTAH
where datediff(day,'01/18/18', trxdate) >=0

sourcdoc in ('CRJ', 'RMJ') --, 'SJ')

datediff(day,'11/1/17', trxdate) >=0
and ortrxtyp = 9
--where ORCTRNUM like 'PYMNT00000097%'	--'RECCOB00000341%'

select *
from CM00101

select top 100 pyenttyp, ttlpymts, chekbkid, *
from pm30200
where datediff(day, '1/26/18', docdate) = 0
--pyenttyp != 0
--ttlpymts != 0


select top 100 *
from vwRmTransaccionesTodas
where 
datediff(day,'01/18/18', docdate) >=0
--and rmdtypal = 9


