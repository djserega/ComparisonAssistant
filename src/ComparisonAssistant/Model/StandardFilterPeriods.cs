using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant.Model
{
    public class StandardFilterPeriods
    {
        public string Name { get; internal set; }
        internal DateTime? DateStart { get; set; }
        internal DateTime? DateEnd { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public StandardFilterPeriods() { }

        public StandardFilterPeriods(string name, DateTime? dateStart = null, DateTime? dateEnd = null)
        {
            Name = name;
            DateStart = dateStart;
            DateEnd = dateEnd;
        }

        internal List<StandardFilterPeriods> GetListPeriodsByDefault()
        {
            List<StandardFilterPeriods> list = new List<StandardFilterPeriods>()
            {
                { new StandardFilterPeriods(Name = "Сегодня", DateStart = DateTime.Now.StartDay(), DateEnd = DateTime.Now.EndDay()) },
                { new StandardFilterPeriods(Name = "Со вчера", DateStart = DateTime.Now.StartDay().AddDays(-1), DateEnd = DateTime.Now.EndDay()) },
                { new StandardFilterPeriods(Name = "Текущая неделя", DateStart = DateTime.Now.StartWeek(), DateEnd = DateTime.Now.EndDay()) },
                { new StandardFilterPeriods(Name = "Последние 7 дней", DateStart = DateTime.Now.StartDay().AddDays(-7), DateEnd = DateTime.Now.EndDay()) },
                { new StandardFilterPeriods(Name = "Произвольный") }
            };

            return list;
        }
    }
}
