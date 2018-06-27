using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//添加的命名空间
using VWMS.CommonClass;
using System.IO.Ports;
using Microsoft.Win32;
using System.Threading;

namespace VWMS
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        PelcoD pelcod = new PelcoD();
        SoftReg softreg = new SoftReg();
        SerialPort serialPort = new SerialPort("COM1", 2400, Parity.None, 8);
        int m_dwDevNum = 0;//视频路数
        byte addressin = Byte.Parse(Convert.ToString(0x01));
        byte speedin = Byte.Parse(Convert.ToString(0xff));
        byte[] messagesend;
        DataCon datacon = new DataCon();
        DataOperate dataoperate = new DataOperate();
        public static string admin;

        //窗体加载时，初始化视频卡，并开始预览视频
        private void frmMain_Load(object sender, EventArgs e)
        {
            plVideo1.BackgroundImage = null;
            RegistryKey retkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("software", true).CreateSubKey("wxk").CreateSubKey("wxk.INI");
            foreach (string strRNum in retkey.GetSubKeyNames())//判断是否注册
            {
                if (strRNum == softreg.getRNum())
                {
                    this.Text = "家庭视频监控系统（已注册）" + admin;
                    string strSql = "select * from tb_admin where name='" + admin + "' and power=0";
                    DataSet ds = dataoperate.getDs(strSql, "tb_admin");
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                                    btnSetMonitor.Enabled = btnAutoMonitor.Enabled = false; //不显示仅管理员用户可用的按钮。
                    }
                    btnReg.Enabled = false;
                    startMonitor();
                    return; //return跳出了此方法，因此不会执行接下来的语句。
                }
            }
            this.Text = "家庭视频监控系统（未注册）";
            btnReg.Enabled = true;
            btnSetMonitor.Enabled = btnAutoMonitor.Enabled = false; //不显示仅注册用户可用的按钮。
            startMonitor();
            MessageBox.Show("您现在使用的是试用版，该软件可以免费试用30次！","提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Int32 tLong;
            //通过注册表记录使用次数，若注册表中未查找到注册项，则认为是新用户首次使用，并创建注册项。
            try
            {
                tLong = (Int32)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\angel", "UseTimes", 0);
                MessageBox.Show("感谢您已使用了" + tLong + "次", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\angel", "UseTimes", 0, RegistryValueKind.DWord);
                MessageBox.Show("欢迎新用户使用本软件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            tLong = (Int32)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\angel", "UseTimes", 0);
            if (tLong < 30)
            {
                int Times = tLong + 1;
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\angel", "UseTimes", Times);
            }
            else
            {
                MessageBox.Show("试用次数已到", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
        }
        //移动窗体位置时，视频随之移动
        private void frmMain_Move(object sender, EventArgs e)
        {
            for (int i = 0; i < m_dwDevNum; i++)
            {
                plVideo1.Invalidate();
                VideoOperate.VCAUpdateOverlayWnd(this.Handle);
                VideoOperate.VCAUpdateVideoPreview(i, plVideo1.Handle);
            }
        }

        //每个Open()并使用Write写入命令后必须Close()
        #region  云台控制
        //增加聚焦
        private void btnAHighlghts_MouseDown(object sender, MouseEventArgs e)
        {
            messagesend = pelcod.CameraFocus(addressin, PelcoD.Focus.Near);
            serialPort.Open();
            serialPort.Write(messagesend, 0, 7);
            serialPort.Close();
        }
        //减小聚焦
        private void btnCHighlghts_MouseDown(object sender, MouseEventArgs e)
        {
            messagesend = pelcod.CameraFocus(addressin, PelcoD.Focus.Far);
            serialPort.Open();
            serialPort.Write(messagesend, 0, 7);
            serialPort.Close();
        }
        //增加对焦
        private void btnAFocus_MouseDown(object sender, MouseEventArgs e)
        {
            messagesend = pelcod.CameraZoom(addressin, PelcoD.Zoom.Tele);
            serialPort.Open();
            serialPort.Write(messagesend, 0, 7);
            serialPort.Close();
        }
        //减小对焦
        private void btnCFocus_MouseDown(object sender, MouseEventArgs e)
        {
            messagesend = pelcod.CameraZoom(addressin, PelcoD.Zoom.Wide);
            serialPort.Open();
            serialPort.Write(messagesend, 0, 7);
            serialPort.Close();
        }
        //增加光圈
        private void btnAAperture_MouseDown(object sender, MouseEventArgs e)
        {
            messagesend = pelcod.CameraIrisSwitch(addressin, PelcoD.Iris.Close);
            serialPort.Open();
            serialPort.Write(messagesend, 0, 7);
            serialPort.Close();
        }
        //减小光圈
        private void btnCAperture_MouseDown(object sender, MouseEventArgs e)
        {
            messagesend = pelcod.CameraIrisSwitch(addressin, PelcoD.Iris.Open);
            serialPort.Open();
            serialPort.Write(messagesend, 0, 7);
            serialPort.Close();
        }
        //增加雨刷
        private void btnAWipers_MouseDown(object sender, MouseEventArgs e)
        {
            messagesend = pelcod.CameraSwitch(addressin, PelcoD.Switch.On);
            serialPort.Open();
            serialPort.Write(messagesend, 0, 7);
            serialPort.Close();
        }
        //减小雨刷
        private void btnCWipers_MouseDown(object sender, MouseEventArgs e)
        {
            messagesend = pelcod.CameraSwitch(addressin, PelcoD.Switch.Off);
            serialPort.Open();
            serialPort.Write(messagesend, 0, 7);
            serialPort.Close();
        }
        //方向控制——上
        private void btnUp_MouseDown(object sender, MouseEventArgs e)
        {
            messagesend = pelcod.CameraTilt(addressin, PelcoD.Tilt.Up, speedin);
            serialPort.Open();
            serialPort.Write(messagesend, 0, 7);
            serialPort.Close();
        }
        //方向控制——下
        private void btnDown_MouseDown(object sender, MouseEventArgs e)
        {
            messagesend = pelcod.CameraTilt(addressin, PelcoD.Tilt.Down, speedin);
            serialPort.Open();
            serialPort.Write(messagesend, 0, 7);
            serialPort.Close();
        }
        //方向控制——左
        private void btnLeft_MouseDown(object sender, MouseEventArgs e)
        {
            messagesend = pelcod.CameraPan(addressin, PelcoD.Pan.Left, speedin);
            serialPort.Open();
            serialPort.Write(messagesend, 0, 7);
            serialPort.Close();
        }
        //方向控制——右
        private void btnRight_MouseDown(object sender, MouseEventArgs e)
        {
            messagesend = pelcod.CameraPan(addressin, PelcoD.Pan.Right, speedin);
            serialPort.Open();
            serialPort.Write(messagesend, 0, 7);
            serialPort.Close();
        }
        //可通过鼠标在画面中按下时移动鼠标来实现移动操作。
        //获取鼠标焦点，循环检测鼠标焦点
        /*

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point pt);//获取鼠标焦点

        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int x, int y);//设置鼠标焦点

        const int MOUSEEVENTF_MOVE = 0x0001;      //移动鼠标 
        const int MOUSEEVENTF_LEFTDOWN = 0x0002; //模拟鼠标左键按下 
        const int MOUSEEVENTF_LEFTUP = 0x0004; //模拟鼠标左键抬起 
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008; //模拟鼠标右键按下 
        const int MOUSEEVENTF_RIGHTUP = 0x0010; //模拟鼠标右键抬起 
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; //模拟鼠标中键按下 
        const int MOUSEEVENTF_MIDDLEUP = 0x0040; //模拟鼠标中键抬起 
        const int MOUSEEVENTF_ABSOLUTE = 0x8000; //标示是否采用绝对坐标

        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Cursor.Position.X, Cursor.Position.Y, 0, 0);//Cursor.Position.X, Cursor.Position.Y指示鼠标所在绝对位置
        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 800, 450, 0, 0);//(800*450)鼠标左键点击屏幕中央

        private Point _mousePoint;
        private Boolean ok = flase; //记录是否是MouseDown的操作

        private void xxx_MouseDown(object sender, MouseEventArgs e)
        {
            Control cc = (Control)sender;
            if (e.Button == MouseButtons.Left)
            {
                _mousePoint.X = e.X + leftA(cc);
                _mousePoint.Y = e.Y + topA(cc);
            }
            ok = true;
        }

        private void xxx_MouseMove(object sender, MouseEventArgs e)
        {
            if (ok = true)
            {
                Control cc = (Control)sender;
                if (e.Button == MouseButtons.Left)
                {
                    if(jdz(e.X, _mousePoint.X) > jdz(e.Y, _mousePoint .Y))
                        //横向移动摄像头
                    elseif(jdz(e.X, _mousePoint.X) < jdz(e.Y, _mousePoint .Y))
                        //纵向移动摄像头
                }
            }
            ok = false;
        }

        public int jdz(int x, int y)
        {
            //取绝对值
            if(x - y >= 0)
                return x-y;
            else 
                rertun y-x;
        }

 */
        #endregion

        //打开监控管理窗体
        private void btnSetMonitor_Click(object sender, EventArgs e)
        {
            //分权限设置用户组，非权力用户不可更改用户列表
            frmSetMonitor frmsetmonitor = new frmSetMonitor();
            frmsetmonitor.ShowDialog();
        }
        //录像
        private void btnVideo_Click(object sender, EventArgs e)
        {
            if (btnVideo.Text == "录像")
            {
                sfDialog.Filter = "*.avi|*.avi";
                sfDialog.Title = "保存视频文件";
                sfDialog.InitialDirectory = Application.StartupPath.Substring(0, Application.StartupPath.LastIndexOf("\\")).Substring(0, Application.StartupPath.Substring(0, Application.StartupPath.LastIndexOf("\\")).LastIndexOf("\\")) + "\\Video\\";
                if (sfDialog.ShowDialog() == DialogResult.OK)
                {
                    btnVideo.Text = "停止录像";
                    VideoOperate.VCASetKeyFrmInterval(0, 250);  //设置MPEG压缩的关键帧间隔
                    VideoOperate.VCASetBitRate(0, 256); //设置MPEG压缩的位率
                    VideoOperate.VCASetVidCapFrameRate(0, 25, false);   //设置视频捕获频率
                    VideoOperate.VCASetVidCapSize(0, 320, 240); //设置视频捕获尺寸，此处即为宽320高240
                    VideoOperate.VCASetXVIDQuality(0, 10, 3);   //设置MPEG4_XVID压缩的质量
                    VideoOperate.VCASetXVIDCompressMode(0, VideoOperate.COMPRESSMODE.XVID_VBR_MODE);    //设置MPEG4_XVID压缩的模式
                    VideoOperate.VCAStartVideoCapture(0, VideoOperate.CAPMODEL.CAP_MPEG4_STREAM, VideoOperate.MP4MODEL.MPEG4_AVIFILE_CALLBACK, sfDialog.FileName);  //开始视频捕获
                }
            }
            else if (btnVideo.Text == "停止录像")
            {
                btnVideo.Text = "录像";
                VideoOperate.VCAStopVideoCapture(0);    //停止视频捕获
            }
        }
        //回放
        private void btnPlay_Click(object sender, EventArgs e)
        {
            frmPlay frmpaly = new frmPlay();
            frmpaly.ShowDialog();
        }
        //快照
        private void btnSnapShots_Click(object sender, EventArgs e)
        {
            if (rbtnBMP.Checked)
            {
                VideoOperate.VCASaveAsBmpFile(0, Application.StartupPath.Substring(0, Application.StartupPath.LastIndexOf("\\")).Substring(0, Application.StartupPath.Substring(0, Application.StartupPath.LastIndexOf("\\")).LastIndexOf("\\")) + "\\Photo\\" + DateTime.Now.ToFileTime() + ".bmp");
            }
            else if (rbtnJPG.Checked)
            {
                VideoOperate.VCASaveAsJpegFile(0, Application.StartupPath.Substring(0, Application.StartupPath.LastIndexOf("\\")).Substring(0, Application.StartupPath.Substring(0, Application.StartupPath.LastIndexOf("\\")).LastIndexOf("\\")) + "\\Photo\\" + DateTime.Now.ToFileTime() + ".jpg", 100);
            }
            else 
            {
                return ;
            }
        }
        //开始自动监控
        private void btnAutoMonitor_Click(object sender, EventArgs e)
        {
            if (btnAutoMonitor.Text == "开始")
            {
                if (rbtnWideWatch.Checked)
                {
                    messagesend = pelcod.CameraTilt(addressin, PelcoD.Tilt.Up, speedin);
                    serialPort.Open();
                    serialPort.Write(messagesend, 0, 7);
                    serialPort.Close();
                    Thread.Sleep(2000);
                    messagesend = pelcod.CameraPan(addressin, PelcoD.Pan.Left, speedin);
                    serialPort.Open();
                    serialPort.Write(messagesend, 0, 7);
                    serialPort.Close();
                    Thread.Sleep(2000);
                    messagesend = pelcod.CameraTilt(addressin, PelcoD.Tilt.Down, speedin);
                    serialPort.Open();
                    serialPort.Write(messagesend, 0, 7);
                    serialPort.Close();
                    Thread.Sleep(2000);
                    messagesend = pelcod.CameraPan(addressin, PelcoD.Pan.Right, speedin);
                    serialPort.Open();
                    serialPort.Write(messagesend, 0, 7);
                    serialPort.Close();
                }
                else if (rbtnVerticalWatch.Checked)
                {
                    messagesend = pelcod.CameraTilt(addressin, PelcoD.Tilt.Up, speedin);
                    serialPort.Open();
                    serialPort.Write(messagesend, 0, 7);
                    serialPort.Close();
                    Thread.Sleep(2000);
                    messagesend = pelcod.CameraTilt(addressin, PelcoD.Tilt.Down, speedin);
                    serialPort.Open();
                    serialPort.Write(messagesend, 0, 7);
                    serialPort.Close();
                }
                else if (rbtnLevelWatch.Checked)
                {
                    messagesend = pelcod.CameraPan(addressin, PelcoD.Pan.Left, speedin);
                    serialPort.Open();
                    serialPort.Write(messagesend, 0, 7);
                    serialPort.Close();
                    Thread.Sleep(2000);
                    messagesend = pelcod.CameraPan(addressin, PelcoD.Pan.Right, speedin);
                    serialPort.Open();
                    serialPort.Write(messagesend, 0, 7);
                    serialPort.Close();
                }
                btnAutoMonitor.Text = "停止";
            }
            else
            {
                stopMove();
                btnAutoMonitor.Text = "开始";
            }
        }
        //停止监控
        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {

                if (btnStop.Text == "停止")
                {
                    string strDPath = Application.StartupPath;
                    string strPath = strDPath.Substring(0, strDPath.LastIndexOf("\\")).Substring(0, strDPath.Substring(0, strDPath.LastIndexOf("\\")).LastIndexOf("\\")) + "\\Image\\主页面\\主界面图片.bmp";
                    plVideo1.BackgroundImage = System.Drawing.Image.FromFile(strPath);
                    VideoOperate.VCAUnInitSdk();
                    btnStop.Text = "开始";
                }
                else if (btnStop.Text == "开始")
                {
                    plVideo1.BackgroundImage = null;
                    startMonitor();
                    btnStop.Text = "停止";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("非法错误。"+ex.ToString());
            }
        }
        //打开软件注册窗体
        private void btnReg_Click(object sender, EventArgs e)
        {
            frmRegister frmregister = new frmRegister();
            frmregister.Show();
            this.Hide();
        }

        //鼠标不可设置为Click单击事件，否则视频将一直移动
        //鼠标设置为MouseDown、MouseUp事件后，仅当鼠标按下时移动，鼠标松起时停止。
        

        #region  释放鼠标时，视频监控停止移动
        private void btnUp_MouseUp(object sender, MouseEventArgs e)
        {
            stopMove();
        }
        private void btnDown_MouseUp(object sender, MouseEventArgs e)
        {
            stopMove();
        }
        private void btnLeft_MouseUp(object sender, MouseEventArgs e)
        {
            stopMove();
        }
        private void btnRight_MouseUp(object sender, MouseEventArgs e)
        {
            stopMove();
        }
        private void btnAHighlghts_MouseUp(object sender, MouseEventArgs e)
        {
            stopMove();
        }
        private void btnCHighlghts_MouseUp(object sender, MouseEventArgs e)
        {
            stopMove();
        }
        private void btnAFocus_MouseUp(object sender, MouseEventArgs e)
        {
            stopMove();
        }
        private void btnCFocus_MouseUp(object sender, MouseEventArgs e)
        {
            stopMove();
        }
        private void btnAAperture_MouseUp(object sender, MouseEventArgs e)
        {
            stopMove();
        }
        private void btnCAperture_MouseUp(object sender, MouseEventArgs e)
        {
            stopMove();
        }
        private void btnAWipers_MouseUp(object sender, MouseEventArgs e)
        {
            stopMove();
        }
        private void btnCWipers_MouseUp(object sender, MouseEventArgs e)
        {
            stopMove();
        }
        #endregion

        //关闭主窗体时，退出应用程序
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        //开始监控
        protected void startMonitor()   
        {
            if (VideoOperate.VCAInitSdk(this.Handle, VideoOperate.DISPLAYTRANSTYPE.PCI_MEMORY_VIDEOMEMORY, false))
            {
                m_dwDevNum = VideoOperate.VCAGetDevNum();
                if (m_dwDevNum == 0)
                {
                    MessageBox.Show("VC404卡驱动程序没有安装");
                }
                else
                {
                    for (int i = 0; i < m_dwDevNum; i++)
                    {
                        VideoOperate.VCAOpenDevice(i, plVideo1.Handle);
                        VideoOperate.VCAStartVideoPreview(i);
                    }
                }
            }
        }
        //停止移动
        protected void stopMove()
        {
            messagesend = pelcod.CameraStop(addressin);
            serialPort.Open();
            serialPort.Write(messagesend, 0, 7);
            serialPort.Close();
        }
    }
}