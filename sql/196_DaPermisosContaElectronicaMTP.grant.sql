-------------------------------------------------------------------------------------------
--Permiso a usuarios Windows:
-------------------------------------------------------------------------------------------
use MTP7 ; --TEST
EXEC sp_addrolemember 'rol_contaelectr', 'MTP\Localization Mexico';
--EXEC sp_addrolemember 'rol_contaelectr', 'MTP\Alejandro.Becker';
EXEC sp_addrolemember 'rol_contaelectr', 'MTP\Juan.Fernandez';

use dynamics;
EXEC sp_addrolemember 'dyngrp', 'MTP\Localization Mexico';
--EXEC sp_addrolemember 'dyngrp', 'MTP\Alejandro.Becker';
EXEC sp_addrolemember 'dyngrp', 'MTP\Juan.Fernandez';

