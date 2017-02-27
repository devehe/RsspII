/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-29 13:23:15 
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
    /// һ���¼������࣬���������û����ݽ����¼���
    /// </summary>
    public class UserDataIncomingEventArgs : EventArgs
    {
        /// <summary>
        /// ���캯����
        /// </summary>
        /// <param name="pkg">�յ���Package��</param>
        public UserDataIncomingEventArgs(IncomingPackage pkg)
        {
            this.Package = pkg;
        }

        /// <summary>
        /// ��ȡ�յ��İ���
        /// </summary>
        public IncomingPackage Package { get; private set; }
    }
}
