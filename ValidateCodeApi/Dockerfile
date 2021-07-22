#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
RUN dotnet nuget remove source 'Microsoft Visual Studio Offline Packages' || true
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/Apis/ValidateCodeApi/ValidateCodeApi.csproj", "src/Apis/ValidateCodeApi/"]
COPY ["src/Services/ValidateCode.Core/ValidateCode.Core.csproj", "src/Services/ValidateCode.Core/"]
COPY ["src/Repositories/ValidateCodeRepository/ValidateCodeRepository.csproj", "src/Repositories/ValidateCodeRepository/"]
COPY ["src/Core/Infrastructures/Infrastructures.csproj", "src/Core/Infrastructures/"]
COPY ["src/Core/Models/Models.csproj", "src/Core/Models/"]
COPY ["src/Core/DataServices.Base/DataServices.Base.csproj", "src/Core/DataServices.Base/"]
RUN dotnet restore "src/Apis/ValidateCodeApi/ValidateCodeApi.csproj"
COPY . .
WORKDIR "/src/src/Apis/ValidateCodeApi"
RUN dotnet build "ValidateCodeApi.csproj" -c ${BuildConfiguration} -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "ValidateCodeApi.csproj" -c ${BuildConfiguration} -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ValidateCodeApi.dll"]