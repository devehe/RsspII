/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-8 9:02:49 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net.ITest
{
    /// <summary>
    /// �豸���͡�
    /// ����Чֵ0~7��ʹ���������ء���
    /// </summary>
    public enum EquipmentType : byte
    {
        /// <summary>
        /// ��Ч��
        /// </summary>
        None = 0,

        /// <summary>
        /// RBC
        /// </summary>
        RBC = 1,

        /// <summary>
        /// ����ϵͳ
        /// </summary>
        VOBC = 2,

        /// <summary>
        /// Ӧ����
        /// </summary>
        Responder = 3,

        /// <summary>
        /// Key Management Centre ��Կ�������ġ�
        /// </summary>
        KMC = 5,

        /// <summary>
        /// ������
        /// </summary>
        CBI = 6,
    }
}
