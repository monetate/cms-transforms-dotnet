#!/bin/bash

VERSION_PREFIX=$(cd cms-transforms-c-sharp/CmsTransformLibrary && grep '<Version>' < CmsTransformLibrary.csproj | sed 's/.*<Version>\(.*\)<\/Version>/\1/')
VERSION_PREFIX="$(echo "$VERSION_PREFIX" | tr -d '\r\n')"
SOURCE="CmsTransformLibrary.${VERSION_PREFIX}.nupkg"
USER = $1
PW = $2
API_KEY = '$USER:$PW'

dotnet nuget add source https://monetate.jfrog.io/artifactory/api/nuget/v3/dotnet-local -n Artifactory -u $USER -p $PW --store-password-in-clear-text

cd cms-transforms-c-sharp/CmsTransformLibrary/bin/Release/ && dotnet nuget push ${SOURCE} -s Artifactory -k $API_KEY && cd ../../../..