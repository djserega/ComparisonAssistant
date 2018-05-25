﻿#Использовать cmdline
#Использовать v8storage

Лог = Логирование.ПолучитьЛог("oscript.lib.v8runner");
Лог.УстановитьУровень(0);

Парсер = Новый ПарсерАргументовКоманднойСтроки();
Парсер.ДобавитьПараметр("СтрокаСоединения");
Парсер.ДобавитьПараметр("ПутьКХранилищу");
Парсер.ДобавитьПараметр("ИмяПользователя");
Парсер.ДобавитьПараметр("ПарольПользователя");

Параметры = Парсер.Разобрать(АргументыКоманднойСтроки);

ХранилищеКонфигурации = Новый МенеджерХранилищаКонфигурации(Параметры["ПутьКХранилищу"],, Параметры["СтрокаСоединения"]);
ХранилищеКонфигурации.УстановитьПараметрыАвторизации(
			Параметры["ИмяПользователя"],
			Параметры["ПарольПользователя"]);

ХранилищеКонфигурации.ЗахватитьОбъектыВХранилище();