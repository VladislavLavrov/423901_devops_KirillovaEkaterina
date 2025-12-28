# Инструкция по запуску ForecastApp1

## Шаг 1: Запуск Docker контейнеров

Откройте терминал в корневой директории проекта и выполните:

```bash
docker-compose up -d
```

Эта команда:
- Создаст и запустит контейнер MariaDB
- Создаст и запустит контейнер с приложением
- Настроит сеть между контейнерами

Проверьте, что контейнеры запущены:
```bash
docker-compose ps
```

## Шаг 2: Создание схемы базы данных

После запуска контейнеров нужно создать таблицы в базе данных.

### Вариант 1: Через Docker (рекомендуется)

```bash
# Копируем SQL скрипт в контейнер MariaDB
docker cp database_schema.sql forecastapp_mariadb:/tmp/schema.sql

# Выполняем скрипт
docker exec -i forecastapp_mariadb mysql -uroot -prootpassword predictApp < database_schema.sql
```

### Вариант 2: Через MySQL клиент (если установлен локально)

```bash
mysql -h localhost -P 3306 -u forecastuser -pforecastpass predictApp < database_schema.sql
```

### Вариант 3: Через любой MySQL клиент (DBeaver, HeidiSQL, MySQL Workbench)

1. Подключитесь к базе данных:
   - **Host**: localhost
   - **Port**: 3306
   - **Database**: predictApp
   - **User**: root
   - **Password**: rootpassword

2. Откройте файл `database_schema.sql` и выполните его

## Шаг 3: Проверка работы приложения

1. Откройте браузер и перейдите по адресу: **http://localhost:5005**

2. Войдите в систему:
   - **Логин**: `admin`
   - **Пароль**: `admin`
   - Или используйте: `accountant` / `accountant`

3. Проверьте, что приложение загрузилось

## Шаг 4: Тестирование авторизации

1. Откройте страницу входа: **http://localhost:5250/login.html**

2. Войдите в систему:
   - **Логин**: `admin`
   - **Пароль**: `admin`

3. Проверьте API авторизации:
   ```bash
   # Вход
   curl -X POST http://localhost:5005/api/Auth/login \
     -d "username=admin&password=admin" \
     -c cookies.txt
   
   # Проверка текущего пользователя
   curl -X GET http://localhost:5005/api/Auth/me \
     -b cookies.txt
   
   # Выход
   curl -X POST http://localhost:5005/api/Auth/logout \
     -b cookies.txt
   ```

## Полезные команды

### Просмотр логов приложения
```bash
docker-compose logs -f forecastapp
```

### Просмотр логов MariaDB
```bash
docker-compose logs -f mariadb
```

### Остановка контейнеров
```bash
docker-compose down
```

### Остановка с удалением данных БД
```bash
docker-compose down -v
```

### Перезапуск приложения
```bash
docker-compose restart forecastapp
```

### Подключение к базе данных через командную строку
```bash
docker exec -it forecastapp_mariadb mysql -uforecastuser -pforecastpass predictApp
```

## Структура таблиц

- **users** - пользователи системы
  - `username` (VARCHAR, PRIMARY KEY) - имя пользователя
  - `password_hash` (VARCHAR) - пароль (в продакшене должен быть хеширован)
  - `role` (VARCHAR) - роль пользователя

## API Endpoints

### Авторизация
- `POST /api/Auth/login` - вход в систему
  - Параметры: `username`, `password` (form-data)
  - Возвращает: информация о пользователе и устанавливает cookie
  
- `POST /api/Auth/logout` - выход из системы
  - Удаляет cookie сессии
  
- `GET /api/Auth/me` - информация о текущем пользователе
  - Требует авторизации
  - Возвращает: `username`, `role`

## Устранение проблем

### Приложение не запускается
1. Проверьте логи: `docker-compose logs forecastapp`
2. Убедитесь, что порт 5250 свободен
3. Проверьте, что MariaDB запущена: `docker-compose ps`

### Ошибки подключения к БД
1. Убедитесь, что MariaDB запущена и здорова: `docker-compose ps`
2. Проверьте строку подключения в `appsettings.json`
3. Проверьте, что схема БД создана (выполнен `database_schema.sql`)

### Ошибки при импорте
1. Проверьте формат Excel файла
2. Убедитесь, что заголовки блоков точно соответствуют ожидаемым:
   - "Долг перед поставщиками входящий"
   - "Отгрузки от поставщиков фактические"
   - "Справочник поставщиков"
   - "Договоры"
   - "Условия договоров"
3. Проверьте логи приложения: `docker-compose logs -f forecastapp`

