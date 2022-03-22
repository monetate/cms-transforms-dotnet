.PHONY: nuget-spec nuget-pack nuget-push test-dotnet

nuget-spec:
	pushd cms-transforms-c-sharp/CmsTransformLibrary && nuget spec && popd;

nuget-pack:
	pushd cms-transforms-c-sharp/CmsTransformLibrary && nuget pack CmsTransformLibrary.csproj && popd;

nuget-push:
	pushd cms-transforms-c-sharp/CmsTransformLibrary && nuget push CmsTransformLibrary.1.0.0.nupkg -Source ArtifactoryNugetV3 && popd;

test-dotnet:
	pushd cms-transforms-c-sharp/CmsTransformTests && dotnet test && popd;