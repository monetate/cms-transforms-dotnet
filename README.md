# cms-transforms-dotnet

test-c-sharp:
If required, install relevant packages
```
brew install mono-libgdiplus && brew install dotnet
```
then test using
```
cd cms-transforms-c-sharp/CmsTransformTests && dotnet test && cd ../..
```

# Setting up credentials: 
The config files for NuGet can be found in `~/.config/NuGet/NuGet.Config`

These values can be retrieved from Artifactory UI under “Set Me Up” making sure to choose dotnet-local as the repository
and NuGet as the package type.

Add Artifactory to your list of sources
```
nuget sources Add -Name Artifactory -Source https://monetate.jfrog.io/artifactory/api/nuget/v3/dotnet-local -username {username} -password {password}
```
Add the following line directly to the NuGet config file under `<packageSources>`
```
<add key="ArtifactoryNuGetV3" value="https://monetate.jfrog.io/artifactory/api/nuget/v3/dotnet-local" protocolVersion="3" />
```
Set apikey with Artifactory
```
nuget setapikey {username}:{apikey} -Source Artifactory
```


# Publishing to Artifactory (dev instructions): 

Generate a .nuspec file from the transforms-c-sharp/CmsTransformLibrary directory (the same directory as the .csproj file).
```
nuget spec
```

Make changes to the attributes of the newly created .nuspec file. These values can be copied over from the .csproj file.
The ID is the value used to download/index the project on Artifactory. It should ALWAYS be CmsTransformLibrary.
Increment the version number using appropriate versioning conventions (a.b.c: a=Major, b=Minor, c=Micro).
Pack the .nuspec file into a .nupkg which will then be placed into the current directory.
```
nuget pack CmsTransformLibrary.nuspec
```

Push your .nupkg file to Artifactory
```
nuget push CmsTransformLibrary.a.b.c.nupkg -Source ArtifactoryNuGetV3
```
Navigate to Artifactory dotnet-local UI to confirm the .nupkg has been uploaded