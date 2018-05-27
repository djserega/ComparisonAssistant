using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant.Model
{
    internal class ConnectorStorage
    {
        internal string Server { get; set; }
        internal string Base { get; set; }
        internal string StoragePath { get; set; }
        internal string StorageUser { get; set; }
        internal string StoragePass { get; set; }

        internal string ConnectionString
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Server)
                    || string.IsNullOrWhiteSpace(Base))
                    return string.Empty;

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("/S");
                stringBuilder.Append(Server);
                stringBuilder.Append("\\");
                stringBuilder.Append(Base);

                return stringBuilder.ToString();
            }
        }

        internal bool CheckFilledParameters()
        {
            bool result = true;

            if (string.IsNullOrEmpty(Server))
                result = false;
            else if (string.IsNullOrEmpty(Base))
                result = false;
            else if (string.IsNullOrEmpty(StoragePath))
                result = false;
            else if (string.IsNullOrEmpty(StorageUser))
                result = false;

            return result;
        }

    }
}
