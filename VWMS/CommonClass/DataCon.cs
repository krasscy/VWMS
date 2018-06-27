using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace VWMS.CommonClass
{
    class DataCon
    {
        public OleDbConnection getCon()
        {
            string strDPath = Application.StartupPath;
            string strDataSource = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" 
                + strDPath.Substring(0, strDPath.LastIndexOf("\\")).Substring(0, strDPath.Substring(0, strDPath.LastIndexOf("\\")).LastIndexOf("\\")) + "\\DataBase\\db_VWMS.mdb";
            OleDbConnection oledbCon = new OleDbConnection(strDataSource);
            return oledbCon;
        }
    }
}
