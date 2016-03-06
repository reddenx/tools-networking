using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Sql.ObjectMapping
{
    public class DBColumnAttribute : Attribute
    {
        public readonly string ColumnName;

        public DBColumnAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }
    }
}
