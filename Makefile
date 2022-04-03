dotnet-test:
	pushd cms-transforms-c-sharp/CmsTransformTests && dotnet test && popd;

dotnet-pack:
	pushd cms-transforms-c-sharp/CmsTransformLibrary && dotnet pack CmsTransformLibrary.csproj -c Release && popd;

.PHONY: dotnet-test dotnet-pack