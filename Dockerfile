FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

RUN apt-get update -y && apt-get install -y python awscli