
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dcemvwSaldos]') AND OBJECTPROPERTY(id,N'IsView') = 1)
    DROP view dbo.[dcemvwSaldos];
GO

create VIEW [dbo].[dcemvwSaldos]  AS  
--Propósito. Saldo de cuentas por periodo
--
  select ACTINDX, YEAR1, periodid, CRDTAMNT, debitamt from gl10110
  union all
  select ACTINDX, YEAR1, periodid, CRDTAMNT, debitamt from gl10111
GO


IF (@@Error = 0) PRINT 'Creación exitosa de la vista: [dcemvwSaldos]'
ELSE PRINT 'Error en la creación de la vista: [dcemvwSaldos]'
GO


