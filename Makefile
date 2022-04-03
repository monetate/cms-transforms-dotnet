dotnet-test:
	cd cms-transforms-c-sharp/CmsTransformTests && dotnet test && cd ../..;

dotnet-pack:
	cd cms-transforms-c-sharp/CmsTransformLibrary && dotnet pack CmsTransformLibrary.csproj -c Release && cd ../..;

.PHONY: dotnet-test dotnet-pack