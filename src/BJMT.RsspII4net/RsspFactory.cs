/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-28 8:59:40 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.Config;

namespace BJMT.RsspII4net
{
    /// <summary>
    /// ʹ��RSSP-IIͨѶ�Ľڵ㹤���ࡣ
    /// </summary>
    public static class RsspFactory
    {
        /// <summary>
        /// ����һ��RSSP-II�ͻ��ˡ�
        /// </summary>
        /// <param name="config">�ͻ���������Ϣ��</param>
        /// <returns>һ��IRsspNode�ӿڡ�</returns>
        public static IRsspNode CreateClientNode(RsspClientConfig config)
        {
            if (config.ServiceType != ServiceType.D)
            {
                throw new ArgumentException("ֻ֧��D��������͡�");
            }

            return new RsspNodeClient(config);
        }

        /// <summary>
        /// ����һ��RSSP-II��������
        /// </summary>
        /// <param name="config">������������Ϣ��</param>
        /// <returns>һ��IRsspNode�ӿڡ�</returns>
        public static IRsspNode CreateServerNode(RsspServerConfig config)
        {
            return new RsspNodeServer(config);
        }
    }
}
