using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace VWMS.CommonClass
{
    class DataOperate
    {
        DataCon datacon = new DataCon();
        OleDbConnection oledbcon;
        OleDbCommand oledbcom;
        OleDbDataAdapter oledbda;
        DataSet ds;

        public void getCom(string strCon)
        {
            oledbcon = datacon.getCon();
            oledbcom = new OleDbCommand(strCon, oledbcon);
            oledbcon.Open();
            oledbcom.ExecuteNonQuery();
            oledbcon.Close();
        }

        public DataSet getDs(string strCon,string tbname)
        {
            oledbcon = datacon.getCon();
            oledbda = new OleDbDataAdapter(strCon, oledbcon);
            ds = new DataSet();
            oledbda.Fill(ds, tbname);
            return ds;
        }
    }
}
