/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-25 10:11:54 
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

namespace BJMT.RsspII4net.Config
{
    /// <summary>
    /// RSSP-II�������·�����ࡣ
    /// </summary>
    public class RsspTcpLinkConfig
    {
        /// <summary>
        /// �ͻ���IP��ַ��
        /// </summary>
        public IPAddress ClientIpAddress { get; set; }

        /// <summary>
        /// �������ս�㡣
        /// </summary>
        public IPEndPoint ServerEndPoint { get; set; }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="clientIP">�ͻ���IP��ַ��</param>
        /// <param name="serverEP">�������ս�㡣</param>
        public RsspTcpLinkConfig(IPAddress clientIP, IPEndPoint serverEP)
        {
            this.ClientIpAddress = clientIP;
            this.ServerEndPoint = serverEP;
        }
    }
}
