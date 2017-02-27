/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-18 16:01:58 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net.SAI.TTS.Frames
{
    abstract class SaiTtsFrame : SaiFrame
    {
        /// <summary>
        /// ��ȡ/���÷��ͷ���ǰʱ���
        /// </summary>
        public UInt32 SenderTimestamp { get; set; }

        /// <summary>
        /// ��ȡ/���ý��շ���һ�εķ���ʱ���
        /// </summary>
        public UInt32 ReceiverLastSendTimestamp { get; set; }

        /// <summary>
        /// ��ȡ/���÷��ͷ��յ���һ����Ϣʱ��ʱ���
        /// </summary>
        public UInt32 SenderLastRecvTimestamp { get; set; }

        protected SaiTtsFrame(SaiFrameType type, ushort seqNo, 
            uint senderTimestamp, uint receiverLastSendTimestamp, uint senderLastRecvTimestamp)
            : base(type, seqNo)
        {
            this.SenderTimestamp = senderTimestamp;
            this.ReceiverLastSendTimestamp = receiverLastSendTimestamp;
            this.SenderLastRecvTimestamp = senderLastRecvTimestamp;
        }
    }
}
