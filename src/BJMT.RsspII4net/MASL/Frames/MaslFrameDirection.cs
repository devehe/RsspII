/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-8 9:53:20 
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
    /// <summary>
    /// Masl֡�����塣
    /// </summary>
    enum MaslFrameDirection
    {
        /// <summary>
        /// ���� -> Ӧ��
        /// </summary>
        Client2Server = 0,

        /// <summary>
        /// Ӧ�� -> ����
        /// </summary>
        Server2Client = 1,
    }
}
