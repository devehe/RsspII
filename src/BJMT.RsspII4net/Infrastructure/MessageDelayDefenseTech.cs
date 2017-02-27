/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-6 14:05:06 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net
{
    /// <summary>
    /// RSSP��Ϣ�ӳٷ�������
    /// </summary>
    public enum MessageDelayDefenseTech
    {
        /// <summary>
        /// ��Чֵ��
        /// </summary>
        None,

        /// <summary>
        /// ExcutionCycle ����ִ�����ڵķ���������
        /// </summary>
        EC,

        /// <summary>
        /// TripleTimeStamp ����ʱ�������������
        /// </summary>
        TTS,
    }
}
