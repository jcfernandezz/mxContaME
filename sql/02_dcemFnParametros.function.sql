IF OBJECT_ID ('dbo.dcemFnParametros') IS NOT NULL
   DROP FUNCTION dbo.dcemFnParametros
GO

create function dbo.dcemFnParametros(@tag1 varchar(15), @tag2 varchar(15), @tag3 varchar(15), @tag4 varchar(15), @tag5 varchar(15), @tag6 varchar(15))
returns table
as
--Propósito. Devuelve los parámetros de la compañía para localización México. 
--Requisitos. Los @tagx deben configurarse en la ventana Información de internet del id de dirección LECU de la compañía.
--25/3/15 jcf Creación 
--
return
(
	select 
		case when charindex(@tag1+'=', ia.inetinfo) > 0 and charindex(char(13), ia.inetinfo) > 0 then
			substring(ia.inetinfo, charindex(@tag1+'=', ia.inetinfo) +len(@tag1)+1, charindex(char(13), ia.inetinfo, charindex(@tag1+'=', ia.inetinfo)) - charindex(@tag1+'=', ia.inetinfo) - len(@tag1)-1) 
		else 'no existe tag: '+@tag1 end param1,
		CASE when charindex(@tag2+'=', ia.inetinfo) > 0 and  charindex(char(13), ia.inetinfo) > 0 then
			substring(ia.inetinfo, charindex(@tag2+'=', ia.inetinfo)+ len(@tag2)+1, charindex(char(13), ia.inetinfo, charindex(@tag2+'=', ia.inetinfo)) - charindex(@tag2+'=', ia.inetinfo) - len(@tag2)-1) 
		else 'no existe tag: '+@tag2 end param2,
		CASE when charindex(@tag3+'=', ia.inetinfo) > 0 and  charindex(char(13), ia.inetinfo) > 0 then
			substring(ia.inetinfo, charindex(@tag3+'=', ia.inetinfo)+ len(@tag3)+1, charindex(char(13), ia.inetinfo, charindex(@tag3+'=', ia.inetinfo)) - charindex(@tag3+'=', ia.inetinfo) - len(@tag3)-1)
		else 'no existe tag: '+@tag3 end param3,
		CASE when charindex(@tag4+'=', ia.inetinfo) > 0 and  charindex(char(13), ia.inetinfo) > 0 then
			substring(ia.inetinfo, charindex(@tag4+'=', ia.inetinfo)+ len(@tag4)+1, charindex(char(13), ia.inetinfo, charindex(@tag4+'=', ia.inetinfo)) - charindex(@tag4+'=', ia.inetinfo) - len(@tag4)-1)
		else 'no existe tag: '+@tag4 end param4,
		CASE when charindex(@tag5+'=', ia.inetinfo) > 0 and  charindex(char(13), ia.inetinfo) > 0 then
			substring(ia.inetinfo, charindex(@tag5+'=', ia.inetinfo)+ len(@tag5)+1, charindex(char(13), ia.inetinfo, charindex(@tag5+'=', ia.inetinfo)) - charindex(@tag5+'=', ia.inetinfo) - len(@tag5)-1)
		else 'no existe tag: '+@tag5 end param5,
		CASE when charindex(@tag6+'=', ia.inetinfo) > 0 and  charindex(char(13), ia.inetinfo) > 0 then
			substring(ia.inetinfo, charindex(@tag6+'=', ia.inetinfo)+ len(@tag6)+1, charindex(char(13), ia.inetinfo, charindex(@tag6+'=', ia.inetinfo)) - charindex(@tag6+'=', ia.inetinfo) - len(@tag6)-1)
		else 'no existe tag: '+@tag6 end param6
	from SY01200 ia						--coInetAddress Dirección de la compañía
		CROSS join DYNAMICS..SY01500 ci	--sy_company_mstr 
		inner join sy00600 lm			--sy_location_mstr
		on ci.INTERID = DB_NAME()
		and lm.CMPANYID = ci.CMPANYID
		and lm.LOCATNID = ia.ADRSCODE
	where ia.Master_Type = 'CMP'
	and ia.Master_ID = ci.INTERID
	and ia.ADRSCODE = 'DCEM'
)
go


IF (@@Error = 0) PRINT 'Creación exitosa de la función: dcemFnParametros()'
ELSE PRINT 'Error en la creación de la función: dcemFnParametros()'
GO
-------------------------------------------------------------------------------
--select PARAM1
--from dcemFnParametros('NAUT', '-', '-', '-', '-', '-')
