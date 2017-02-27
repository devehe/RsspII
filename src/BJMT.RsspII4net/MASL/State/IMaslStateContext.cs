/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-13 8:41:06 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.ALE;
using BJMT.RsspII4net.Infrastructure.Services;

namespace BJMT.RsspII4net.MASL.State
{
    interface IMaslStateContext
    {
        MaslState CurrentState { get; set; }

        bool Connected { get; set; }
        IMaslConnectionObserver Observer { get; }

        IMacCalculator MacCalculator { get; }

        IAuMessageBuilder AuMessageBuilder { get; }
        IAuMessageMacCaculator AuMessageMacCalculator { get; }

        RsspEndPoint RsspEP { get; }
        AleConnection AleConnection { get; }

        void StartHandshakeTimer();
        void StopHandshakeTimer();
    }
}
