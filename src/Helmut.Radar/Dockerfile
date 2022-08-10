#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5002
EXPOSE 445

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Helmut.Radar/Helmut.Radar.csproj", "src/Helmut.Radar/"]
COPY ["src/Helmut.General/Helmut.General.csproj", "src/Helmut.General/"]
RUN dotnet restore "src/Helmut.Radar/Helmut.Radar.csproj"
COPY . .
WORKDIR "/src/src/Helmut.Radar"
RUN dotnet build "Helmut.Radar.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Helmut.Radar.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Helmut.Radar.dll"]