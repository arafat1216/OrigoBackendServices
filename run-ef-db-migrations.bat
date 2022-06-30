@echo off
title Running EF DB-migrations
set rootpath=%cd%
cls
@echo.

@echo ===============================================================
@echo Running EF migrations for 'AssetServices'
@echo ===============================================================
cd "%rootpath%\src\Services\Asset\AssetServices"
@echo.
@echo [ AssetContext ]
dotnet ef database update --context AssetsContext
if %ERRORLEVEL% == 1 goto Error
:: if %ERRORLEVEL% == 1 echo Exit code of previous command/application is 1.
:: if %ERRORLEVEL% == 0 echo Exit code of previous command/application is 0.
@echo.
@echo [ LoggingDbContext ]
dotnet ef database update --context LoggingDbContext
if %ERRORLEVEL% == 1 goto Error
echo.

@echo ===============================================================
@echo Running EF migrations for 'CustomerServices'
@echo ===============================================================
cd "%rootpath%\src\Services\Customer\CustomerServices"
@echo.
@echo [ CustomerContext ]
dotnet ef database update --context CustomerContext
if %ERRORLEVEL% == 1 goto Error
@echo.
@echo [ LoggingDbContext ]
dotnet ef database update --context LoggingDbContext
if %ERRORLEVEL% == 1 goto Error
echo.

@echo ===============================================================
@echo Running EF migrations for 'HardwareServiceOrderServices'
@echo ===============================================================
cd "%rootpath%\src\Services\HardwareServiceOrder\HardwareServiceOrderServices"
@echo.
@echo [ CustomerContext ]
dotnet ef database update --context HardwareServiceOrderContext
if %ERRORLEVEL% == 1 goto Error
@echo.

@echo ===============================================================
@echo Running EF migrations for 'ProductCatalog'
@echo ===============================================================
cd "%rootpath%\src\Services\ProductCatalog\ProductCatalog.Infrastructure"
@echo.
@echo [ ProductCatalogContext ]
dotnet ef database update --context ProductCatalogContext
if %ERRORLEVEL% == 1 goto Error
@echo.

@echo ===============================================================
@echo Running EF migrations for 'SubscriptionManagementServices'
@echo ===============================================================
cd "%rootpath%\src\Services\SubscriptionManagement\SubscriptionManagementServices"
@echo.
@echo [ SubscriptionManagementContext ]
dotnet ef database update --context SubscriptionManagementContext
if %ERRORLEVEL% == 1 goto Error
@echo.
@echo [ LoggingDbContext ]
dotnet ef database update --context LoggingDbContext
if %ERRORLEVEL% == 1 goto Error
echo.


:Exit
@echo.
pause
exit


:Error
@echo.
@echo.
@echo ===============================================================
@echo The DB migration failed. Aborting the run!
@echo ===============================================================
@echo.
goto Exit