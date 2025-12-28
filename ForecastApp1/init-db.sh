#!/bin/bash
# Скрипт для инициализации базы данных

echo "Ожидание запуска MariaDB..."
sleep 10

echo "Создание схемы базы данных..."
docker exec -i forecastapp_mariadb mysql -uroot -prootpassword predictApp < database_schema.sql

if [ $? -eq 0 ]; then
    echo "✓ Схема базы данных успешно создана!"
else
    echo "✗ Ошибка при создании схемы базы данных"
    exit 1
fi

echo ""
echo "Проверка созданных таблиц..."
docker exec -it forecastapp_mariadb mysql -uroot -prootpassword predictApp -e "SHOW TABLES;"

echo ""
echo "Готово! Приложение готово к использованию."

