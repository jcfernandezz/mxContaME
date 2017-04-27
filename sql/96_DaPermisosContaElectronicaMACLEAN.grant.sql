-------------------------------------------------------------------------------------------
--Permiso a usuarios Windows:
-------------------------------------------------------------------------------------------
use ZMMEX; --TEST
EXEC sp_addrolemember 'rol_contaelectr', 'MEMCO-DS01\GPCustomization';
EXEC sp_addrolemember 'rol_contaelectr', 'MEMCO-DS01\tilselam';

use dynamics;
EXEC sp_addrolemember 'dyngrp', 'MEMCO-DS01\GPCustomization';
EXEC sp_addrolemember 'dyngrp', 'MEMCO-DS01\tilselam';

