/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-8 10:16:18 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net.MASL.Frames
{
    /// <summary>
    /// Disconnect frame.
    /// </summary>
    class MaslDiFrame : MaslFrame
    {
        /// <summary>
        /// ���û�������Ĺ̶����ȡ�
        /// </summary>
        private const int FrameLen = 3;

        /// <summary>
        /// �Ͽ���ԭ��
        /// </summary>
        public byte MajorReason { get; private set; }
        
        /// <summary>
        /// �Ͽ���ԭ��
        /// </summary>
        public byte MinorReason { get; private set; }
        
        /// <summary>
        /// ���캯����
        /// </summary>
        public MaslDiFrame()
        {

        }

        public MaslDiFrame(MaslFrameDirection direction, byte majorReason, byte minorReason)
            : base(MaslFrameType.DI, direction, 0)
        {
            this.MajorReason = majorReason;
            this.MinorReason = minorReason;
        }

        public override byte[] GetBytes()
        {
            int index = 0;
            var bytes = new byte[MaslDiFrame.FrameLen];

            // ETY + MTI + DF
            bytes[index++] = this.GetHeaderByte();

            // Major reason.
            bytes[index++] = this.MajorReason;

            // Minor reason.
            bytes[index++] = this.MinorReason;

            return bytes;
        }

        public override void ParseBytes(byte[] bytes, int startIndex, int endIndex)
        {
            // ETY + MTI + DF
            this.ParseHeaderByte(bytes[startIndex++]);

            // Major reason.
            this.MajorReason = bytes[startIndex++];

            // Minor reason.
            this.MinorReason = bytes[startIndex++];
        }

    }
}
