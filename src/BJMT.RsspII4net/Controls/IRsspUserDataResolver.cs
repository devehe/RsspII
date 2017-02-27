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
    /// �û����ݽ������ӿڡ�
    /// </summary>
    public interface IRsspUserDataResolver
    {
        /// <summary>
        /// ��ȡ�û����ݵ�������Ϣ��
        /// </summary>
        /// <param name="userData">�û����ݡ�</param>
        /// <param name="isIncomingData">�Ƿ�Ϊ���������ݣ�true��ʾ���������ݣ�false��ʾ��������ݡ�</param>
        /// <returns>Э��֡����������Ϣ</returns>
        string GetDescription(IEnumerable<byte> userData, bool isIncomingData);

        /// <summary>
        /// ��ȡ�û����ݵ��Զ����ǩ
        /// </summary>
        /// <param name="userData">�û����ݡ�</param>
        /// <param name="isIncomingData">�Ƿ�Ϊ���������ݣ�true��ʾ���������ݣ�false��ʾ��������ݡ�</param>
        /// <returns>�Զ����ǩ</returns>
        string GetLabel(IEnumerable<byte> userData, bool isIncomingData);
    }
}
