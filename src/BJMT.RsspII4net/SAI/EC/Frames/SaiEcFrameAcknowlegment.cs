/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-18 15:49:33 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.SAI.EC.Frames
{
    class SaiEcFrameAcknowlegment : SaiEcFrameApplication
    {
        public SaiEcFrameAcknowlegment()
        {
            this.FrameType = SaiFrameType.EC_AppDataAcknowlegment;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        public SaiEcFrameAcknowlegment(ushort seqNo, uint ecValue, byte[] userData)
            :base(seqNo, ecValue, userData)
        {
            this.FrameType = SaiFrameType.EC_AppDataAcknowlegment;
        }
    }
}
