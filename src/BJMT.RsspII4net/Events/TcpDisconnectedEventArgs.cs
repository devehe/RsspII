/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-29 13:16:39 
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
    /// һ���¼������࣬��������TCP�����ж��¼������ݡ�
    /// </summary>
    public class TcpDisconnectedEventArgs : TcpEventArgs
    {
        /// <summary>
        /// ���캯����
        /// </summary>
        public TcpDisconnectedEventArgs(string connectionID, 
            uint localID, IPEndPoint localEP,
            uint remoteID, IPEndPoint remoteEP)
            :base(connectionID, localID, localEP, remoteID, remoteEP)
        {
        }

    }
}
