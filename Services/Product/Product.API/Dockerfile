FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Projeleri kopyala
COPY ["Services/Product/Product.API/Product.API.csproj", "Services/Product/Product.API/"]
COPY ["Services/Product/Product.Domain/Product.Domain.csproj", "Services/Product/Product.Domain/"]
COPY ["Services/Product/Product.Infrastructure/Product.Infrastructure.csproj", "Services/Product/Product.Infrastructure/"]
COPY ["ECommerce.Shared/ECommerce.Shared.csproj", "ECommerce.Shared/"]

# Bağımlılıkları yükle
RUN dotnet restore "Services/Product/Product.API/Product.API.csproj"

# Tüm kaynak kodları kopyala
COPY . .

# Uygulamayı derle
WORKDIR "/src/Services/Product/Product.API"
RUN dotnet build "Product.API.csproj" -c Release -o /app/build

# Yayınlama
FROM build AS publish
RUN dotnet publish "Product.API.csproj" -c Release -o /app/publish

# Final imaj
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 9001
ENTRYPOINT ["dotnet", "Product.API.dll"]