#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["ValidateCodeApi/ValidateCodeApi.csproj", "ValidateCodeApi/"]
COPY ["ValidateCode.Core/ValidateCode.Core.csproj", "ValidateCode.Core/"]
COPY ["ValidateCodeRepository/ValidateCodeRepository.csproj", "ValidateCodeRepository/"]
RUN dotnet restore "ValidateCodeApi/ValidateCodeApi.csproj"
COPY . .
WORKDIR "/src/ValidateCodeApi"
RUN dotnet build "ValidateCodeApi.csproj" -c ${BuildConfiguration} -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "ValidateCodeApi.csproj" -c ${BuildConfiguration} -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ValidateCodeApi.dll"]