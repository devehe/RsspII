/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-29 15:01:30 
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
    interface IAleConnectionObserver
    {
        /// <summary>
        /// ��ALE������ʱ���á�
        /// </summary>
        void OnAleConnected();

        /// <summary>
        /// ��ALE��Ͽ�ʱ���á�
        /// </summary>
        void OnAleDisconnected();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aleUserData"></param>
        void OnAleUserDataArrival(byte[] aleUserData);
    }
}
