FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Azro.Web/Azro.Web.csproj", "Azro.Web/"]
RUN dotnet restore "Azro.Web/Azro.Web.csproj"
COPY . .
WORKDIR "/src/Azro.Web"
RUN dotnet build "Azro.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Azro.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Azro.Web.dll"]