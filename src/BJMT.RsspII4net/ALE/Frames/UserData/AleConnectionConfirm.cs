/*----------------------------------------------------------------
// ��˾���ƣ�????�Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-18 13:46:27 
// ��    �䣺heng222_z@163.com
//
// Copyright (C) ????�Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.ALE.Frames
{
    /// <summary>
    /// ALE������ȷ��ʱʹ�õ�����
    /// </summary>
    class AleConnectionConfirm : AleUserData
    {
        /// <summary>
        /// Ӧ�𷽱�š�
        /// </summary>
        public UInt32 ServerID { get; set; }
        
        /// <summary>
        /// �û����ݣ���AU2 SaPDU��
        /// </summary>
        public byte[] UserData { get; set; }

        public override ushort Length
        {
            get
            {
                int value = 4;
                if (this.UserData != null)
                {
                    value += this.UserData.Length;
                }

                return (ushort)value;
            }
        }

        
        public AleConnectionConfirm()
            : base(AleFrameType.ConnectionConfirm)
        {

        }


        public AleConnectionConfirm(uint serverID, byte[] userData)
            : this()
        {
            this.ServerID = serverID;
            this.UserData = userData;
        }

        public override byte[] GetBytes()
        {
            var bytes = new byte[this.Length];
            int startIndex = 0;

            // Ӧ�𷽱��
            var tempBuf = RsspEncoding.ToNetUInt32(this.ServerID);
            Array.Copy(tempBuf, 0, bytes, startIndex, tempBuf.Length);
            startIndex += 4;

            // �û�����
            if (this.UserData != null)
            {
                Array.Copy(this.UserData, 0, bytes, startIndex, this.UserData.Length);
            }

            return bytes;
        }

        public override void ParseBytes(byte[] bytes, int startIndex, int endIndex)
        {
            // Ӧ�𷽱��
            this.ServerID = RsspEncoding.ToHostUInt32(bytes, startIndex);
            startIndex += 4;

            // �û�����
            var len = endIndex - startIndex + 1;
            if (len > 0)
            {
                this.UserData = new byte[len];
                Array.Copy(bytes, startIndex, this.UserData, 0, len);
            }
        }
    }
}
