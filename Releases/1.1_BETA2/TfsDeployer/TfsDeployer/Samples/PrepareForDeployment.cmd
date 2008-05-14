@rem %0 returns the full patch of the command script (ie C:\TEMP\PrepareForDeployment.cmd):
@echo 0: %0

@rem %1 returns the drop folder of the build (ie \\server\share\MyDropFolder\MyBuildType_20080130.5):
@echo 1: %1

@rem %2 returns the build number (ie MyBuildType_20080130.5):
@echo 2: %2

@rem %3 returns the value of the first ScriptParameter if defined (ie ProdSvr1):
@echo 3: %3
