# Stage 1: Build the .NET projects
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar archivos de proyecto y restaurar dependencias
COPY tupencauy/tupencauy.csproj ./tupencauy/
COPY tupencauywebapi/tupencauywebapi.csproj ./tupencauywebapi/
RUN dotnet restore ./tupencauy/tupencauy.csproj
RUN dotnet restore ./tupencauywebapi/tupencauywebapi.csproj

# Copiar el resto de los archivos y construir las aplicaciones
COPY . .
RUN dotnet publish ./tupencauy/tupencauy.csproj -c Release -o /out/tupencauy
RUN dotnet publish ./tupencauywebapi/tupencauywebapi.csproj -c Release -o /out/tupencauywebapi

# Stage 2: Create final image with runtime environment for MVC
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime-mvc
WORKDIR /app
COPY --from=build /out/tupencauy ./
EXPOSE 80
EXPOSE 443
CMD ["dotnet", "tupencauy.dll"]

# Stage 3: Create final image with runtime environment for WebAPI
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime-webapi
WORKDIR /app
COPY --from=build /out/tupencauywebapi ./
EXPOSE 5234
CMD ["dotnet", "tupencauywebapi.dll"]
