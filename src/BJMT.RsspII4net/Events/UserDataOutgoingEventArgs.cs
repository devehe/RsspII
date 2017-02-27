/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-30 8:58:25 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net.Events
{
    /// <summary>
    /// һ���¼������࣬���������û����ݷ����¼���
    /// </summary>
    public class UserDataOutgoingEventArgs : EventArgs
    {
        /// <summary>
        /// ���캯����
        /// </summary>
        /// <param name="pkg">�յ���Package��</param>
        public UserDataOutgoingEventArgs(OutgoingPackage pkg)
        {
            this.Package = pkg;
        }

        /// <summary>
        /// ��ȡ��Ҫ���͵İ���
        /// </summary>
        public OutgoingPackage Package { get; private set; }

    }
}
