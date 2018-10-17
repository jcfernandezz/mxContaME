--DROP TABLE daceComprobanteCFDIRelacionado;
--DROP TABLE daceComprobanteCFDI;


IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'dace')
BEGIN
    EXEC( 'create schema dace authorization rol_contaelectr;' );
END
go

-- ------------------------------------------------------------
-- Guarda datos relevantes de los CFDIs recibidos del proveedor
-- ------------------------------------------------------------
--IF not EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dace.daceComprobanteCFDI') AND OBJECTPROPERTY(id,N'IsTable') = 1)
IF OBJECT_ID('dace.ComprobanteCFDI', 'U') IS NULL
begin

CREATE TABLE dace.ComprobanteCFDI (
  UUID VARCHAR(50)  NOT NULL  ,
  TIPOCOMPROBANTE VARCHAR(1)  NOT NULL  ,
  FECHA VARCHAR(20)  NOT NULL  ,
  TOTAL NUMERIC(19,5)  NOT NULL DEFAULT 0 ,
  MONEDA NUMERIC(19,5)  NOT NULL DEFAULT 0 ,
  EMISOR_RFC VARCHAR(30)  NOT NULL  ,
  RESUMENCFDI VARCHAR(100)    ,
  NOMBREARCHIVO VARCHAR(255)  NOT NULL  ,
  CARPETAARCHIVO VARCHAR(255)  NOT NULL    ,
PRIMARY KEY(UUID, TIPOCOMPROBANTE));
end
GO

-- ------------------------------------------------------------
-- Contiene los cfdis relacionados al pago.
-- ------------------------------------------------------------
--IF not EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dace.daceComprobanteCFDI') AND OBJECTPROPERTY(id,N'IsTable') = 1)
IF OBJECT_ID('dace.ComprobanteCFDIRelacionado', 'U') IS NULL
begin
CREATE TABLE dace.ComprobanteCFDIRelacionado (
  CFDI_TIPOCOMPROBANTE VARCHAR(1)  NOT NULL  ,
  CFDI_UUID VARCHAR(50)  NOT NULL  ,
  UUIDRELACIONADO VARCHAR(50)  NOT NULL    ,
  FOREIGN KEY(CFDI_UUID, CFDI_TIPOCOMPROBANTE)
    REFERENCES dace.ComprobanteCFDI(UUID, TIPOCOMPROBANTE)
      ON DELETE CASCADE);


end

GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE Name = 'daceComprobanteCFDIRelacionado_FKIndex1') 
	DROP INDEX dace.ComprobanteCFDIRelacionado.daceComprobanteCFDIRelacionado_FKIndex1;

CREATE UNIQUE INDEX daceComprobanteCFDIRelacionado_FKIndex1 ON dace.ComprobanteCFDIRelacionado (CFDI_UUID, CFDI_TIPOCOMPROBANTE, UUIDRELACIONADO);
GO
