/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-29 14:44:25 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.ALE.Frames;

namespace BJMT.RsspII4net.ALE
{
    interface IAleTunnelObserver
    {
        /// <summary>
        /// ��TCP���ӶϿ�ʱ��
        /// </summary>
        /// <param name="theConnection"></param>
        /// <param name="reason"></param>
        void OnTcpDisconnected(AleTunnel theConnection, string reason);

        /// <summary>
        /// ���յ�ALEЭ��֡ʱ��
        /// </summary>
        /// <param name="theConnection"></param>
        /// <param name="theFrame"></param>
        void OnAleFrameArrival(AleTunnel theConnection, AleFrame theFrame);
    }

    interface IAleClientTunnelObserver : IAleTunnelObserver
    {
        /// <summary>
        /// ��TCP�ͻ����������ӷ�����ʱ��
        /// </summary>
        /// <param name="theConnection"></param>
        void OnTcpConnecting(AleClientTunnel theConnection);

        /// <summary>
        /// ��TCP�ͻ������ӵ�������ʱ��
        /// </summary>
        void OnTcpConnected(AleClientTunnel theConnection);

        /// <summary>
        /// ��TCP�ͻ������ӵ�������ʧ��ʱ��
        /// </summary>
        /// <param name="theConnection"></param>
        /// <param name="reason"></param>
        void OnTcpConnectFailure(AleClientTunnel theConnection, string reason);
    }

    interface IAleServerTunnelObserver : IAleTunnelObserver
    {

    }
}
