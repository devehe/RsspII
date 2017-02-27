/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-5 13:08:33 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace BJMT.RsspII4net.Events
{
    /// <summary>
    /// TCP�¼�����
    /// </summary>
    public abstract class TcpEventArgs : EventArgs
    {
        /// <summary>
        /// ���캯����
        /// </summary>
        public TcpEventArgs(string connectionID, 
            uint localID, IPEndPoint localEP,
            uint remoteID, IPEndPoint remoteEP)
        {
            this.ConnectionID = connectionID;

            this.LocalID = localID;
            this.LocalEndPoint = localEP;
            this.RemoteID = remoteID;
            this.RemoteEndPoint = remoteEP;
        }

        /// <summary>
        /// ��ȡTCP���ӵ�Ψһ��ʶ����
        /// </summary>
        public string ConnectionID { get; private set; }

        /// <summary>
        /// ��ȡ����ID��
        /// </summary>
        public uint LocalID { get; private set; }
        /// <summary>
        /// ��ȡ�����ս�㡣
        /// </summary>
        public IPEndPoint LocalEndPoint { get; private set; }

        /// <summary>
        /// ��ȡ�Է�ID��
        /// </summary>
        public uint RemoteID { get; private set; }
        /// <summary>
        /// ��ȡ�Է��ս�㡣
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; private set; }

    }
}
