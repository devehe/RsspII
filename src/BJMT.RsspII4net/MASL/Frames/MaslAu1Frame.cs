/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-8 10:13:10 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using BJMT.RsspII4net.Exceptions;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.MASL.Frames
{
    /// <summary>
    /// First Authentication message Frame.
    /// </summary>
    class MaslAu1Frame : MaslFrame
    {
        private const int FrameLength = 13;

        /// <summary>
        /// ����ID�ĺ������ֽڡ�
        /// </summary>
        public uint ClientID { get; set; }

        /// <summary>
        /// ʹ�õļ����㷨��
        /// </summary>
        public EncryptionAlgorithm EncryAlgorithm { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        public byte[] RandomB { get; private set; }

        /// <summary>
        /// ���캯����
        /// </summary>
        public MaslAu1Frame()
        {
            this.RandomB = new byte[8];
        }

        public MaslAu1Frame(byte initiatorType, uint clientID, EncryptionAlgorithm encryAlgorithm, byte[] randomB)
            :base(MaslFrameType.AU1, MaslFrameDirection.Client2Server, initiatorType)
        {
            if (randomB == null || randomB.Length != 8)
            {
                throw new ArgumentException("RandomB�ĳ��ȱ���Ϊ8�ֽڡ�");
            }

            this.ClientID = clientID;
            this.EncryAlgorithm = encryAlgorithm;
            this.RandomB = randomB;
        }

        public override byte[] GetBytes()
        {
            int index = 0;
            var bytes = new byte[MaslAu1Frame.FrameLength];

            // ETY + MTI + DF
            bytes[index++] = this.GetHeaderByte();

            // ��Ϣ����ID
            var tempBuf = RsspEncoding.ToNetUInt32(this.ClientID);
            Array.Copy(tempBuf, 1, bytes, index, 3);
            index += 3;

            // ��ȫ����
            bytes[index++] = (byte)this.EncryAlgorithm;

            // �����
            Array.Copy(this.RandomB, 0, bytes, index, 8);

            return bytes;
        }

        public override void ParseBytes(byte[] bytes, int startIndex, int endIndex)
        {
            if ((endIndex - startIndex + 1) < MaslAu1Frame.FrameLength)
            {
                throw new Au1LengthException();
            }

            var orgStartIndex = startIndex;

            // ETY + MTI + DF
            this.ParseHeaderByte(bytes[startIndex++]);

            // ��Ϣ����ID
            this.ClientID = (RsspEncoding.ToHostUInt32(bytes, orgStartIndex) & 0xFFFFFF);
            startIndex += 3;

            // ��ȫ����
            this.EncryAlgorithm = (EncryptionAlgorithm)bytes[startIndex++];

            // �����
            Array.Copy(bytes, startIndex, this.RandomB, 0, 8);
        }
    }
}
