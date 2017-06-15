use MTP1

select [dbo].[DcemFcnPolizas](1, 2017)

--use mtp1
--select jrnentry, trxdate, refrence
--		from dbo.dcemvwtransaccion
--		where datediff(day, '6/10/17', trxdate) >= 0
--		and datediff(day, '6/12/17', trxdate) <= 0
--		--and jrnentry between 92000 and 92300
--		GROUP BY JRNENTRY, TRXDATE, refrence

	--select t.jrnentry, t.trxdate, t.refrence --, dbo.dcemfcntransaccion(0, 0, 0, t.jrnentry, t.TRXDATE)
	--from dbo.dcemvwtransaccion t
	--where not exists (
	--	select p.jrnentry 
	--	from dcem.dcemPoliza p
	--	where p.jrnentry = t.jrnentry
	--	and datediff(day, p.trxdate, t.trxdate) = 0
	--)
	--and datediff(day, '5/13/17', trxdate) >= 0
	--and datediff(day, '6/14/17', trxdate) <= 0
	--GROUP BY t.JRNENTRY, t.TRXDATE, t.refrence


------------------------------------------------------------------------------


BEGIN TRAN;
MERGE dcem.dcemPoliza AS T
USING (
    select 
		jrnentry,
		trxdate,
		refrence
	from dbo.dcemvwtransaccion
	where jrnentry between 92000 and 92300
	GROUP BY JRNENTRY, TRXDATE, refrence
) AS S
ON (T.jrnentry = S.jrnentry
	and datediff(day, T.trxdate, S.trxdate) = 0)
WHEN MATCHED AND convert(varchar(max), isnull(T.nodoTransaccion, '')) !=  convert(varchar(max), dbo.dcemfcntransaccion(0, 0, 0, S.jrnentry, 0))
	THEN UPDATE SET T.nodoTransaccion = dbo.dcemfcntransaccion(0, 0, 0, S.jrnentry, 0)
WHEN NOT MATCHED BY TARGET 
    THEN INSERT(jrnentry, trxdate, refrence, nodoTransaccion) VALUES(S.jrnentry, S.trxdate, S.refrence, dbo.dcemfcntransaccion(0, 0, 0, S.jrnentry, 0));
--OUTPUT $action, inserted.*, deleted.*;
COMMIT TRAN;

-------------------------------------------------------------------------------------
SELECT YEAR(trxdate), MONTH(trxdate), count(*)
FROM dcem.dcemPoliza 
group by YEAR(trxdate), MONTH(trxdate)
order by 1, 2

INSERT into dcem.dcemPoliza(jrnentry, trxdate, refrence, nodoTransaccion) 
	select t.jrnentry, t.trxdate, t.refrence, dbo.dcemfcntransaccion(0, 0, 0, t.jrnentry, 0)
	from dbo.dcemvwtransaccion t
	where not exists (
		select p.jrnentry 
		from dcem.dcemPoliza p
		where p.jrnentry = t.jrnentry
		and datediff(day, p.trxdate, t.trxdate) = 0
	)
--	and YEAR(trxdate) >= 2015
	and t.jrnentry between 92000 and 92300
	GROUP BY t.JRNENTRY, t.TRXDATE, t.refrence

	--delete from dcem.dcemPoliza