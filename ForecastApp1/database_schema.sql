-- Схема базы данных для ForecastApp1
-- MariaDB/MySQL
-- Упрощенная версия - только авторизация

CREATE DATABASE IF NOT EXISTS predictApp CHARACTER SET utf8 COLLATE utf8_general_ci;
USE predictApp;

-- Таблица пользователей
CREATE TABLE IF NOT EXISTS users (
    username VARCHAR(100) PRIMARY KEY,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Создание тестового пользователя (пароль: admin)
-- ВНИМАНИЕ: В продакшене используйте хеширование паролей!
INSERT INTO users (username, password_hash, role) 
VALUES ('admin', 'admin', 'admin')
ON DUPLICATE KEY UPDATE username = username;

INSERT INTO users (username, password_hash, role) 
VALUES ('accountant', 'accountant', 'accountant')
ON DUPLICATE KEY UPDATE username = username;

