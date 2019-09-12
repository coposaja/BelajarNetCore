using Belajar9.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Belajar9.Models
{
    public class UserExcelModel : DataTableAdaptor
    {
        private List<UserModel> listUserModel { get; set; }

        public UserExcelModel(List<UserModel> listUserModel)
        {
            this.listUserModel = listUserModel;
        }
        public DataTable ToDataTable()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Username", typeof(string));
            dataTable.Columns.Add("Full Name", typeof(string));
            dataTable.Columns.Add("Age", typeof(int));

            foreach (UserModel dr in listUserModel)
            {
                var row = new object[3];
                row[0] = dr.Username;
                row[1] = dr.FullName;
                row[2] = dr.Age;

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
