using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace Ultimus.AuditManager.Admin.Commons
{
    public class DataBaseAccess
    {

        public SqlConnection SqlCon;
        public SqlCommand SqlCom;

        public DataBaseAccess()
        {

        }

        public DataBaseAccess(string connectionString, string query)
        {
            SqlCon = new System.Data.SqlClient.SqlConnection(connectionString);
            SqlCon.Open();

            SqlCom = new System.Data.SqlClient.SqlCommand();
            SqlCom.Connection = SqlCon;
            SqlCom.CommandType = CommandType.Text;
            SqlCom.CommandText = query;
        }

        public void CloseConnection() {
            try
            {
                SqlCon.Close();
            }
            catch (Exception ex) {
                Debug.Write("[Ultimus.ComponentManager] GetTablesByConnectionId ERROR " + ex.ToString());
            }
        }
    }
}