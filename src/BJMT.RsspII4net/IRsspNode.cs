/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-29 9:52:29 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using BJMT.RsspII4net.Events;

namespace BJMT.RsspII4net
{
    /// <summary>
    /// һ���ӿڣ���������ʹ��RSSP-IIЭ��Ľڵ㡣
    /// </summary>
    public interface IRsspNode : IDisposable
    {
        /// <summary>
        /// ��RSSP-II�ڵ㡣
        /// </summary>
        void Open();

        /// <summary>
        /// �ر�RSSP-II�ڵ㲢�ͷ��������ӡ�
        /// </summary>
        void Close();

        /// <summary>
        /// �������ݡ�
        /// </summary>
        void Send(OutgoingPackage package);

        /// <summary>
        /// �ر���ָ���豸������ӡ�
        /// </summary>
        /// <param name="remoteID">��Ҫ�رյ��豸��</param>
        void Disconnect(UInt32 remoteID);

        /// <summary>
        /// ��ȡ�����ӵ��豸��š�
        /// </summary>
        List<uint> GetConnectedNodeID();

        /// <summary>
        /// һ���¼���������־����ʱ������
        /// </summary>
        event EventHandler<LogCreatedEventArgs> LogCreated;        

        /// <summary>
        /// һ���¼�����TCP/IP���ڽ�������ʱ������
        /// </summary>
        event EventHandler<TcpConnectingEventArgs> TcpConnecting;
        /// <summary>
        /// һ���¼�����TCP/IP���ӳɹ�ʱ������
        /// </summary>
        event EventHandler<TcpConnectedEventArgs> TcpConnected;
        /// <summary>
        /// һ���¼�����TCP/IP����ʧ��ʱ������
        /// </summary>
        event EventHandler<TcpConnectFailedEventArgs> TcpConnectFailed;
        /// <summary>
        /// һ���¼�����TCP/IP���ӶϿ�ʱ������
        /// </summary>
        event EventHandler<TcpDisconnectedEventArgs> TcpDisconnected;
        /// <summary>
        /// һ���¼�����TCP/IP�ս��㿪ʼ����ʱ������
        /// </summary>
        event EventHandler<TcpEndPointListeningEventArgs> TcpEndPointListening;
        /// <summary>
        /// һ���¼�����TCP/IP�ս�����ʧ��ʱ������
        /// </summary>
        event EventHandler<TcpEndPointListenFailedEventArgs> TcpEndPointListenFailed;

        /// <summary>
        /// һ���¼������ڵ�����ӽ���ʱ������
        /// </summary>
        event EventHandler<NodeConnectedEventArgs> NodeConnected;
        /// <summary>
        /// һ���¼������ڵ�����ӶϿ�ʱ������
        /// </summary>
        event EventHandler<NodeInterruptionEventArgs> NodeDisconnected;

        /// <summary>
        /// һ���¼������û����ݵ���ʱ������
        /// </summary>
        event EventHandler<UserDataIncomingEventArgs> UserDataIncoming;
        /// <summary>
        /// һ���¼������û����ݷ���ʱ������
        /// </summary>
        event EventHandler<UserDataOutgoingEventArgs> UserDataOutgoing;

        /// <summary>
        /// һ���¼��������ͻ����еĸ��������仯ʱ������
        /// </summary>
        event EventHandler<OutgoingCacheCountChangedEventArgs> OutgoingCacheCountChanged;
        /// <summary>
        /// һ���¼��������ջ����еĸ��������仯ʱ������
        /// </summary>
        event EventHandler<IncomingCacheCountChangedEventArgs> IncomingCacheCountChanged;


        /// <summary>
        /// һ���¼��������ն��з����ӳ�����ʱ������
        /// </summary>
        event EventHandler<IncomingCacheDelayedEventArgs> IncomingCacheDelayed;
        /// <summary>
        /// һ���¼��������Ͷ��з����ӳ�����ʱ������
        /// </summary>
        event EventHandler<OutgoingCacheDelayedEventArgs> OutgoingCacheDelayed;
    }
}
