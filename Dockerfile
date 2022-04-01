FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

RUN yum install -y python awscli