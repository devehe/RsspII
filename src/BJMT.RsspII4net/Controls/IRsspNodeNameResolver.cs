/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ʒ���ƣ�BJMT-UR ATS
//
// �� �� �ˣ�zhh_217
// �������ڣ�2013-7-19 8:55:35 
// ��    �䣺zhh_217@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾ 2009����������Ȩ��
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;

namespace BJMT.RsspII4net.Controls
{
    /// <summary>
    /// �豸���ƽ�����
    /// </summary>
    public interface IRsspNodeNameResolver
    {
        /// <summary>
        /// ���豸IDת��Ϊ�豸����
        /// </summary>
        /// <param name="deviceId">�豸ID</param>
        /// <returns>�豸����</returns>
        string Convert(uint deviceId);
    }
}
