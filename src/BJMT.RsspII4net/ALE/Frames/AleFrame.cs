/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-17 11:08:52 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using BJMT.RsspII4net.Exceptions;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.ALE.Frames
{
    /// <summary>
    /// ALEPKT��Ϣ�ṹ��
    /// �μ���RSSP-II��·�źŰ�ȫͨ��Э�顷P65��
    /// </summary>
    class AleFrame
    {
        /// <summary>
        /// ALE���ײ����ȡ�
        /// </summary>
        public const byte HeadLength = 10;

        /// <summary>
        /// �ײ�����ռ�õ��ֽڸ�����
        /// </summary>
        public const byte SizeofHeadLen = 2;

        #region "Filed"
        #endregion

        #region "Constructor"
        public AleFrame()
        {
            this.Version = 0x01;
        }

        public AleFrame(byte appType, UInt16 seqNo, bool isNormal, AleUserData userData)
            :this()
        {
            if (userData == null)
            {
                throw new ArgumentNullException();
            }

            this.ApplicationType = appType;
            this.SequenceNo = seqNo;
            this.IsNormal = isNormal;
            this.FrameType = userData.FrameType;
            this.UserData = userData;
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// �����ȣ���ȥ���ֶκ��������ĳ��ȣ����ֽ�Ϊ��λ����
        /// </summary>
        public UInt16 PacketLength { get { return (UInt16)(this.UserDataLength + 8); } }

        /// <summary>
        /// �汾�š�
        /// </summary>
        public byte Version { get; set; }

        /// <summary>
        /// Ӧ�����͡�
        /// </summary>
        public byte ApplicationType { get; set; }

        /// <summary>
        /// ������š�
        /// </summary>
        public UInt16 SequenceNo { get; set; }

        /// <summary>
        /// N/R��־��true��ʾ������-���������ӣ�false��ʾ������-���ࡱ���ӡ�
        /// </summary>
        public bool IsNormal { get; set; }

        /// <summary>
        /// ֡���͡�
        /// </summary>
        public AleFrameType FrameType { get; set; }

        /// <summary>
        /// �û����ݡ�
        /// </summary>
        public AleUserData UserData { get; set; }

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
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"

        /// <summary>
        /// ��ȡ���л�����ֽ�����
        /// </summary>
        public byte[] GetBytes()
        {
            var bytes = new byte[AleFrame.HeadLength + this.UserDataLength];
            var startIndex = 0;

            // ������
            var tempBuf = RsspEncoding.ToNetUInt16(this.PacketLength);
            Array.Copy(tempBuf, 0, bytes, startIndex, tempBuf.Length);
            startIndex += AleFrame.SizeofHeadLen;

            // �汾
            bytes[startIndex++] = this.Version;

            // Ӧ������
            bytes[startIndex++] = this.ApplicationType;

            // �������к�
            tempBuf = RsspEncoding.ToNetUInt16(this.SequenceNo);
            Array.Copy(tempBuf, 0, bytes, startIndex, tempBuf.Length);
            startIndex += 2;

            // N/R��־
            bytes[startIndex++] = (byte)(this.IsNormal ? 1 : 0);

            // ������
            bytes[startIndex++] = (byte)this.FrameType;

            // У���
            var crcValue = CrcTool.CaculateCCITT16(bytes, 0, 8);
            //tempBuf = BitConverter.GetBytes(crcValue);
            tempBuf = RsspEncoding.ToNetUInt16(crcValue);
            Array.Copy(tempBuf, 0, bytes, startIndex, tempBuf.Length);
            startIndex += 2;

            // �û�����
            var userData = this.UserData.GetBytes();
            if (userData != null)
            {
                Array.Copy(userData, 0, bytes, startIndex, this.UserDataLength);
            }

            return bytes;
        }

        /// <summary>
        /// ����ָ�����ֽ�����
        /// </summary>
        public void ParseBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length < AleFrame.HeadLength)
            {
                throw new AleFrameParsingException("�޷���ָ�����ֽ�������ΪAleFrame�����Ȳ�����");
            }

            int startIndex = 0;

            // ������
            var pktLen = RsspEncoding.ToHostUInt16(bytes, startIndex);
            startIndex += AleFrame.SizeofHeadLen;

            // �汾
            this.Version = bytes[startIndex++];

            // Ӧ������
            this.ApplicationType = bytes[startIndex++];

            // �������к�
            this.SequenceNo = RsspEncoding.ToHostUInt16(bytes, startIndex);
            startIndex += 2;

            // N/R��־
            this.IsNormal = (bytes[startIndex++] == 1);

            // ������
            this.FrameType = (AleFrameType)bytes[startIndex++];

            // У���
            if (this.FrameType != AleFrameType.KAA && this.FrameType != AleFrameType.KANA) // ������Ϣ����ҪУ��
            {
                //var actualCrcValue = BitConverter.ToUInt16(bytes, startIndex);
                var actualCrcValue = RsspEncoding.ToHostUInt16(bytes, startIndex);
                var expectedCrcValue = CrcTool.CaculateCCITT16(bytes, 0, 8);
                if (actualCrcValue != expectedCrcValue)
                {
                    throw new AleFrameParsingException(string.Format("����AleЭ��֡ʱ�����쳣��CRC���鲻һ�£�����ֵ��{0}��ʵ��ֵ��{1}��",
                        expectedCrcValue, actualCrcValue));
                }
            }
            startIndex += 2;

            // �û�����
            var userDataLen = pktLen - (AleFrame.HeadLength - AleFrame.SizeofHeadLen);
            if (userDataLen < 0)
            {
                throw new AleFrameParsingException(string.Format("���ֽ�������ΪALE֡ʱ�����쳣���û����ݳ���Ϊ��ֵ{0}��", userDataLen));
            }
            this.UserData = AleUserData.Parse(this.FrameType, bytes, startIndex, startIndex + userDataLen - 1);
        }

        /// <summary>
        /// ��ָ�����ֽ�������ΪAleFrame��
        /// </summary>
        public static AleFrame Parse(byte[] bytes)
        {
            var frame = new AleFrame();
            
            frame.ParseBytes(bytes);

            return frame;
        }
        #endregion

    }
}
