#!/bin/bash

VERSION_PREFIX=$(cd cms-transforms-c-sharp/CmsTransformLibrary && grep '<Version>' < CmsTransformLibrary.csproj | sed 's/.*<Version>\(.*\)<\/Version>/\1/')
VERSION_PREFIX="$(echo "$VERSION_PREFIX"|tr -d '\n')"
ARTIFACTORY_UNAME=$(aws s3 cp s3://secret-monetate-dev/artifactory/monetate.jfrog.io/dotnet-local/dotnet-local-user -)
ARTIFACTORY_PW=$(aws s3 cp s3://secret-monetate-dev/artifactory/monetate.jfrog.io/dotnet-local/dotnet-local-upw -)
SOURCE="CmsTransformLibrary.${VERSION_PREFIX}.nupkg"
API_KEY="${ARTIFACTORY_UNAME}:${ARTIFACTORY_PW}"

NUGET_HAS_SOURCE=$(dotnet nuget list source | grep 'Artifactory')
if [[ -z "$NUGET_HAS_SOURCE" ]] # if NuGet doesn't have Artifactory Source
then
    dotnet nuget add source https://monetate.jfrog.io/artifactory/api/nuget/v3/dotnet-local -n Artifactory -u ${ARTIFACTORY_UNAME} -p ${ARTIFACTORY_PW} --store-password-in-clear-text
fi

cd cms-transforms-c-sharp/CmsTransformLibrary/bin/Release/ && dotnet nuget push ${SOURCE} -s Artifactory -k ${API_KEY} && cd ../../../..