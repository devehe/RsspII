/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-14 9:09:44 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using BJMT.RsspII4net.MASL;
using BJMT.RsspII4net.SAI.EC;
using BJMT.RsspII4net.Utilities;
using BJMT.RsspII4net.Infrastructure.Services;

namespace BJMT.RsspII4net.SAI
{
    interface ISaiStateContext
    {
        SaiState CurrentState { get; set; }

        bool Connected { get; set; }

        RsspEndPoint RsspEP { get; }

        SeqNoManager SeqNoManager { get; }

        MaslConnection NextLayer { get; }

        DefenseStrategy DefenseStrategy { get; set; }

        ISaiFrameTransport FrameTransport { get; }

        ISaiConnectionObserver Observer { get; }
        
        void StartHandshakeTimer();
        void StopHandshakeTimer();
    }
}
