using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant.Model
{
    internal class ConnectorStorage
    {
        internal bool TypeConnection { get; set; } // true - server
        internal string Server { get; set; }
        internal string Base { get; set; }
        internal string PathBase { get; set; }
        internal string StoragePath { get; set; }
        internal string StorageUser { get; set; }
        internal string StoragePass { get; set; }

        internal string ConnectionString
        {
            get
            {
                if (TypeConnection)
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
                else
                {
                    if (string.IsNullOrWhiteSpace(PathBase))
                        return string.Empty;

                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("/F");
                    stringBuilder.Append(PathBase);

                    return stringBuilder.ToString();
                }
            }
        }

        internal bool CheckFilledParameters()
        {
            bool result = true;

            if (TypeConnection && string.IsNullOrEmpty(Server))
                result = false;
            else if (TypeConnection && string.IsNullOrEmpty(Base))
                result = false;
            else if (!TypeConnection && string.IsNullOrEmpty(PathBase))
                result = false;
            else if (string.IsNullOrEmpty(StoragePath))
                result = false;
            else if (string.IsNullOrEmpty(StorageUser))
                result = false;

            return result;
        }

    }
}
