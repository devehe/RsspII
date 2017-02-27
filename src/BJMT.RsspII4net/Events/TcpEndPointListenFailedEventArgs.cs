/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-29 13:17:56 
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
    /// һ���¼������࣬��������TCP�ս�����ʧ���¼������ݡ�
    /// </summary>
    public class TcpEndPointListenFailedEventArgs : EventArgs
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="localID">���ؽڵ�ID��</param>
        /// <param name="endPoint">����ʧ�ܵ��ս�㡣</param>
        public TcpEndPointListenFailedEventArgs(uint localID, IPEndPoint endPoint)
        {
            this.LocalID = localID;
            this.EndPoint = endPoint;
        }

        /// <summary>
        /// ���ؽڵ��š�
        /// </summary>
        public uint LocalID { get; private set; }

        /// <summary>
        /// ��ȡ��ǰ����ʧ�ܵ��ս�㡣
        /// </summary>
        public IPEndPoint EndPoint { get; private set; } 

    }
}
