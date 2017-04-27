IF OBJECT_ID ('dbo.dcemFnGetSegmentoX') IS NOT NULL
   DROP FUNCTION dbo.dcemFnGetSegmentoX
GO

create function dbo.dcemFnGetSegmentoX(@texto varchar(21), @segmento smallint)
returns varchar(18)
--Propósito. Obtiene el segundo segmento de una cadena separada por @separador 
--16/01/14 jcf Creación 
--
begin
		return 
			case when @segmento = 2 then
					case when patindex('%-%', @texto) <> 0 then
										right(@texto, len(@texto)-patindex('%-%', @texto))
						when patindex('%/%', @texto) <> 0 then				
										right(@texto, len(@texto)-patindex('%/%', @texto))
					else ''
					end
				when @segmento = 1 then
					case when patindex('%-%', @texto) > 0 then
										left(@texto, patindex('%-%', @texto)-1)
						when patindex('%/%', @texto) > 0 then
										left(@texto, patindex('%/%', @texto)-1)					
					else @texto
					end
				else ''
			end
					
end
go


IF (@@Error = 0) PRINT 'Creación exitosa de: dcemFnGetSegmentoX()'
ELSE PRINT 'Error en la creación de: dcemFnGetSegmentoX()'
GO

--select dbo.dcemFnGetSegmentoX('321', 1), dbo.dcemFnGetSegmentoX('454-banco                      ', 2)
