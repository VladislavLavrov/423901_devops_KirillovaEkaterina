# ForecastApp1

Простое веб-приложение с системой авторизации на основе MariaDB.

## Требования

- Docker и Docker Compose
- .NET 9.0 SDK (для локальной разработки)

## Быстрый старт с Docker

1. Клонируйте репозиторий или перейдите в директорию проекта

2. Запустите приложение с помощью Docker Compose:
   ```bash
   docker-compose up -d
   ```

3. **ВАЖНО:** Создайте схему базы данных:
   
   **Windows:**
   ```bash
   init-db.bat
   ```
   
   **Linux/Mac:**
   ```bash
   chmod +x init-db.sh
   ./init-db.sh
   ```
   
   Или вручную:
   ```bash
   docker exec -i forecastapp_mariadb mysql -uroot -prootpassword predictApp < database_schema.sql
   ```

4. Приложение будет доступно по адресу:
   - HTTP: http://localhost:5005
   - MariaDB: localhost:3306

5. Войдите в систему:
   - Логин: `admin` / Пароль: `admin`

6. Остановка приложения:
   ```bash
   docker-compose down
   ```

7. Остановка с удалением данных БД:
   ```bash
   docker-compose down -v
   ```

**Подробная инструкция:** См. файл `SETUP.md`

## Конфигурация

### База данных

MariaDB настраивается автоматически при первом запуске:
- **База данных**: `predictApp`
- **Пользователь**: `forecastuser`
- **Пароль**: `forecastpass`
- **Root пароль**: `rootpassword`

### Переменные окружения

В `docker-compose.yml` можно настроить:
- `ASPNETCORE_ENVIRONMENT` - окружение приложения (Development/Production)
- `ConnectionStrings__DefaultConnection` - строка подключения к БД

## Локальная разработка

1. Установите MariaDB локально или используйте Docker:
   ```bash
   docker run -d --name mariadb -p 3306:3306 -e MYSQL_ROOT_PASSWORD=rootpassword -e MYSQL_DATABASE=predictApp -e MYSQL_USER=forecastuser -e MYSQL_PASSWORD=forecastpass mariadb:11.3
   ```

2. Обновите `appsettings.Development.json` с правильной строкой подключения

3. Запустите приложение:
   ```bash
   cd ForecastApp1
   dotnet run
   ```

## Функциональность

Приложение предоставляет систему авторизации:
- **Вход в систему** (`POST /api/Auth/login`) - аутентификация пользователей
- **Выход из системы** (`POST /api/Auth/logout`) - выход из системы
- **Информация о пользователе** (`GET /api/Auth/me`) - получение данных текущего пользователя

## Создание схемы базы данных

После первого запуска необходимо создать таблицу пользователей в базе данных. Используйте SQL скрипт `database_schema.sql`.

Таблица:
- `users` - пользователи системы (username, password_hash, role)

## Порты

- **5250** - HTTP порт приложения
- **3306** - Порт MariaDB

## Структура проекта

- `ForecastApp1/` - основное приложение ASP.NET Core
- `Dockerfile` - конфигурация Docker образа
- `docker-compose.yml` - конфигурация Docker Compose
- `.dockerignore` - файлы, исключаемые из Docker образа

