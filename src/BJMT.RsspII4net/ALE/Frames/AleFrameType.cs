/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-18 13:56:56 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net.ALE.Frames
{
    /// <summary>
    /// ALEʹ�õ����а������Ͷ��塣
    /// </summary>
    enum AleFrameType : byte
    {
        /// <summary>
        /// ��������
        /// </summary>
        ConnectionRequest = 0x01,
        /// <summary>
        /// ����ȷ��
        /// </summary>
        ConnectionConfirm = 0x02,

        /// <summary>
        /// ���ݴ���
        /// </summary>
        DataTransmission = 0x03,

        /// <summary>
        /// �����ӣ��Ͽ�ָʾ
        /// </summary>
        Disconnect = 0x04,

        /// <summary>
        /// ������ͨ���л���������·
        /// </summary>
        SwitchN2R = 251,
        /// <summary>
        /// ������ͨ���л���������·
        /// </summary>
        SwitchR2N = 253,

        /// <summary>
        /// �ǻ��·��������Ϣ��
        /// </summary>
        KANA = 254,
        /// <summary>
        /// ���·��������Ϣ
        /// </summary>
        KAA = 255,
    }
}
