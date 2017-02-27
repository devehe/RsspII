/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-18 15:24:13 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net.SAI
{
    /// <summary>
    /// SAI���֡���Ͷ��塣
    /// </summary>
    enum SaiFrameType : byte
    {
        /// <summary>
        /// ʱ��ƫ�ƿ�ʼ����
        /// </summary>
        TTS_OffsetStart = 1,
        /// <summary>
        /// ʱ��ƫ��Answer1����
        /// </summary>
        TTS_OffsetAnswer1 = 2,
        /// <summary>
        /// ʱ��ƫ��Answer2����
        /// </summary>
        TTS_OffsetAnswer2 = 3,
        /// <summary>
        /// ʱ��ƫ�ƹ��㱨��
        /// </summary>
        TTS_OffsetEstimate = 4,
        /// <summary>
        /// ʱ��ƫ�ƽ�������
        /// </summary>
        TTS_OffsetEnd = 5,
        /// <summary>
        /// ʹ��TTS������Ӧ����Ϣ��
        /// </summary>
        TTS_AppData = 6,


        /// <summary>
        /// EC��ʼ��Ϣ
        /// </summary>
        EC_Start = 0x81,
        /// <summary>
        /// ʹ��EC������Ӧ����Ϣ��
        /// </summary>
        EC_AppData = 0x86,
        /// <summary>
        /// ʹ��EC����������Ӧ���Ӧ����Ϣ��
        /// </summary>
        EC_AppDataAskForAck = 0x87,
        /// <summary>
        /// ʹ��EC����������Ӧ��ȷ�ϵ�Ӧ����Ϣ��
        /// </summary>
        EC_AppDataAcknowlegment = 0x88,
    }
}
