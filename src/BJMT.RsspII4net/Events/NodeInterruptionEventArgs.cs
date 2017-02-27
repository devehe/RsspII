/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-29 13:20:41 
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
    /// һ���¼������࣬���������ڵ��ж��¼������ݡ�
    /// </summary>
    public class NodeInterruptionEventArgs : EventArgs
    {
        /// <summary>
        /// ���캯����
        /// </summary>
        /// <param name="localID">���ؽڵ�ID</param>
        /// <param name="remoteID">Զ�̽ڵ�ID</param>
        public NodeInterruptionEventArgs(uint localID, uint remoteID)
        {
            this.LocalID = localID;
            this.RemoteID = remoteID;
        }

        /// <summary>
        /// ��ȡ���ؽڵ�ID��
        /// </summary>
        public uint LocalID { get; private set; }

        /// <summary>
        /// ��ȡԶ�̽ڵ�ID��
        /// </summary>
        public uint RemoteID { get; private set; }

    }
}
