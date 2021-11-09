Створення додатку бази даних, орієнтованого на взаємодію з СУБД PostgreSQL
Структура бази даних: користувач - замовмлення(1:N), замовлення - товар(M:N)
Програма містить такі команди:
g(i,o,u)(id) - get entity by id;
d(i,o,u)(id) - delete entity by id;
dio - delete connection between order and item;
iio - adding connection between order and item;
u(i,o,u)(id) - update entity by id;
i(i,o,u) - insert entity;
s - search;
Gall - generation of values.