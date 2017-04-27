
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dcemvwtransaccion]') AND OBJECTPROPERTY(id,N'IsView') = 1)
    DROP view dbo.[dcemvwtransaccion];
GO

create VIEW [dbo].[dcemvwtransaccion] AS  
--Propósito. Transacciones contables para generación de archivos del SAT

SELECT
A.ACTINDX,
GL.actnumst,
GL1.ACTDESCR,
A.TRXDATE,
A.DSCRIPTN,
A.DEBITAMT,
A.CRDTAMNT,
MND.ISOCURRC MONEDA,
A.XCHGRATE,
A.ORCTRNUM, 
A.ORTRXTYP, 
A.SOURCDOC,
A.JRNENTRY,
A.refrence,
A.ORMSTRID,
A.SERIES,
A.ordocnum,
A.dex_row_id
from gl20000 A, GL00105 GL, GL00100 GL1, DYNAMICS.DBO.MC40200 mnd
WHERE GL.ACTINDX   = a.ACTINDX
  AND GL1.ACTINDX  = a.ACTINDX
  AND MND.CURNCYID = A.CURNCYID
union ALL
SELECT
B.ACTINDX,
GL.actnumst,
GL1.ACTDESCR,
B.TRXDATE,
B.DSCRIPTN,
B.DEBITAMT,
B.CRDTAMNT,
MND.ISOCURRC MONEDA,
B.XCHGRATE,
B.ORCTRNUM, 
B.ORTRXTYP, 
B.SOURCDOC,
B.JRNENTRY,
B.refrence,
B.ORMSTRID,
B.SERIES,
B.ordocnum,
b.dex_row_id
from gl30000 B, GL00105 GL, GL00100 GL1, DYNAMICS.DBO.MC40200 mnd
WHERE GL.ACTINDX   = B.ACTINDX
  AND GL1.ACTINDX  = B.ACTINDX
  AND MND.CURNCYID = B.CURNCYID

GO

IF (@@Error = 0) PRINT 'Creación exitosa de la vista: [dcemvwtransaccion]'
ELSE PRINT 'Error en la creación de la vista: [dcemvwtransaccion]'
GO



