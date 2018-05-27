using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant.Model
{
    internal class TranslateObject
    {
        internal Dictionary<string, string> DictionaryObjct { get; } = new Dictionary<string, string>()
        {
            { "Configuration", "Конфигурация"},
            { "Language", "Язык"},
            { "Subsystem", "Подсистема"},
            { "StyleItem", "Элемент стиля"},
            { "CommonPicture", "Общая картинка"},
            { "Interface", "Интерфейс"},
            { "SessionParameter", "Параметр сеанса"},
            { "Role", "Роль"},
            { "CommonTemplate", "Общий макет"},
            { "FilterCriteria", "Критерий отбора"},
            { "CommonModule", "Общий модуль"},
            { "CommonAttribute", "Общий реквизит"},
            { "ExchangePlan", "План обмена"},
            { "XDTOPackage", "XDTO-пакет"},
            { "WebService", "Web-сервис"},
            { "EventSubscription", "Подписка на событие"},
            { "ScheduledJob", "Регламентное задание"},
            { "FunctionalOption", "Функциональная опция"},
            { "FunctionalOptionsParameter", "Параметр функциональных опций"},
            { "CommonCommand", "Общая команда"},
            { "CommandGroup", "Группа команд"},
            { "Constant", "Константа"},
            { "CommonForm", "Общая форма"},
            { "Catalog", "Справочник"},
            { "Document", "Документ"},
            { "DocumentNumerator", "Нумератор документов"},
            { "Sequence", "Последовательность"},
            { "DocumentJournal", "Журнал документов"},
            { "Enum", "Перечисление"},
            { "Report", "Отчет"},
            { "DataProcessor", "Обработка"},
            { "InformationRegister", "Регистр сведений"},
            { "AccumulationRegister", "Регистр накопления"},
            { "ChartOfCharacteristicTypes", "План видов характеристик"},
            { "BusinessProcess", "Бизнес процесс"},
            { "Task", "Задачи"},
            { "ExternalDataSource", "Внешний источники данных"}
        };
    }
}
