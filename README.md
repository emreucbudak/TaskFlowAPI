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
