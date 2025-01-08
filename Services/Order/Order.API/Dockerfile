# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Projeleri kopyala
COPY ["Services/Order/Order.API/Order.API.csproj", "Services/Order/Order.API/"]
COPY ["Services/Order/Order.Domain/Order.Domain.csproj", "Services/Order/Order.Domain/"]
COPY ["Services/Order/Order.Infrastructure/Order.Infrastructure.csproj", "Services/Order/Order.Infrastructure/"]
COPY ["Services/Order/Order.Tests/Order.Tests.csproj", "Services/Order/Order.Tests/"]
COPY ["ECommerce.Shared/ECommerce.Shared.csproj", "ECommerce.Shared/"]

# Bağımlılıkları yükle
RUN dotnet restore "Services/Order/Order.API/Order.API.csproj"
RUN dotnet restore "Services/Order/Order.Tests/Order.Tests.csproj"

# Tüm kaynak kodları kopyala
COPY . .

# Testleri çalıştır
RUN dotnet test "Services/Order/Order.Tests/Order.Tests.csproj" --configuration Release

# Uygulamayı derle
WORKDIR "/src/Services/Order/Order.API"
RUN dotnet build "Order.API.csproj" -c Release -o /app/build

# Yayınlama
FROM build AS publish
RUN dotnet publish "Order.API.csproj" -c Release -o /app/publish

# Final imaj
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 9000
ENTRYPOINT ["dotnet", "Order.API.dll"]