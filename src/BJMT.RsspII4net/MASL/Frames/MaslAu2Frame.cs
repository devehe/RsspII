/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-8 10:14:00 
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
    class MaslAu2Frame : MaslFrame
    {
        private const int FrameLength = 21;

        /// <summary>
        /// ��ϢӦ��ID�ĺ������ֽڡ�
        /// </summary>
        public uint ServerID { get; set; }

        /// <summary>
        /// ʹ�õļ����㷨/��ȫ������
        /// </summary>
        public EncryptionAlgorithm EncryAlgorithm { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        public byte[] RandomA { get; private set; }

        /// <summary>
        /// ��Ϣ��֤�롣
        /// </summary>
        public byte[] MAC { get; set; }

        /// <summary>
        /// ���캯����
        /// </summary>
        public MaslAu2Frame()
        {
            this.RandomA = new byte[8];
            this.MAC = new byte[8];
        }

        public MaslAu2Frame(byte initiatorType, uint clientID, EncryptionAlgorithm encryAlgorithm, byte[] randomA)
            :base(MaslFrameType.AU2, MaslFrameDirection.Server2Client, initiatorType)
        {
            if (randomA == null || randomA.Length != 8)
            {
                throw new ArgumentException("RandomA�ĳ��ȱ���Ϊ8�ֽڡ�");
            }

            this.ServerID = clientID;
            this.EncryAlgorithm = encryAlgorithm;
            this.RandomA = randomA;
        }

        public override byte[] GetBytes()
        {
            int index = 0;
            var bytes = new byte[MaslAu2Frame.FrameLength];

            // ETY + MTI + DF
            bytes[index++] = this.GetHeaderByte();

            // ��Ϣ����ID
            var tempBuf = RsspEncoding.ToNetUInt32(this.ServerID);
            Array.Copy(tempBuf, 1, bytes, index, 3);
            index += 3;

            // ��ȫ����
            bytes[index++] = (byte)this.EncryAlgorithm;

            // �����
            Array.Copy(this.RandomA, 0, bytes, index, 8);
            index += 8;

            // MAC
            Array.Copy(this.MAC, 0, bytes, index, 8);
            index += 8;

            return bytes;
        }

        public override void ParseBytes(byte[] bytes, int startIndex, int endIndex)
        {
            if ((endIndex - startIndex + 1) < MaslAu2Frame.FrameLength)
            {
                throw new Au2LengthException();
            }

            var orgStartIndex = startIndex;

            // ETY + MTI + DF
            this.ParseHeaderByte(bytes[startIndex++]);

            // ��Ϣ����ID
            this.ServerID = (RsspEncoding.ToHostUInt32(bytes, orgStartIndex) & 0xFFFFFF);
            startIndex += 3;

            // ��ȫ����
            this.EncryAlgorithm = (EncryptionAlgorithm)bytes[startIndex++];

            // �����
            Array.Copy(bytes, startIndex, this.RandomA, 0, 8);
            startIndex += 8;

            // MAC
            Array.Copy(bytes, startIndex, this.MAC, 0, 8);
            startIndex += 8;
        }
    }
}
