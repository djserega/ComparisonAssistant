# Помощник сравнения

Парсер логов gitsync (https://github.com/oscript-library/gitsync)


Описание
--------

Приложение разработано как помощник переноса изменений из конфигурации которая подключена к gitsync (хранилище разработки) в рабочую конфигурацию.


Использование
--------

- при каждом помещении в хранилище программист указывает в комментарии номер задачи.
- gitsync выполняет выгрузку конфигурации в git
- по расписанию или после завершении выгрузки выгружается log (формат выгрузки описан ниже). 
- "Помощник сравнения" анализирует файл логов (log.txt) в каталоге приложения.

### В результате имеем:
#### Фильтры:
```bsl
1. по пользователю
2. по задаче
3. по дате помещения в хранилище
```
#### Таблицу изменений объектов 1С
```bsl
- Статус
- Имя объекта
- Изменения в объекте
- Изменения в модуле объекта
- Изменения в модуле менеджера
- Изменения в модуле формы
- Изменения в шаблонах
```

#### Шаблон выгрузки логов:
```bsl
git log --branches=*[master] --pretty=format:"%an --- %s --- %cI " --name-status > log.txt
```

Требуется .NET Framework: v 4.5.

В планах закончить разработку захвата объектов в хранилище приемнике сравнения (будет использована библиотека onescript "v8storage").

Use icon from https://www.flaticon.com/ is licensed by http://creativecommons.org/licenses/by/3.0/
