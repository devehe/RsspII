/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-8 9:09:59 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net.MASL.Frames
{
    enum MaslFrameType : byte
    {
        /// <summary>
        /// none.
        /// </summary>
        None = 0,
        /// <summary>
        /// First Authentication message.
        /// </summary>
        AU1 = 1,
        /// <summary>
        /// Second Authenticatioin message.
        /// </summary>
        AU2 = 2,
        /// <summary>
        /// Third Authentication message.
        /// </summary>
        AU3 = 3,
        /// <summary>
        /// Authtication Response message.
        /// </summary>
        AR = 9,

        /// <summary>
        /// Data Transimission message.
        /// </summary>
        DT = 5,

        /// <summary>
        /// Disconnect message.
        /// </summary>
        DI = 8,
    }
}
