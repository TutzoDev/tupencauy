# Stage 1: Build the .NET MVC project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar el archivo de proyecto y restaurar las dependencias
COPY tupencauy.csproj ./
RUN dotnet restore

# Copiar el resto de los archivos y construir la aplicación
COPY . ./
WORKDIR /app
RUN dotnet publish -c Release -o /out

# Stage 2: Create final image with runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

# Expose ports
EXPOSE 15427
EXPOSE 44350

# Entrypoint
CMD ["dotnet", "tupencauy.dll"]
