# 🍽️ NChrono - FeaneMVC Online Food Store

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=.net&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?style=for-the-badge&logo=.net&logoColor=white)
![Entity Framework](https://img.shields.io/badge/Entity_Framework-512BD4?style=for-the-badge&logo=.net&logoColor=white)
![SQL Server](https://img.shields.io/badge/Microsoft_SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)

**Современный онлайн-магазин еды с доставкой и бронированием столиков**

[🚀 Демо](#demo) • [📖 Документация](#table-of-contents) • [🐛 Сообщить об ошибке](../../issues) • [✨ Запросить функцию](../../issues)

</div>

---

## 📋 Содержание

- [🎯 О проекте](#-о-проекте)
- [✨ Основные возможности](#-основные-возможности)
- [🛠️ Технологический стек](#️-технологический-стек)
- [🚀 Быстрый старт](#-быстрый-старт)
  - [📋 Требования](#-требования)
  - [⚙️ Установка](#️-установка)
  - [🐳 Запуск с Docker](#-запуск-с-docker)
- [📖 Использование](#-использование)
- [🏗️ Архитектура проекта](#️-архитектура-проекта)
- [🤝 Участие в разработке](#-участие-в-разработке)
- [📄 Лицензия](#-лицензия)
- [📧 Контакты](#-контакты)

---

## 🎯 О проекте

**NChrono** - это современное веб-приложение для онлайн-заказа еды, разработанное на платформе **ASP.NET Core 8.0**. Проект предоставляет полнофункциональную систему управления рестораном с возможностями доставки блюд и бронирования столиков.

### 🎨 Скриншоты

> 📸 *Скриншоты интерфейса будут добавлены после развертывания*

---

## ✨ Основные возможности

### 👥 Для клиентов
- 🍕 **Каталог блюд** - Просмотр меню с подробными описаниями и фотографиями
- 🛒 **Корзина покупок** - Удобное добавление блюд и оформление заказа
- 🏪 **Система бронирования** - Резервирование столиков в ресторане
- 👤 **Личный кабинет** - Управление профилем и историей заказов
- 📧 **Email уведомления** - Подтверждения заказов и важные обновления

### 🔧 Для администраторов
- 👨‍💼 **Управление пользователями** - Регистрация, роли и права доступа
- 🍽️ **Управление меню** - Добавление, редактирование и удаление блюд
- 📊 **Панель администратора** - Мониторинг заказов и резервирований
- 🔐 **Система ролей** - Гибкое управление правами (Админ, Модератор, VIP)
- 🔑 **Сброс паролей** - Административные функции безопасности

---

## 🛠️ Технологический стек

### Backend
- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **ORM**: Entity Framework Core 7.0
- **Database**: SQL Server / PostgreSQL
- **Email**: MailKit 4.11.0
- **Architecture**: MVC Pattern + Repository Pattern

### Frontend
- **UI Framework**: Bootstrap
- **JavaScript**: jQuery
- **Styling**: CSS3 + SCSS

### DevOps & Tools
- **Containerization**: Docker
- **Database Management**: SQL Server Management Studio / pgAdmin
- **Version Control**: Git
- **IDE**: Visual Studio / Visual Studio Code

---

## 🚀 Быстрый старт

### 📋 Требования

Убедитесь, что у вас установлены следующие компоненты:

- **[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** или выше
- **[SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads)** или **[PostgreSQL](https://www.postgresql.org/download/)**
- **[Docker](https://www.docker.com/get-started)** (опционально)
- **[Git](https://git-scm.com/downloads)**

### ⚙️ Установка

1. **Клонируйте репозиторий**
   ```bash
   git clone https://github.com/DmitryV13/NChrono.git
   cd NChrono
   ```

2. **Перейдите в директорию проекта**
   ```bash
   cd FeaneMVC
   ```

3. **Восстановите зависимости**
   ```bash
   dotnet restore
   ```

4. **Настройте базу данных**
   
   Отредактируйте файл `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=YOUR_SERVER;Initial Catalog=FeaneMVC;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True;"
     }
   }
   ```

5. **Примените миграции**
   ```bash
   dotnet ef database update
   ```

6. **Запустите приложение**
   ```bash
   dotnet run
   ```

7. **Откройте браузер**
   
   Перейдите по адресу: `https://localhost:5001` или `http://localhost:5000`

### 🐳 Запуск с Docker

Для быстрого развертывания с базой данных SQL Server:

1. **Запустите контейнеры**
   ```bash
   docker-compose up -d
   ```

2. **Соберите и запустите приложение**
   ```bash
   docker build -t nchrono-app .
   docker run -p 5000:80 nchrono-app
   ```

---

## 📖 Использование

После запуска приложения вы можете:

### 🏠 Главная страница
- Просмотр популярных блюд
- Навигация по категориям меню
- Поиск по названию блюда

### 🛍️ Процесс заказа
1. Выберите блюда из каталога
2. Добавьте их в корзину
3. Заполните информацию о доставке
4. Подтвердите заказ

### 📅 Бронирование столика
1. Выберите дату и время
2. Укажите количество гостей
3. Оставьте комментарий (опционально)
4. Подтвердите бронирование

---

## 🏗️ Архитектура проекта

```
FeaneMVC/
├── 📁 Controllers/          # MVC контроллеры
├── 📁 Models/               # Модели данных
│   ├── 📁 Enums/           # Перечисления
│   └── 📁 Response/        # Модели ответов
├── 📁 Views/               # Razor представления
├── 📁 DbModel/             # Контекст базы данных
├── 📁 Repository/          # Паттерн Repository
├── 📁 Interfaces/          # Интерфейсы сервисов
├── 📁 Helpers/             # Вспомогательные классы
├── 📁 Attributes/          # Пользовательские атрибуты
├── 📁 Extensions/          # Методы расширения
└── 📁 wwwroot/             # Статические файлы
```

### 🔧 Ключевые компоненты

- **Controllers**: Обработка HTTP-запросов и бизнес-логика
- **Models**: Структуры данных и валидация
- **Views**: Пользовательский интерфейс (Razor Pages)
- **Repository**: Абстракция доступа к данным
- **Attributes**: Авторизация и ограничения доступа
- **Helpers**: Утилиты для работы с cookies, email и др.

---

## 🤝 Участие в разработке

Мы приветствуем вклад в развитие проекта! Вот как вы можете помочь:

### 🐛 Сообщить об ошибке

1. Проверьте [существующие issues](../../issues)
2. Создайте новый issue с подробным описанием
3. Приложите скриншоты и логи (если применимо)

### ✨ Предложить улучшение

1. **Fork** репозиторий
2. Создайте ветку для новой функции:
   ```bash
   git checkout -b feature/amazing-feature
   ```
3. Внесите изменения и сделайте коммиты:
   ```bash
   git commit -m 'Add: amazing feature'
   ```
4. Отправьте изменения:
   ```bash
   git push origin feature/amazing-feature
   ```
5. Создайте Pull Request

### 📝 Правила разработки

- Следуйте стандартам кодирования C#
- Добавляйте комментарии к сложной логике
- Покрывайте новый код тестами
- Обновляйте документацию при необходимости

---

## 📄 Лицензия

Этот проект распространяется под лицензией **MIT License**. Подробности в файле [LICENSE.txt](LICENSE.txt).

```
MIT License

Copyright (c) 2024 NChrono Project

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
```

---

## 📧 Контакты

<div align="center">

**Есть вопросы или предложения?**

[![Email](https://img.shields.io/badge/Email-wonderful_by@bk.ru-D14836?style=for-the-badge&logo=gmail&logoColor=white)](mailto:wonderful_by@bk.ru)
[![GitHub](https://img.shields.io/badge/GitHub-DmitryV13-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/DmitryV13)

</div>

---

<div align="center">

**⭐ Если проект был полезен, поставьте звездочку!**

Made with ❤️ by [DmitryV13](https://github.com/DmitryV13)

</div>