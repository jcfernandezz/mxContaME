--Contabilidad electrónica
--Propósito. Guarda el log de los archivos exportados de contabilidad electrónica
--Usado por. App
--2015 abecker Creación
--
IF not EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.DcemContabilidadExportados') AND OBJECTPROPERTY(id,N'IsTable') = 1)
begin

	CREATE TABLE [dbo].[DcemContabilidadExportados](
		[id] [int] IDENTITY(1,1) NOT NULL,
		[fecha] [datetime] NOT NULL,
		[year1] [smallint] NOT NULL,
		[periodid] [smallint] NOT NULL,
		[catalogo] [xml] NOT NULL,
		[tipodoc] [nchar](20) NOT NULL,
		[version] [smallint] NOT NULL,
	 CONSTRAINT [PK_DcemContabilidadExportados] PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
end
	
GO

GRANT SELECT, UPDATE, INSERT, DELETE ON [DcemContabilidadExportados] TO rol_contaelectr
---------------------------------------------------------------------------------------------
--TEST
--select *
--from [DcemContabilidadExportados]
