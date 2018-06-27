using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace VWMS.CommonClass
{
    class PelcoD
    {
        private string watchdir = "";//监控方向
        private static readonly byte STX = 0xFF;//同步字节

        #region  监控方向和定时监控实体
        public string WatchDir
        {
            get
            {
                return watchdir;
            }
            set
            {
                watchdir = value;
            }
        }
        #endregion

        #region 基本指令定义
        #region 指令码1
        private const byte FocusNear = 0x01;//增加聚焦
        private const byte IrisOpen = 0x02;//减小光圈
        private const byte IrisClose = 0x04;//增加光圈
        private const byte CameraOnOff = 0x08;//摄像机打开和关闭
        private const byte AutoManualScan = 0x10;//自动和手动扫描
        private const byte Sense = 0x80;//Sence码
        #endregion

        #region  指令码2
        private const byte PanRight = 0x02;//右
        private const byte PanLeft = 0x04;//左
        private const byte TiltUp = 0x08;//上
        private const byte TiltDown = 0x10;//下
        private const byte ZoomTele = 0x20;//增加对焦
        private const byte ZoomWide = 0x40;//减小对焦
        private const byte FocusFar = 0x80;//减小聚焦
        #endregion

        #region 镜头左右平移的速度
        private const byte PanSpeedMin = 0x00;//停止
        private const byte PanSpeedMax = 0xFF;//最高速
        #endregion

        #region 镜头上下移动的速度
        private const byte TiltSpeedMin = 0x00;//停止
        private const byte TiltSpeedMax = 0x3F;//最高速
        #endregion
        #endregion

        #region 云台控制枚举
        public enum Switch { On = 0x01, Off = 0x02 }//雨刷控制
        public enum Focus { Near = FocusNear, Far = FocusFar }//聚焦控制
        public enum Zoom { Wide = ZoomWide, Tele = ZoomTele }//对焦控制
        public enum Tilt { Up = TiltUp, Down = TiltDown }//上下控制
        public enum Pan { Left = PanLeft, Right = PanRight }//左右控制
        public enum Scan { Auto, Manual }//自动和手动控制
        public enum Iris { Open = IrisOpen, Close = IrisClose }//光圈控制
        #endregion

        #region 云台控制方法
        //雨刷控制
        public byte[] CameraSwitch(uint deviceAddress, Switch action)
        {
            byte m_action = CameraOnOff;
            if (action == Switch.On)
                m_action = CameraOnOff + Sense;
            return Message.GetMessage(deviceAddress, m_action, 0x00, 0x00, 0x00);
        }
        //光圈控制
        public byte[] CameraIrisSwitch(uint deviceAddress, Iris action)
        {
            return Message.GetMessage(deviceAddress, (byte)action, 0x00, 0x00, 0x00);
        }
        //聚焦控制
        public byte[] CameraFocus(uint deviceAddress, Focus action)
        {
            if (action == Focus.Near)
                return Message.GetMessage(deviceAddress, (byte)action, 0x00, 0x00, 0x00);
            else
                return Message.GetMessage(deviceAddress, 0x00, (byte)action, 0x00, 0x00);
        }
        //对焦控制
        public byte[] CameraZoom(uint deviceAddress, Zoom action)
        {
            return Message.GetMessage(deviceAddress, 0x00, (byte)action, 0x00, 0x00);
        }
        //上下控制
        public byte[] CameraTilt(uint deviceAddress, Tilt action, uint speed)
        {
            if (speed < TiltSpeedMin)
                speed = TiltSpeedMin;
            if (speed > TiltSpeedMax)
                speed = TiltSpeedMax;
            return Message.GetMessage(deviceAddress, 0x00, (byte)action, 0x00, (byte)speed);
        }
        //左右控制
        public byte[] CameraPan(uint deviceAddress, Pan action, uint speed)
        {
            if (speed < PanSpeedMin)
                speed = PanSpeedMin;
            if (speed > PanSpeedMax)
                speed = PanSpeedMax; 
            return Message.GetMessage(deviceAddress, 0x00, (byte)action, (byte)speed, 0x00);
        }
        //停止云台的移动
        public byte[] CameraStop(uint deviceAddress)
        {
            return Message.GetMessage(deviceAddress, 0x00, 0x00, 0x00, 0x00);
        }
        //自动和手动控制
        public byte[] CameraScan(uint deviceAddress, Scan scan)
        {
            byte m_byte = AutoManualScan;
            if (scan == Scan.Auto)
                m_byte = AutoManualScan + Sense;
            return Message.GetMessage(deviceAddress, m_byte, 0x00, 0x00, 0x00);
        }
        #endregion

        public struct Message
        {
            public static byte Address;
            public static byte CheckSum;
            public static byte Command1, Command2, Data1, Data2;
            public static byte[] GetMessage(uint address, byte command1, byte command2, byte data1, byte data2)
            {
                if (address < 1 & address > 256)
                    throw new Exception("Pelco D协议只支持256设备");
                Address = Byte.Parse((address).ToString());
                Data1 = data1;
                Data2 = data2;
                Command1 = command1;
                Command2 = command2;
                CheckSum = (byte)( STX ^ Address ^ Command1 ^ Command2 ^ Data1 ^ Data2);
                return new byte[] { STX, Address, Command1, Command2, Data1, Data2, CheckSum };
            }
        }
    }
}
