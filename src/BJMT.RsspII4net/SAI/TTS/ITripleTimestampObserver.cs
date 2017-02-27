/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ���������15����ATS��Ŀ
//
// �� �� �ˣ�zhangheng
// �������ڣ�04/24/2014 16:58:20 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾ 2009-2015 ��������Ȩ��
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;

namespace BJMT.RsspII4net.SAI.TTS
{
    /// <summary>
    /// TTS�۲���
    /// </summary>
    internal interface ITripleTimestampObserver
    {
        /// <summary>
        /// ������ʱ����ķ��������ʱ���á�
        /// </summary>
        /// <param name="latestTimestamp">���µ�ʱ���</param>
        /// <param name="lastTimestamp">��һ�ε�ʱ���</param>
        void OnTimestampZeroPassed(UInt32 latestTimestamp, UInt32 lastTimestamp);
    }
}
