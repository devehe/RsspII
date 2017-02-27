/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-29 15:01:52 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net.Infrastructure.Services
{
    interface IMaslConnectionObserver
    {
        /// <summary>
        /// ֪ͨMasl�����ӽ�����
        /// </summary>
        void OnMaslConnected();

        void OnMaslDisconnected();

        void OnMaslUserDataArrival(byte[] maslUserData);
    }
}
