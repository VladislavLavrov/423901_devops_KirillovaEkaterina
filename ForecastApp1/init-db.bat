@echo off
REM Скрипт для инициализации базы данных (Windows)

echo Ожидание запуска MariaDB...
timeout /t 10 /nobreak >nul

echo Создание схемы базы данных...
docker exec -i forecastapp_mariadb mysql -uroot -prootpassword predictApp < database_schema.sql

if %errorlevel% equ 0 (
    echo ✓ Схема базы данных успешно создана!
) else (
    echo ✗ Ошибка при создании схемы базы данных
    exit /b 1
)

echo.
echo Проверка созданных таблиц...
docker exec -it forecastapp_mariadb mysql -uroot -prootpassword predictApp -e "SHOW TABLES;"

echo.
echo Готово! Приложение готово к использованию.
pause

