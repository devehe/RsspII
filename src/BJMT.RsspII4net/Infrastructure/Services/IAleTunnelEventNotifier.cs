/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-29 13:52:09 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.Events;

namespace BJMT.RsspII4net.Infrastructure.Services
{
    interface IAleTunnelEventNotifier
    {
        void NotifyTcpConnecting(TcpConnectingEventArgs args);
        void NotifyTcpConnected(TcpConnectedEventArgs args);
        void NotifyTcpConnectFailure(TcpConnectFailedEventArgs args);
        void NotifyTcpDisconnected(TcpDisconnectedEventArgs args);
    }
}
