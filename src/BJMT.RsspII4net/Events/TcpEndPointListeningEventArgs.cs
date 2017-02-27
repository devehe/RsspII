/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-29 13:17:32 
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
    /// һ���¼������࣬��������TCP�ս�����ڼ����¼������ݡ�
    /// </summary>
    public class TcpEndPointListeningEventArgs : EventArgs
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="localID">���ؽڵ�ID��</param>
        /// <param name="endPoint">���ڼ������ս�㡣</param>
        public TcpEndPointListeningEventArgs(uint localID, IPEndPoint endPoint)
        {
            this.LocalID = localID;
            this.EndPoint = endPoint;
        }

        /// <summary>
        /// ���ؽڵ��š�
        /// </summary>
        public uint LocalID { get; private set; }

        /// <summary>
        /// ��ȡ��ǰ���ڼ������ս�㡣
        /// </summary>
        public IPEndPoint EndPoint { get; private set; } 
    }
}
