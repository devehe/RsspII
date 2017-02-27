/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-27 10:37:32 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.SAI.Events;

namespace BJMT.RsspII4net.SAI
{
    interface ISaiFrameTransport
    {
        uint NextSendSeq();

        void SendSaiFrame(SaiFrame saiFrame);

        event EventHandler<SaiFrameIncomingEventArgs> SaiFrameReceived;
    }
}
