# TaskFlow - SaaS MVP (Task & Team Management)

TaskFlow, bireysel ve takım görev yönetimi için geliştirilmiş bir **SaaS MVP** örneğidir. Bu proje, **plan bazlı özellik yönetimi**, **takım/kişisel görev limiti**, **deadline hatırlatma**, **görev önceliklendirme** ve **bildirim sistemleri** gibi temel SaaS mantıklarını deneyimlemek amacıyla geliştirilmiştir. Proje sadece **deneyim ve CV amaçlıdır**; üretim ortamında kullanılmamaktadır.

---

## 🔹 Planlar ve Özellikler

3 farklı plan mevcuttur: **Free, Standard, Premium**  

- **Free Plan**
  - Temel görev yönetimi  
  - Sohbet
  - Kullanıcı ve takım limiti
  - Departman Yönetimi  

- **Standard Plan**
  - Görev önceliklendirme / kategori  
  - Deadline hatırlatma
  - Bireysel / Grup Sohbet
  - Kullanıcı Takım Limiti
  - Departman Yönetimi  

- **Premium Plan**
  - Tüm Standard özellikler  
  - Görev eklendiğinde bildirim  
  - Gelişmiş takım ve kullanıcı limiti
  - Günlük Görev Planlayıcı
  - Bireysel / Grup Sohbet
  - Departman Yönetimi  

---

## 🔹 Kullanılan Teknolojiler

- **Backend:** .NET 10, C# 14  
- **Veri Tabanı:** PostgreSQL, Entity Framework Core  
- **Gerçek Zamanlı Bildirim:** SignalR  
- **Mesaj Kuyruğu / Asenkron İşlem:** RabbitMQ, DotNetCore.CAP
- **Cache / Hızlandırma:** Redis  
- **Containerization:** Docker  
- **SaaS Mantığı:** Plan bazlı feature toggle ve CompanyPlan yapısı  

---

## 🔹 Notlar
- Premium özellikler ve SaaS mantığı simüle edilmiştir

---

## Local Docker Compose Secrets

This project no longer uses a root `.env` file for runtime secrets.
Create the following files under `secrets/` before running Docker Compose:

- `secrets/postgres_user`
- `secrets/postgres_password`
- `secrets/rabbitmq_user`
- `secrets/rabbitmq_password`
- `secrets/redis_password`
- `secrets/stripe_secret_key`
- `secrets/jwt_secret_key`

Then start services:

```bash
docker compose up --build
```
