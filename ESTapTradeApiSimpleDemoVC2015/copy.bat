xcopy ..\XTA_W32\Api\ESTap_v9.0.3.11 Release\XTA_W32\Api\ESTap_v9.0.3.11 /I /E /Y
copy ..\XTA_W32\Cpp\XFinApi.ITradeApi.dll Release\XFinApi.ITradeApi.dll /Y

xcopy ..\XTA_W32\Api\ESTap_v9.0.3.11 Debug\XTA_W32\Api\ESTap_v9.0.3.11 /I /E /Y
copy ..\XTA_W32\Cpp\XFinApi.ITradeApid.dll Debug\XFinApi.ITradeApid.dll /Y

pause