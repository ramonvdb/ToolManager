using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ToolManagerLibrary
{
    public class ToolList
    {

        public List<ToolModel> Complete()
        {
            return SqliteDataAcces.LoadAllToolData();
        }

        public List<ToolModel> Filter(string machine, string type, string function)
        {
            return SqliteDataAcces.LoadFilteredData(machine, type, function);
        }

    }
}
