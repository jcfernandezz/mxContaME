use MTP1

sp_columns gl20000
sp_statistics gl20000

create table dcem.dcemPoliza (
JRNENTRY	int	not null PRIMARY KEY,
TRXDATE	datetime not null PRIMARY KEY,
nodoConcepto xml null,
nodoTransaccion xml null
);
------------------------------------------------------------------------------

BEGIN TRAN;
MERGE dcem.dcemPoliza AS T
USING Source AS S
ON (T.EmployeeID = S.EmployeeID) 
WHEN NOT MATCHED BY TARGET AND S.EmployeeName LIKE 'S%' 
    THEN INSERT(EmployeeID, EmployeeName) VALUES(S.EmployeeID, S.EmployeeName)
WHEN MATCHED 
    THEN UPDATE SET T.EmployeeName = S.EmployeeName
WHEN NOT MATCHED BY SOURCE AND T.EmployeeName LIKE 'S%'
    THEN DELETE 
OUTPUT $action, inserted.*, deleted.*;
ROLLBACK TRAN;




