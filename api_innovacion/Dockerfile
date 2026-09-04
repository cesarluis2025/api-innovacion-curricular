FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

# Copiamos solo el .csproj primero para que Docker cachee la restauración
# de paquetes y no la repita cada vez que cambias un .cs
COPY *.csproj ./
RUN dotnet restore

COPY . .

EXPOSE 8080

# dotnet watch: cuando guardas un .cs en tu editor, recompila y reinicia
# la API sola (el código está montado como volumen, ver docker-compose.yml)
CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:8080", "--no-launch-profile"]
