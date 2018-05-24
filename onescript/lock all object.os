﻿#Использовать cmdline
#Использовать v8storage

Парсер = Новый ПарсерАргументовКоманднойСтроки();
Парсер.ДобавитьПараметр("ПутьКХранилищу");
//Парсер.ДобавитьПараметр("ПутьКФайлу");
Парсер.ДобавитьПараметр("ИмяПользователя");
Парсер.ДобавитьПараметр("ПарольПользователя");

Параметры = Парсер.Разобрать(АргументыКоманднойСтроки);

ХранилищеКонфигурации = Новый МенеджерХранилищаКонфигурации(Параметры["ПутьКХранилищу"]);
ХранилищеКонфигурации.УстановитьПараметрыАвторизации(
			Параметры["ИмяПользователя"],
			Параметры["ПарольПользователя"]);

ХранилищеКонфигурации.ЗахватитьОбъектыВХранилище();