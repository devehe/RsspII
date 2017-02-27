/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-29 15:02:35 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;

namespace BJMT.RsspII4net.Infrastructure.Services
{
    interface ISaiConnectionObserver
    {
        void OnSaiConnected(uint localID, uint remoteID);
        void OnSaiDisconnected(uint localID, uint remoteID);

        void OnSaiUserDataArrival(uint remoteID, byte[] userData, Int64 timeDelay, MessageDelayDefenseTech defenseTech);
    }
}
