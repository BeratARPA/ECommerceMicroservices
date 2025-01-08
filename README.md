# E-Ticaret Mikroservis Mimarisi

Bu proje, mikroservis mimarisi kullanılarak geliştirilmiş basit bir e-ticaret uygulamasıdır. Proje, ürün ve sipariş yönetimi için iki ana mikroservis içerir.

## Teknolojiler

- .NET 8.0
- Entity Framework Core
- Redis (Önbellekleme)
- RabbitMQ (Mesaj Kuyruğu)
- Docker
- SQL Server
- Ocelot (API Gateway)

## Mimari Yapı

Proje aşağıdaki bileşenlerden oluşur:

- **API Gateway (Port: 42196)**
- **Order Service (Port: 9000)**
- **Product Service (Port: 9001)**
- **Redis Cache**
- **SQL Server**
- **RabbitMQ**

## Başlangıç

Projeyi çalıştırmak için:

bash
docker-compose up -d

## API Endpointleri

### Ürün Servisi (Product API)

Base URL: `http://localhost:42196/products`

#### Ürün İşlemleri

1. **Tüm Ürünleri Listele**
   ```
   GET /product
   ```

2. **Ürün Detayı**
   ```
   GET /product/{id}
   ```

3. **Yeni Ürün Ekle**
   ```
   POST /product
   ```
   Request Body:
   ```json
   {
     "name": "Ürün Adı",
     "stock": 100,
     "price": 29.99
   }
   ```

4. **Stok Güncelle**
   ```
   PUT /product/{id}
   ```
   Request Body:
   ```json
   {
     "stock": 150
   }
   ```

5. **Ürün Sil**
   ```
   DELETE /product/{id}
   ```

### Sipariş Servisi (Order API)

Base URL: `http://localhost:42196/orders`

#### Sipariş İşlemleri

1. **Tüm Siparişleri Listele**
   ```
   GET /order
   ```

2. **Sipariş Detayı**
   ```
   GET /order/{id}
   ```

3. **Yeni Sipariş Oluştur**
   ```
   POST /order
   ```
   Request Body:
   ```json
   {
     "productId": 1,
     "quantity": 2
   }
   ```

4. **Sipariş İptal**
   ```
   DELETE /order/{id}
   ```

## Özellikler

- **Önbellekleme**: Redis kullanılarak ürün ve sipariş verileri önbelleğe alınır
- **Event-Driven**: RabbitMQ ile servisler arası iletişim
- **API Gateway**: Ocelot ile tek noktadan erişim
- **Containerization**: Docker ile kolay dağıtım
- **Unit Testing**: Sipariş servisi için birim testler

## Servis İletişimi

- Sipariş oluşturulduğunda, ürün stoku otomatik olarak güncellenir
- Sipariş iptal edildiğinde, ürün stoku iade edilir
- Tüm servis iletişimleri RabbitMQ üzerinden event-driven olarak gerçekleşir

## Notlar

- Tüm servisler Docker container'ları içinde çalışır
- Her servis kendi veritabanına sahiptir
- Redis önbellekleme 10 dakika süreyle veri saklar
- API Gateway üzerinden tüm servislere tek bir noktadan erişilebilir

## Geliştirme

Projeyi geliştirmek için:

1. Repository'yi klonlayın
2. Docker Desktop'ı yükleyin
3. Root dizinde `docker-compose up -d` komutunu çalıştırın
