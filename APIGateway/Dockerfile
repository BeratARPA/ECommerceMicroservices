FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Projeyi kopyala
COPY ["APIGateway/APIGateway.csproj", "APIGateway/"]

# Bağımlılıkları yükle
RUN dotnet restore "APIGateway/APIGateway.csproj"

# Tüm kaynak kodları kopyala
COPY . .

# Uygulamayı derle
WORKDIR "/src/APIGateway"
RUN dotnet build "APIGateway.csproj" -c Release -o /app/build

# Yayınlama
FROM build AS publish
RUN dotnet publish "APIGateway.csproj" -c Release -o /app/publish

# Final imaj
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 42196
ENTRYPOINT ["dotnet", "APIGateway.dll"]