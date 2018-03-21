-------------------------------------------------------------------------------------------
--Permiso a usuarios Windows:
-------------------------------------------------------------------------------------------
use MEX10; --TEST
EXEC sp_addrolemember 'rol_contaelectr', 'GILA\Ext-tiiselam4';
EXEC sp_addrolemember 'rol_contaelectr', 'GILA\tiiselam';
--EXEC sp_addrolemember 'rol_contaelectr', 'GILA\consultor';

exec sp_addrolemember 'rol_contaelectr', 'GILA\contador.mexico';
exec sp_addrolemember 'rol_contaelectr', 'GILA\mauricio.gomez';
exec sp_addrolemember 'rol_contaelectr', 'GILA\martha.chavez';
exec sp_addrolemember 'rol_contaelectr', 'GILA\ext-delrio';


use dynamics;
EXEC sp_addrolemember 'dyngrp', 'GILA\Ext-tiiselam4';

exec sp_addrolemember 'rol_contaelectr', 'GILA\contador.mexico';
exec sp_addrolemember 'rol_contaelectr', 'GILA\mauricio.gomez';
exec sp_addrolemember 'rol_contaelectr', 'GILA\martha.chavez';
exec sp_addrolemember 'rol_contaelectr', 'GILA\ext-delrio';
