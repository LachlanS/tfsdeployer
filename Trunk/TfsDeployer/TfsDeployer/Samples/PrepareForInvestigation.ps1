# this script merely demonstrates the information available to a script run by Tfs Deployer

# This is the TFS 2008 build information structure:
$TfsDeployerBuildDetail | Format-List -Property * ;

$tmp = @" # this is the output you can expect to see from the above command:
BuildDefinition         : BuildDefinition instance 51182882
                            DefaultDropLocation: \\server\share\MyDropFolder
                            Description: 
                            Enabled: True
                            Id: 0
                            LastBuildUri: vstfs:///Build/Build/867
                            LastGoodBuildLabel: MyBuildType_20080130.4@$/MyTeamProject
                            LastGoodBuildUri: vstfs:///Build/Build/866
                          
BuildDefinitionUri      : vstfs:///Build/Definition/13
BuildFinished           : True
BuildNumber             : MyBuildType_20080130.5
DropLocation            : \\server\share\MyDropFolder\MyBuildType_20080130.5
FinishTime              : 30/01/2008 5:05:21 PM
KeepForever             : False
LabelName               : 
LastChangedBy           : MYDOMAIN\MyUser
LastChangedOn           : 30/01/2008 7:10:39 PM
LogLocation             : \\server\share\MyDropFolder\MyBuildType_20080130.5\BuildLog.txt
Quality                 : Rejected
RequestedBy             : Build System Account
RequestedFor            : MYDOMAIN\MyUser
SourceGetVersion        : C10348
StartTime               : 30/01/2008 5:05:21 PM
Uri                     : vstfs:///Build/Build/867
"@

# This is the old TFS 2005 obsoleted structure (still supported):
$TfsDeployerBuildData | Format-List -Property * ;

$tmp = @" # this is the output you can expect to see from the above command:
BuildMachine     : buildserver.mydomain.local
BuildNumber      : MyBuildType_20080130.5
BuildQuality     : Rejected
BuildStatus      : Passed
BuildStatusId    : 8
BuildType        : MyBuildType
BuildTypeFileUri : vstfs:///Build/Definition/13
BuildUri         : vstfs:///Build/Build/867
DropLocation     : \\server\share\MyDropFolder\MyBuildType_20080130.5
FinishTime       : 30/01/2008 5:05:21 PM
GoodBuild        : False
LastChangedBy    : MYDOMAIN\MyUser
LastChangedOn    : 30/01/2008 7:10:39 PM
LogLocation      : 
RequestedBy      : Build System Account
StartTime        : 30/01/2008 5:05:21 PM
TeamProject      : MyTeamProject
"@


