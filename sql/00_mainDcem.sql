--Propósito. Objetos sql para contabilidad electrónica de México. Requerimiento de impuestos México.
--Requisitos. 
--23/4/15 jcf Creación
--28/1/16 jcf Corrige objetos base para usar alter
--29/1/16 jcf Corrección a instalar en producción MTP
	--configurar parámetro
	--vwPmTransaccionesTodas
	--08_dcemFnGetDatosBancoDelProveedor.function.sql
	--09_dcemFnGetDatosBancarios.function.sql
	--18_dcemFnGetPMtrx.function.sql
--5/7/17 jcf Instalación de optimización de generación de pólizas en las 6 compañías de MTP
--
-------------------------------------------------------------------------------
--use mtp7
--go 

PRINT 'Creando objetos para contabilidad electrónica...'
:setvar workpath C:\jcTii\Desarrollo\MEX_ContaMediosElectr\mxContaME\sql

--Objetos base 
:r C:\jcTii\GPRelational\pmVwPmAplicados.view.sql
:On Error exit
:r C:\jcTii\GPRelational\pmVwTransaccionesTodas.view.sql
:On Error exit
:r C:\jcTii\GPRelational\popVwPopRecepcionesHdr.view.sql
:On Error exit
:r C:\jcTii\GPRelational\rmvwRmTransaccionesTodas.view.sql
:On Error exit
:r C:\jcTii\GPRelational\popvwPopPmDocumentosDeCompraLoteAbieHist.view.sql
:On Error exit

--Objetos DCEM
:r $(workpath)\01_dcemPoliza.table.sql
:On Error exit
:r $(workpath)\02_dcemFnParametros.function.sql
:On Error exit
:r $(workpath)\07_dcemFnGetSegmento2.sql
:On Error exit

:r $(workpath)\08_dcemFnGetDatosBancoDelProveedor.function.sql
:On Error exit

:r $(workpath)\09_dcemFnGetDatosBancarios.function.sql
:On Error exit
--:r $(workpath)\10_dcemFnGetMCP.function_con_mcp.sql
--:On Error exit
:r $(workpath)\10_dcemFnGetMCP.function_SIN_MCP.sql
:On Error exit
:r $(workpath)\14_dcemFnGetMetodosPagoPM.function.sql
:On Error exit
:r $(workpath)\16_dcemFnGetMetodosPagoRM.function.sql
:On Error exit
:r $(workpath)\18_dcemFnGetPMtrx.function.sql
:On Error exit
:r $(workpath)\20_dcemFnGetRMtrx.function.sql
:On Error exit
:r $(workpath)\21_DcemFcnGetFolioFiscalDeFacturaSOP.function.sql
:On Error exit
:r $(workpath)\22_dcemFnGetDocumentoOriginal.function.sql
:On Error exit
:r $(workpath)\50_dcemFcnReplace.function.sql
:On Error exit
:r $(workpath)\59_DcemFcnCatalogoCtasXML.function.sql
:On Error exit
:r $(workpath)\60_DcemFcnCatalogoXML.function.sql
:On Error exit
:r $(workpath)\61_dcemvwSaldos.view.sql
:On Error exit
:r $(workpath)\62_DcemFcnObtieneSaldo.function.sql
:On Error exit
:r $(workpath)\62_DcemFcnObtieneSaldoTab.function.sql
:On Error exit
:r $(workpath)\63_dcemfcnctas.function.sql
:On Error exit
:r $(workpath)\65_DcemFcnBalance.function.sql
:On Error exit
:r $(workpath)\70_dcemvwtransaccion.view.sql
:On Error exit
--deprecated
--:r 71_DcemFcnGetFolioFiscalDeFacturaPOP.function.sql
--:On Error exit
--:r 72_DcemFcnGetFolioFiscalDeFacturaPM.function.sql
--:On Error exit
--
:r $(workpath)\74_DcemFcnDocNac.function.sql
:On Error exit
:r $(workpath)\75_dcemFcnDocExt.function.sql
:On Error exit
:r $(workpath)\76_DcemFcnCheque.function.sql
:On Error exit
:r $(workpath)\77_DcemFcnTransferencia.function.sql
:On Error exit
:r $(workpath)\78_dcemFcnOtro.function.sql
:On Error exit
:r $(workpath)\79_DcemFcnTransaccion.function.sql
:On Error exit
:r $(workpath)\80_DcemFcnGetPolizaDesdeTabla.sql
:On Error exit
:r $(workpath)\80_dcemFcnPoliza.function.sql
:On Error exit
:r $(workpath)\81_dcemFcnPolizas.function.sql
:On Error exit
:r $(workpath)\82_DcemFcnDocNacFolio.function.sql
:On Error exit
:r $(workpath)\83_DcemFcnDocExtFolio.function.sql
:On Error exit
:r $(workpath)\84_DcemFcnDetFolios.function.sql
:On Error exit
:r $(workpath)\85_DcemFcnAuxiliarFolios.function.sql
:On Error exit
:r $(workpath)\86_dcemFcnObtieneSaldo.function.sql
:On Error exit
:r $(workpath)\87_DcemFcnDetalleAux.function.sql
:On Error exit
:r $(workpath)\88_dcemfcnctasblz.function.sql
:On Error exit
:r $(workpath)\89_DcemFcnAuxiliarCtas.function.sql
:On Error exit
:r $(workpath)\90_DcemVwContabilidad.view.sql
:On Error exit
:r $(workpath)\91_dcemCorrigePoliza.sprocedure.sql
:On Error exit
:r $(workpath)\91_dcemMarcarPolizasConError.sprocedure.sql
:On Error exit
:r $(workpath)\92_PreprocesaArchivosDeContaElectronica.sprocedure.sql
:On Error exit
:r $(workpath)\95_DaPermisosContaElectronica.grant.sql
:On Error exit


PRINT 'Objetos creados satisfactoriamente.'
print '*** ATENCIÓN: Verificar que la tabla dcemContabilidadExportados está creada!!!!! ***'
GO
-------------------------------------------------------------------------------------------

