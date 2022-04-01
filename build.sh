#!/bin/bash

yum install python
yum install awscli

ARTIFACTORY_TOKEN=$(aws s3 cp s3://secret-monetate-dev/artifactory/monetate.jfrog.io/dotnet-local/dotnet-local-pw -)
echo ${ARTIFACTORY_TOKEN}