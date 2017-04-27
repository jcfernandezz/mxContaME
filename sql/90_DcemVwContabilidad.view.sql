--M�xico
--Prop�sito. Obtiene los periodos de los reportes y datos predeterminados para la emisi�n de la contabilidad electr�nica
--

--IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DcemVwContabilidad]') AND OBJECTPROPERTY(id,N'IsView') = 1)
--    DROP view dbo.[DcemVwContabilidad];
--GO
IF (OBJECT_ID ('dbo.DcemVwContabilidad', 'V') IS NULL)
   exec('create view dbo.DcemVwContabilidad as SELECT 1 as t');
go

ALTER VIEW [dbo].DcemVwContabilidad  AS  

select year1, periodid, 
--dbo.DcemFcnCatalogoXML(periodid, YEAR1) catalogo, 
'Cat�logo' tipodoc
  FROM SY40100
where FORIGIN = 1
  and SERIES = 0
  and ODESCTN = ''
  and PERIODID > 0

union all 

select year1, periodid, 
--dbo.DcemFcnBalance(YEAR1, periodid) balance, 
'Balanza' tipodoc
  FROM SY40100
where FORIGIN = 1
  and SERIES = 0
  and ODESCTN = ''
  and PERIODID > 0

union all 

select year1, periodid, 
--dbo.DcemFcnPolizas(YEAR1, periodid) balance, 
'P�lizas' tipodoc
  FROM SY40100
where FORIGIN = 1
  and SERIES = 0
  and ODESCTN = ''
  and PERIODID > 0

union all 

select year1, periodid, 
--dbo.DcemFcnAuxiliarFolios(YEAR1, periodid) balance, 
'Auxiliar folios' tipodoc
  FROM SY40100
where FORIGIN = 1
  and SERIES = 0
  and ODESCTN = ''
  and PERIODID > 0

union all 

select year1, periodid, 
--dbo.DcemFcnAuxiliarCtas(YEAR1, periodid) balance, 
'Auxiliar Cuentas' tipodoc
  FROM SY40100
where FORIGIN = 1
  and SERIES = 0
  and ODESCTN = ''
  and PERIODID > 0


GO

IF (@@Error = 0) PRINT 'Creaci�n exitosa de la vista: DcemVwContabilidad'
ELSE PRINT 'Error en la creaci�n de la vista: DcemVwContabilidad'
GO



--select *
--from [DcemVwContabilidad] 
