/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-13 8:40:16 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.Infrastructure.Services;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.ALE.State
{
    interface IAleStateContext
    {
        AleState CurrentState { get; set; }

        bool Connected { get; }

        IAleConnectionObserver Observer { get; }
        RsspEndPoint RsspEP { get; }

        IAuMessageBuilder AuMsgBuilder { get; }

        IAleTunnelEventNotifier TunnelEventNotifier { get; }
        SeqNoManager SeqNoManager { get; }
        IEnumerable<AleTunnel> Tunnels { get; }

        bool ContainsTunnel(AleTunnel tunnel);
        void AddConnection(AleTunnel item);
        void IncreaseValidConnection();
        void DescreaseValidConnection();
        void RemoveCloseConnection(AleTunnel theConnection);
    }
}
