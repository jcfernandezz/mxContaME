
IF OBJECT_ID ('dbo.DcemFcnReplace') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnReplace]
GO

create function [dbo].[DcemFcnReplace] (@campo varchar(51)) returns varchar(51)
--Propósito. Reemplaza caracteres especiales
--7/4/15 jcf Corrección de parámetros
as
begin
	return(
     (select REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@campo
       ,'&', '&amp;'),'"', '&quot;'),'<', '&lt;'),'>', '&gt;'),'''', '&apos;')) 
          )
end

GO

IF (@@Error = 0) PRINT 'Creación exitosa de la función: DcemFcnReplace()'
ELSE PRINT 'Error en la creación de la función: DcemFcnReplace()'
GO
