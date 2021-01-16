FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine3.11
WORKDIR /APP

FROM correia97/netcoresdksonar:3.1-alpine3.12 as build
WORKDIR /APP

COPY src/LogSample.Model/LogSample.Model.csproj ./LogSample.Model/

COPY src/Serilog/APICore/APICore.csproj ./Serilog/APICore/

RUN dotnet restore ./Serilog/APICore/APICore.csproj

COPY src/ .

RUN dotnet build ./Serilog/APICore/APICore.csproj -c Release
RUN dotnet publish ./Serilog/APICore/APICore.csproj -c Release -o out/

COPY  --from=build ./APP/out .

ENV ASPNETCORE_ENVIRONMENT=docker
# Define qual o executavel do container
ENTRYPOINT [ "dotnet","APICore.dll"  ]