# this script merely demonstrates the information available to a script run by Tfs Deployer

# This is the TFS 2008 build information structure:
$TfsDeployerBuildDetail | Format-List -Property * ;

$tmp = @" # this is the output you can expect to see from the above command:
BuildServer             : Microsoft.TeamFoundation.Build.Client.BuildServer
BuildAgent              : BuildAgent instance 28589617
                            MachineName: buildserver.mydomain.local
                            Port: 9191
                            RequireSecureChannel: False
                            MaxProcesses: 1
                            BuildDirectory: D:\Building\$(BuildDefinitionPath)
                            Status: Enabled
                            StatusMessage: Started building \MyTeamProject\MyBuildType\MyBuildType_20080130.6 on 30/01/2008 7:10:09 PM
                            QueueCount: 1
                            Description: 
                            Uri: vstfs:///Build/Agent/5
                            FullPath: \MyTeamProject\buildserver.mydomain.local
                          
BuildDefinition         : BuildDefinition instance 51182882
                            DefaultBuildAgentUri: vstfs:///Build/Agent/5
                            DefaultDropLocation: \\server\share\MyDropFolder
                            ConfigurationFolderUri: vstfs:///VersionControl/LatestItemVersion/29685
                            MaxTimeout: 0
                            ContinuousIntegrationType: Batch
                            ContinuousIntegrationQuietPeriod: 0
                            LastBuildUri: vstfs:///Build/Build/867
                            LastGoodBuildUri: vstfs:///Build/Build/866
                            LastGoodBuildLabel: MyBuildType_20080130.4@$/MyTeamProject
                            Enabled: True
                            Description: 
                            RetentionPolicies: [4]RetentionPolicy instance 14718474
                            BuildStatus: Failed
                            NumberToKeep: 2
                          , RetentionPolicy instance 7712661
                            BuildStatus: Stopped
                            NumberToKeep: 1
                          , RetentionPolicy instance 16933032
                            BuildStatus: Succeeded
                            NumberToKeep: 5
                          , RetentionPolicy instance 22542438
                            BuildStatus: PartiallySucceeded
                            NumberToKeep: 5
                          
                            Schedules: [0]
                            WorkspaceTemplate: WorkspaceTemplate instance 28763840
                            DefinitionUri: vstfs:///Build/Definition/13
                            InternalMappings: [1]WorkspaceMapping instance 57629153
                            ServerItem: $/MyTeamProject
                            LocalItem: $(SourceDir)
                            MappingType: Map
                            Depth: 120
                          
                            LastModifiedDate: 20/12/2007 4:02:24 PM
                            LastModifiedBy: MYDOMAIN\MyUser
                          
                            Id: 0
                            Uri: vstfs:///Build/Definition/13
                            FullPath: \MyTeamProject\MyBuildType
                          
BuildFinished           : True
ConfigurationFolderPath : $/MyTeamProject/TeamBuildTypes/MyBuildType
Information             : Microsoft.TeamFoundation.Build.Client.BuildInformation
Uri                     : vstfs:///Build/Build/867
BuildNumber             : MyBuildType_20080130.5
BuildDefinitionUri      : vstfs:///Build/Definition/13
CommandLineArguments    : 
StartTime               : 30/01/2008 5:05:21 PM
FinishTime              : 30/01/2008 5:05:21 PM
Status                  : Passed
Quality                 : Rejected
CompilationStatus       : Unknown
TestStatus              : Unknown
DropLocation            : \\server\share\MyDropFolder\MyBuildType_20080130.5
LogLocation             : \\server\share\MyDropFolder\MyBuildType_20080130.5\BuildLog.txt
BuildAgentUri           : vstfs:///Build/Agent/5
ConfigurationFolderUri  : vstfs:///VersionControl/VersionedItem/MyTeamProject%25252fTeamBuildTypes%25252fMyBuildType%2526changesetVersion%253d10148%2526deletionId%253d0
SourceGetVersion        : C10348
RequestedFor            : MYDOMAIN\MyUser
RequestedBy             : Build System Account
LastChangedOn           : 30/01/2008 7:10:39 PM
LastChangedBy           : MYDOMAIN\MyUser
KeepForever             : False
LabelName               : 
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


