/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-8 10:15:11 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using BJMT.RsspII4net.Exceptions;

namespace BJMT.RsspII4net.MASL.Frames
{
    /// <summary>
    /// Data transmission frame.
    /// </summary>
    class MaslDtFrame : MaslFrame
    {
        /// <summary>
        /// ���û�������Ĺ̶����ȡ�
        /// </summary>
        private const int FixedLenExceptUserData = 9;

        /// <summary>
        /// �û����ݡ�
        /// </summary>
        public byte[] UserData { get; private set; }
        
        /// <summary>
        /// ��Ϣ��֤�롣
        /// </summary>
        public byte[] MAC { get; set; }

        /// <summary>
        /// ��ȡ�û����ݵĳ��ȡ�
        /// </summary>
        public int UserDataLen
        {
            get 
            { 
                if (this.UserData == null)
                {
                    return 0;
                }
                else
                {
                    return this.UserData.Length;
                }
            }
        }

        /// <summary>
        /// ���캯����
        /// </summary>
        public MaslDtFrame()
        {
            this.MAC = new byte[8];
        }

        public MaslDtFrame(MaslFrameDirection direction, byte[] userData)
            : base(MaslFrameType.DT, direction, 0)
        {
            this.UserData = userData;
        }

        public override byte[] GetBytes()
        {
            int index = 0;
            var bytes = new byte[FixedLenExceptUserData + this.UserDataLen];

            // ETY + MTI + DF
            bytes[index++] = this.GetHeaderByte();

            // UserData
            var uLen = this.UserDataLen;
            if (uLen != 0)
            {
                Array.Copy(this.UserData, 0, bytes, index, uLen);
                index += uLen;
            }

            // MAC
            Array.Copy(this.MAC, 0, bytes, index, 8);
            index += 8;

            return bytes;
        }

        public override void ParseBytes(byte[] bytes, int startIndex, int endIndex)
        {
            if ((endIndex - startIndex + 1) < MaslDtFrame.FixedLenExceptUserData)
            {
                throw new DtLengthException();
            }

            var totalLen = endIndex - startIndex + 1;

            // ETY + MTI + DF
            this.ParseHeaderByte(bytes[startIndex++]);

            // UserData
            var uLen = totalLen - FixedLenExceptUserData;
            this.UserData = new byte[uLen];
            Array.Copy(bytes, startIndex, this.UserData, 0, uLen);
            startIndex += uLen;

            // MAC
            Array.Copy(bytes, startIndex, this.MAC, 0, 8);
            startIndex += 8;
        }

    }
}
