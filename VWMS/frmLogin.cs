using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//添加的命名空间
using System.Data.OleDb;
using VWMS.CommonClass;

namespace VWMS
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        DataCon datacon = new DataCon();
        DataOperate dataoperate = new DataOperate();

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                errorProName.SetError(txtName, "用户名不能为空!");
            }
            else
            {
                errorProName.Clear();
                string strSql = "select * from tb_admin where name='" + txtName.Text + "' and pwd='" + txtPwd.Text + "'";
                DataSet ds = dataoperate.getDs(strSql, "tb_admin");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    this.Hide();
                    //strSql = "update set on=ture from tb_admin where name ='"+ txtName.Text+"'"
                    frmMain.admin = txtName.Text;
                    frmMain frmmain = new frmMain();
                    frmmain.Show();
                }
                else
                {
                    MessageBox.Show("用户名或密码错误！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txtPwd.Focus();
                e.Handled = true;
            }
        }

        private void txtPwd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btnLogin.Focus();
                e.Handled = true;
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}