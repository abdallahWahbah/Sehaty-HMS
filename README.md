# 🏥 Sehaty (صحتك) - Hospital Management System (HMS)

Sehaty (صحتك) is a modern, comprehensive **Hospital Management System (HMS)** built with a secure **.NET 8 Clean Architecture** backend and a dynamic **Angular 19** frontend. The system is designed to streamline healthcare workflows by connecting Patients, Doctors, Receptionists, and Admins while leveraging advanced **AI features (via OpenAI GPT-4o)**, **online payments (Paymob)**, and **SMS/Email notifications (Twilio & SMTP)**.

---

## 🌐 Language Options / لغات القراءة
* [English Version](#english-documentation)
* [النسخة العربية (Arabic Version)](#النسخة-العربية)

---

# English Documentation

## 🚀 Key Features

### 👤 Role-Based Portals

#### 1. Patient Portal
* **AI Symptom Analyzer:** Input symptoms and receive instant AI recommendations pointing to the correct medical specialty/department.
* **Appointment Booking:** Seamlessly book appointments based on department, doctor availability, and specific slots.
* **Digital Wallet & Payments:** Load funds or pay for consultations using **Paymob Egypt** (Credit/Debit Card & Mobile Wallets).
* **AI Prescription Breakdown:** Get a simplified, user-friendly explanation of prescribed medications, active ingredients, and common side effects.
* **Alternative Medicine Finder:** Suggests local Egyptian alternative medications (same active ingredients, typically cheaper) using AI.
* **Medical Records & Prescriptions:** View and download complete medical history, lab results, and prescription documents.
* **Doctor Feedback:** Rate and review doctors after appointments.

#### 2. Doctor Portal
* **Availability Management:** Define, update, and manage working days and appointment slots.
* **Appointment Handling:** View daily schedules, accept/manage patient bookings, and log diagnostics.
* **AI Patient History Summarizer:** Generate a professional medical summary summarizing long-term patient records, trends, and chronic issues using AI.
* **Electronic Prescriptions:** Write prescriptions detailing medication, dosage, frequency, and duration.
* **Medical Record Management:** Log patient symptoms, diagnoses, vital signs (BP, temperature, heart rate, weight), and treatment plans.

#### 3. Receptionist Portal
* **Walk-in Registrations:** Register patients and manage physical queues.
* **Appointment Check-ins:** Update appointment statuses as patients arrive.
* **Manual Booking:** Assist patients in booking appointments directly at the hospital.

#### 4. Admin Portal
* **System Dashboard:** Track key performance indicators (KPIs), active users, departments, and total billing.
* **User & Staff Management:** Create, update, and disable accounts for doctors, patients, receptionists, and admins.
* **Department Management:** Add or edit hospital departments (e.g., Cardiology, Neurology, Pediatrics).
* **Billing & Transaction Audit:** Monitor all transactions, wallet loads, and paid invoices.
* **Audit Logging:** View historical changes made to appointments and medical records for full transparency.

---

## 🤖 OpenAI GPT-4o Integrations
Sehaty integrates OpenAI to act as a smart medical assistant:
1. **Prescription Explanation:** Translates complex pharmaceutical notes into easy-to-read instructions in Arabic.
2. **Symptom Router:** Maps user symptoms to relevant departments (e.g., "chest pain" -> Cardiology).
3. **Alternatives Engine:** Proposes local Egyptian generic alternatives for costly brands.
4. **Clinical Summarizer:** Scans years of a patient's medical history to present a concise summary for doctors before a checkup.

---

## 🛠️ Technology Stack

### Backend
* **Framework:** ASP.NET Core 8.0 Web API
* **Architecture:** Domain-Driven Clean Architecture (API, Application, Core, Infrastructure)
* **Data Access:** Entity Framework Core (EF Core) with Repository & Unit of Work patterns
* **Database:** Microsoft SQL Server
* **Security:** ASP.NET Core Identity (JWT token validation + Refresh tokens)
* **API Documentation:** Swagger / OpenAPI

### Frontend
* **Framework:** Angular 19.2 (TypeScript)
* **Rendering:** Server-Side Rendering (SSR) for fast loading and SEO compliance
* **UI Components:** PrimeNG (version 19) & PrimeIcons
* **Styling:** Bootstrap 5, FontAwesome 7, and Custom CSS/SCSS

---

## 📁 Project Structure

```text
Sehaty-HMS/
├── Back-end/
│   └── Sehaty.Solution/
│       ├── Sehaty.APIs/          # Presentation Layer (Controllers, Middlewares, Extensions)
│       ├── Sehaty.Application/   # Business Logic, DTOs, Services, AutoMapper Profiles
│       ├── Sehaty.Core/          # Domain Entities, Interfaces, Specifications
│       ├── Sehaty.Infrastructure/# Data Context, Repositories, EF Migrations, Configurations
│       └── Sehaty.Solution.slnx  # Visual Studio Solution File
├── Front-end/
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/             # Guards, Interceptors, Services
│   │   │   ├── features/         # Module-based features (admin, doctor, patient, reception)
│   │   │   ├── layout/           # Shared layout components (navbars, footers)
│   │   │   └── pages/            # Static & informational pages (landing page, not-found)
│   │   └── index.html
│   ├── angular.json
│   └── package.json
```

---

## ⚙️ Configuration & Setup

### 1. Prerequisites
* [.NET SDK 8.0+](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Node.js v18+](https://nodejs.org/)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/)

### 2. Backend Configuration
Navigate to `Back-end/Sehaty.Solution/Sehaty.APIs/appsettings.json` and configure:
```json
{
  "ConnectionStrings": {
    "Sehaty": "Data Source=YOUR_SERVER;Initial Catalog=Sehaty;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"
  },
  "Jwt": {
    "Key": "YOUR_SUPER_SECURE_JWT_SECRET_KEY_HERE",
    "Issuer": "https://localhost:7086",
    "Audience": "https://localhost:7086"
  },
  "EmailSettings": {
    "FromEmail": "your-email@gmail.com",
    "Password": "your-app-password",
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587"
  },
  "TwilioSMSSetting": {
    "AccountSID": "TWILIO_ACCOUNT_SID",
    "AuthToken": "TWILIO_AUTH_TOKEN",
    "TwilioPhoneNumber": "+TWILIO_NUMBER"
  },
  "OpenAI": {
    "ApiKey": "YOUR_OPENAI_API_KEY_HERE"
  },
  "PaymobEgy2Settings": {
    "PublicKey": "PAYMOB_PUBLIC_KEY",
    "CardIntegrationId": 5404105,
    "WalletIntegrationId": 5409135,
    "AccountHMAC": "PAYMOB_HMAC"
  }
}
```

### 3. Database Initialization & Seeding
From the `Back-end/Sehaty.Solution/` directory, open your terminal and run:
```bash
# Apply EF Core migrations and generate the database
dotnet ef database update --project Sehaty.Infrastructure --startup-project Sehaty.APIs
```
*Note: The application includes a self-seeding mechanism. When launched for the first time, it automatically reads configuration data (roles, departments, initial doctors, patients, and users) from JSON files in the Infrastructure project and seeds your database.*

### 4. Running the Backend
From `Back-end/Sehaty.Solution/Sehaty.APIs/` run:
```bash
dotnet run
# OR
dotnet watch run
```
The API documentation will be available at `https://localhost:7086/swagger/index.html`.

### 5. Running the Frontend
Navigate to the `Front-end/` folder and run:
```bash
# Install dependencies
npm install

# Start the local development server
npm start
```
Open your browser and navigate to `http://localhost:4200/`.

---

## 🔑 Demo & Test Credentials
To test the various portals, you can log in with the following seeded accounts. The default password for all seeded users is **`P@ssw0rd`**:

| Role | Email | Password |
| :--- | :--- | :--- |
| **Admin** | `admin@example.com` | `P@ssw0rd` |
| **Doctor** | `doctor1@example.com` | `P@ssw0rd` |
| **Patient** | `patient1@example.com` | `P@ssw0rd` |
| **Receptionist** | `receptionist@example.com` | `P@ssw0rd` |
| **Nurse** | `nurse@example.com` | `P@ssw0rd` |

---

# النسخة العربية

## 🚀 الميزات الرئيسية للمشروع

### 👤 بوابات النظام حسب الصلاحيات

#### 1. بوابة المريض (Patient Portal)
* **محلل الأعراض بالذكاء الاصطناعي:** إدخال الأعراض المرضية ليقوم الذكاء الاصطناعي بتحليلها وتوجيه المريض إلى التخصص الطبي المناسب مباشرةً.
* **حجز المواعيد:** حجز سهل للمواعيد بناءً على القسم، الطبيب، والفتحات الزمنية المتاحة.
* **المحفظة الرقمية والدفع الإلكتروني:** شحن المحفظة والدفع الإلكتروني المباشر عبر بوابة **Paymob Egypt** (البطاقات البنكية ومحافظ الهاتف المحمول).
* **تحليل الروشتة بالذكاء الاصطناعي:** تقديم شرح مبسط للمريض باللغة العربية للأدوية الموصوفة، استخداماتها، ومادتها الفعالة وآثارها الجانبية.
* **البحث عن البدائل المحلية:** اقتراح بدائل محلية مصرية (تحتوي على نفس المادة الفعالة وبتكلفة أقل) عبر محرك الذكاء الاصطناعي.
* **السجل الطبي والروشتات:** استعراض وتحميل السجل الطبي الكامل للمريض والروشتات السابقة.
* **تقييم الأطباء:** كتابة مراجعات وتقييمات للأطباء بعد إتمام الاستشارة الطبية.

#### 2. بوابة الطبيب (Doctor Portal)
* **إدارة المواعيد المتاحة:** تحديد وتعديل أيام العمل والفتحات الزمنية المتاحة لاستقبال المرضى.
* **إدارة الحجوزات:** الاطلاع على قائمة مواعيد اليوم، قبول الحجوزات، وتسجيل الملاحظات الطبية.
* **ملخص التاريخ المرضي بالذكاء الاصطناعي:** توليد ملخص طبي شامل للمريض يجمع سجله السابق وحالاته المزمنة لتسهيل التشخيص على الطبيب.
* **الروشتة الإلكترونية:** كتابة الروشتات وتحديد الأدوية، جرعاتها، وتكرارها، ومدة الاستخدام.
* **إدارة السجلات الطبية:** تدوين الأعراض، التشخيص، العلامات الحيوية (الضغط، الحرارة، النبض، الوزن)، وخطة العلاج.

#### 3. بوابة موظف الاستقبال (Receptionist Portal)
* **تسجيل الحالات العاجلة:** تسجيل المرضى وتنظيم طوابير الانتظار في المستشفى.
* **تسجيل حضور المرضى:** تأكيد وصول المريض لتحديث حالته في لوحة تحكم الطبيب.
* **الحجز اليدوي:** مساعدة المرضى في حجز المواعيد مباشرة من داخل المستشفى.

#### 4. لوحة تحكم المشرف (Admin Portal)
* **مؤشرات الأداء للنظام:** متابعة إحصائيات المستشفى، الأطباء النشطين، المرضى، وإجمالي الفواتير.
* **إدارة شؤون الموظفين والمرضى:** إنشاء وتعديل وحظر حسابات (الأطباء، المرضى، الاستقبال، المشرفين).
* **إدارة الأقسام الطبية:** إضافة وتحديث الأقسام الطبية داخل المستشفى (مثل القلب، الأعصاب، الأطفال).
* **التدقيق المالي:** مراقبة كافة فواتير النظام، عمليات الشحن للمحافظ، والمدفوعات الإلكترونية.
* **سجل التدقيق والتتبع (Audit Logs):** مراجعة تاريخ التعديلات الطارئة على المواعيد والسجلات الطبية لضمان الشفافية.

---

## 🤖 دمج الذكاء الاصطناعي (OpenAI GPT-4o)
يعمل الذكاء الاصطناعي كـ مساعد طبي ذكي داخل منصة "صحتك":
1. **شرح الروشتة:** تحويل المصطلحات الدوائية المعقدة إلى دليل نصي سهل الفهم باللغة العربية للمريض.
2. **موجه الأعراض:** يحلل الشكوى الصحية ويوجه المريض للقسم الطبي المناسب (مثال: "ألم في الصدر" -> قسم القلب).
3. **محرك البدائل الدوائية:** يقترح بدائل تجارية محلية أرخص سعراً وتحمل نفس المادة الفعالة.
4. **الملخص الطبي:** يلخص السجلات الطبية القديمة للمريض في بضعة أسطر لمساعدة الطبيب قبل الفحص.

---

## 🛠️ التقنيات المستخدمة

### الخلفية (Backend)
* **الإطار البرمجي:** ASP.NET Core 8.0 Web API
* **بنية المشروع:** هندسة نظيفة مبنية على النطاق (Domain-Driven Clean Architecture)
* **التعامل مع البيانات:** Entity Framework Core مع تطبيق نمطي Repository & Unit of Work
* **قاعدة البيانات:** Microsoft SQL Server
* **الأمان وصلاحيات الوصول:** ASP.NET Core Identity مع التوثيق عبر JWT وتحديث الرموز (Refresh Tokens)
* **توثيق واجهة البرمجيات:** Swagger / OpenAPI

### الواجهة الأمامية (Frontend)
* **الإطار البرمجي:** Angular 19.2 (TypeScript)
* **تقنية الرندرة:** Server-Side Rendering (SSR) لتسريع التصفح وتحسين محركات البحث
* **العناصر الرسومية:** PrimeNG (النسخة 19) و PrimeIcons
* **التنسيق الجمالي:** Bootstrap 5, FontAwesome 7، و ملفات SCSS مخصصة

---

## ⚙️ طريقة التشغيل والتنصيب

### 1. المتطلبات الأساسية
* [.NET SDK 8.0+](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Node.js v18+](https://nodejs.org/)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/)

### 2. إعداد ملف appsettings.json
قم بفتح الملف `Back-end/Sehaty.Solution/Sehaty.APIs/appsettings.json` وتهيئة البيانات التالية:
* **ConnectionStrings:** نص الاتصال بقاعدة بيانات SQL Server الخاصة بك.
* **Jwt:** إدخال مفتاح التشفير الخاص بك (Key) وإعدادات وقت انتهاء الصلاحية.
* **EmailSettings:** بيانات بريد Gmail الخاص بك لإرسال كود التحقق (الرقم السري للتطبيق).
* **TwilioSMSSetting:** بيانات حساب Twilio لإرسال رسائل الـ SMS.
* **OpenAI:** مفتاح الربط الخاص بـ OpenAI API Key لتفعيل ميزات الذكاء الاصطناعي.
* **PaymobEgy2Settings:** بيانات حساب Paymob لتفعيل الدفع الإلكتروني.

### 3. إنشاء قاعدة البيانات وبدء التغذية (Seeding)
من مجلد `Back-end/Sehaty.Solution/` افتح سطر الأوامر ونفذ:
```bash
# تطبيق الهجرات البرمجية وبناء الجداول
dotnet ef database update --project Sehaty.Infrastructure --startup-project Sehaty.APIs
```
*ملاحظة: يحتوي النظام على ميزة التغذية التلقائية لقاعدة البيانات (Data Seeding). عند تشغيل التطبيق لأول مرة، سيقوم تلقائياً بقراءة البيانات الافتراضية (الأدوار، الأقسام الطبية، حسابات تجريبية للأطباء والمرضى) من ملفات JSON وحفظها في قاعدة البيانات.*

### 4. تشغيل الخلفية (Backend)
من المجلد `Back-end/Sehaty.Solution/Sehaty.APIs/` نفذ:
```bash
dotnet run
# أو للتشغيل التفاعلي
dotnet watch run
```
ستكون لوحة توثيق API متاحة على الرابط `https://localhost:7086/swagger/index.html`.

### 5. تشغيل الواجهة الأمامية (Frontend)
من مجلد `Front-end/` نفذ الأوامر التالية:
```bash
# تنزيل الحزم البرمجية
npm install

# تشغيل خادم التطوير المحلي
npm start
```
افتح المتصفح على الرابط `http://localhost:4200/`.

---

## 🔑 حسابات التجربة والاختبار
يمكنك استخدام الحسابات التالية لتجربة الصلاحيات المختلفة داخل المنصة، وكلمة المرور الافتراضية لجميع الحسابات هي **`P@ssw0rd`**:

| الصلاحية | البريد الإلكتروني | كلمة المرور |
| :--- | :--- | :--- |
| **المشرف (Admin)** | `admin@example.com` | `P@ssw0rd` |
| **الطبيب (Doctor)** | `doctor1@example.com` | `P@ssw0rd` |
| **المريض (Patient)** | `patient1@example.com` | `P@ssw0rd` |
| **موظف الاستقبال (Receptionist)** | `receptionist@example.com` | `P@ssw0rd` |
| **الممرض (Nurse)** | `nurse@example.com` | `P@ssw0rd` |

---

## ⚖️ License & Contributions
This project is open-source. Feel free to contribute, report bugs, or request features through GitHub issues and pull requests.
