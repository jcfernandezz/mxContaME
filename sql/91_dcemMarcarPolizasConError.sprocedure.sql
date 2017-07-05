
IF EXISTS (
  SELECT * 
    FROM INFORMATION_SCHEMA.ROUTINES 
   WHERE SPECIFIC_SCHEMA = 'dcem'
     AND SPECIFIC_NAME = 'dcemMarcarPolizasConError' 
)
   DROP PROCEDURE dcem.dcemMarcarPolizasConError;
GO
------------------------------------------------------------------------------------------------------
--Propósito. Marca pólizas que no validan
--Requisito. Este sp se ejecuta desde la aplicación
--23/6/17 jcf Creación
--
create PROCEDURE dcem.dcemMarcarPolizasConError (@jrnentry varchar(20)) --, @year varchar(4))
AS
	update dcem.dcempoliza set err = 1
	where jrnentry = convert(int, @jrnentry)
	--and YEAR(trxdate) = CONVERT(int, @year);

GO

IF (@@Error = 0) PRINT 'Creación exitosa de: dcem.dcemMarcarPolizasConError'
ELSE PRINT 'Error en la creación de: dcem.dcemMarcarPolizasConError'
GO

----------------------------------------------------------------------------------------------------
-- =============================================
-- test
-- =============================================
--update dcem.dcemPoliza set err = 1
--where JRNENTRY = 261;

--exec dcem.dcemMarcarPolizasConError

--select jrnentry, err
--from dcem.dcempoliza
--where err = 1
