use MTP1

sp_columns gl20000
sp_statistics gl20000

create schema dcem authorization rol_contaelectr;

create table dcem.dcemPoliza (
JRNENTRY	int	not null ,
TRXDATE	datetime not null ,
REFRENCE char(31) null,
nodoTransaccion xml null
PRIMARY KEY (JRNENTRY, TRXDATE)
);
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
SELECT *
FROM dcem.dcemPoliza 

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