/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-18 15:46:33 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.SAI.EC.Frames
{
    class SaiEcFrameApplication : SaiEcFrame
    {
        /// <summary>
        /// �û�����
        /// </summary>
        public byte[] UserData { get; set; }

        /// <summary>
        /// ��ȡ�û����ݵĳ��ȡ�
        /// </summary>
        public int UserDataLength
        {
            get
            {
                if (this.UserData != null)
                {
                    return this.UserData.Length;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        public SaiEcFrameApplication()
            : base(SaiFrameType.EC_AppData, 0)
        {

        }

        /// <summary>
        /// ���캯��
        /// </summary>
        public SaiEcFrameApplication(ushort seqNo, uint ecValue, byte[] userData)
            : base(SaiFrameType.EC_AppData, seqNo)
        {
            this.EcValue = ecValue;
            this.UserData = userData;
        }

        public override byte[] GetBytes()
        {
            if (this.UserDataLength > SaiFrame.MaxUserDataLength)
            {
                throw new ArgumentException(string.Format("SAI���û����ݳ��Ȳ��ܳ���{0}��", SaiFrame.MaxUserDataLength));
            }

            var bytes = new byte[19 + this.UserDataLength];
            int startIndex = 0;

            // ��Ϣ����
            bytes[startIndex++] = (byte)this.FrameType;

            // ���к�
            var tempBuf = RsspEncoding.ToNetUInt16(this.SequenceNo);
            Array.Copy(tempBuf, 0, bytes, startIndex, tempBuf.Length);
            startIndex += 2;

            // Padding
            startIndex += SaiFrame.TtsPaddingLength;

            // EC����
            tempBuf = RsspEncoding.ToNetUInt32(this.EcValue);
            Array.Copy(tempBuf, 0, bytes, startIndex, tempBuf.Length);
            startIndex += 4;

            // user data
            if (this.UserData != null)
            {
                Array.Copy(this.UserData, 0, bytes, startIndex, this.UserData.Length);
            }

            return bytes;
        }

        public override void ParseBytes(byte[] bytes)
        {
            int startIndex = 0;

            // ��Ϣ����
            this.FrameType = (SaiFrameType)bytes[startIndex++];

            // ���к�
            this.SequenceNo = RsspEncoding.ToHostUInt16(bytes, startIndex);
            startIndex += 2;

            // Padding
            startIndex += SaiFrame.TtsPaddingLength;

            // EC����
            this.EcValue = RsspEncoding.ToHostUInt32(bytes, startIndex);
            startIndex += 4;

            // user data
            var len = bytes.Length - startIndex;
            if (len > 0)
            {
                this.UserData = new byte[len];
                Array.Copy(bytes, startIndex, this.UserData, 0, len);
            }
        }

    }
}
