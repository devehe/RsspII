/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-13 14:25:25 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace BJMT.RsspII4net
{
    interface INodeListenerObserver
    {
        void OnEndPointListening(TcpListener listener);
        void OnEndPointListenFailed(TcpListener listener, string message);

        void OnAcceptTcpClient(TcpClient tcpClient);
    }
}
