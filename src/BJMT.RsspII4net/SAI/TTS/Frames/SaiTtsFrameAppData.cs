/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-18 16:38:27 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.SAI.TTS.Frames
{
    class SaiTtsFrameAppData : SaiTtsFrame
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public SaiTtsFrameAppData()
            :base(SaiFrameType.TTS_AppData, 0, 0, 0, 0)
        {
        }

        public SaiTtsFrameAppData(ushort seqNo,
            uint senderTimestamp, uint receiverLastSendTimestamp, uint senderLastRecvTimestamp, 
            byte[] userData)
            : base(SaiFrameType.TTS_AppData, seqNo, senderTimestamp, receiverLastSendTimestamp, senderLastRecvTimestamp)
        {
            this.UserData = userData;
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// �û����ݣ�Ϊ������ʱ��ʾ�����ݱ�Ϊʱ��ƫ�������������Ӧ���ġ�
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
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"

        public override byte[] GetBytes()
        {
            if (this.UserDataLength > SaiFrame.MaxUserDataLength)
            {
                throw new ArgumentException(string.Format("SAI���û����ݳ��Ȳ��ܳ���{0}��", SaiFrame.MaxUserDataLength));
            }

            var bytes = new byte[15 + this.UserDataLength];
            int startIndex = 0;

            // ��Ϣ����
            bytes[startIndex++] = (byte)this.FrameType;

            // ���к�
            var tempBuf = RsspEncoding.ToNetUInt16(this.SequenceNo);
            Array.Copy(tempBuf, 0, bytes, startIndex, tempBuf.Length);
            startIndex += 2;

            // sender timestamp
            tempBuf = RsspEncoding.ToNetUInt32(this.SenderTimestamp);
            Array.Copy(tempBuf, 0, bytes, startIndex, tempBuf.Length);
            startIndex += 4;

            // Responsor last timestamp
            tempBuf = RsspEncoding.ToNetUInt32(this.ReceiverLastSendTimestamp);
            Array.Copy(tempBuf, 0, bytes, startIndex, tempBuf.Length);
            startIndex += 4;

            // sender last timestamp
            tempBuf = RsspEncoding.ToNetUInt32(this.SenderLastRecvTimestamp);
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

            // sender timestamp
            this.SenderTimestamp = RsspEncoding.ToHostUInt32(bytes, startIndex);
            startIndex += 4;

            // Responsor last timestamp
            this.ReceiverLastSendTimestamp = RsspEncoding.ToHostUInt32(bytes, startIndex);
            startIndex += 4;

            // sender last timestamp
            this.SenderLastRecvTimestamp = RsspEncoding.ToHostUInt32(bytes, startIndex);
            startIndex += 4;

            // user data
            var len = bytes.Length - startIndex;
            if (len > 0)
            {
                this.UserData = new byte[len];
                Array.Copy(bytes, startIndex, this.UserData, 0, len);
            }
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
