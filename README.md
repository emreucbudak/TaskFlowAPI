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
- `secrets/google_api_key`
- `secrets/jwt_secret_key`

Each file should contain only the raw secret value, for example:

```text
sk_test_your_stripe_secret_key
```

PowerShell example:

```powershell
New-Item -ItemType Directory -Path .\secrets -Force | Out-Null
Set-Content -Path .\secrets\stripe_secret_key -Value 'sk_test_your_stripe_secret_key' -NoNewline
```

Docker Compose mounts these files into the container as `/run/secrets/<secret_name>`.
For example, `./secrets/stripe_secret_key` becomes `/run/secrets/stripe_secret_key`.

To add a new file-based secret:

1. Create a file under `secrets/` with the secret name.
2. Add the secret under the top-level `secrets:` section in `docker-compose.yml`.
3. Attach that secret to the service under `services.<service>.secrets`.
4. Read it in the app from `/run/secrets/<secret_name>` or via `AddKeyPerFile("/run/secrets", optional: true)`.

Example:

```yaml
services:
  taskflow.presentation:
    secrets:
      - my_custom_secret

secrets:
  my_custom_secret:
    file: ./secrets/my_custom_secret
```

Then start services:

```bash
docker compose up --build
```
