-------------------------------------------------------------------------------------------
--Permiso a usuarios Windows:
-------------------------------------------------------------------------------------------
use gmope; --TEST
EXEC sp_addrolemember 'rol_contaelectr', 'GILA\Ext-tiiselam4';
EXEC sp_addrolemember 'rol_contaelectr', 'GILA\tiiselam';
--EXEC sp_addrolemember 'rol_contaelectr', 'GILA\consultor';
EXEC sp_addrolemember 'rol_contaelectr', 'GILA\luis.flores';
exec sp_addrolemember 'rol_contaelectr', 'GILA\contador.mexico';
exec sp_addrolemember 'rol_contaelectr', 'GILA\mauricio.gomez';
exec sp_addrolemember 'rol_contaelectr', 'GILA\martha.chavez';

use dynamics;
EXEC sp_addrolemember 'dyngrp', 'GILA\Ext-tiiselam4';
EXEC sp_addrolemember 'rol_contaelectr', 'GILA\luis.flores';
exec sp_addrolemember 'rol_contaelectr', 'GILA\contador.mexico';
exec sp_addrolemember 'rol_contaelectr', 'GILA\mauricio.gomez';
exec sp_addrolemember 'rol_contaelectr', 'GILA\martha.chavez';
