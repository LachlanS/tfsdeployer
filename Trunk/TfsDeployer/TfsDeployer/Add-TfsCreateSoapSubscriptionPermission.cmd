set collection=%1
set username=%2
tfssecurity.exe /a+ EventSubscription $SUBSCRIPTION: CREATE_SOAP_SUBSCRIPTION n:%username% ALLOW /collection:%collection%
