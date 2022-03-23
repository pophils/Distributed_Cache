
FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app 

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /KongoCacheBuild

COPY ./*.sln ./
COPY ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done 

COPY ./nuget.config .
COPY ./NugetPackages ./NugetPackages

RUN dotnet restore

COPY . .
 
RUN dotnet build

FROM build AS test
RUN dotnet test 

FROM build AS publish
RUN dotnet publish ./KongoCache.Worker/KongoCache.Worker.csproj -c Release -o /publish
RUN export s=1


FROM base AS final
WORKDIR /app
COPY --from=publish /publish .
ENTRYPOINT ["dotnet", "KongoCache.Worker.dll"]
