/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-25 10:20:58 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace BJMT.RsspII4net.Config
{
    /// <summary>
    /// RSSP-II����������
    /// </summary>
    public class RsspServerConfig : RsspConfig
    {
        /// <summary>
        /// ���������ս�㡣
        /// </summary>
        public IEnumerable<IPEndPoint> ListenEndPoints { get; private set; }
        
        /// <summary>
        /// ��ȡ�ɽ��ܵĿͻ����б� 
        /// </summary>
        public IEnumerable<KeyValuePair<uint, List<IPEndPoint>>> AcceptableClients { get; private set; }

        /// <summary>
        /// ����һ�����������ö���
        /// </summary>
        /// <param name="localID">���ؽڵ�ID��</param>
        /// <param name="localEquipType">�����豸���͡�</param>
        /// <param name="appType">Ӧ�����͡�</param>
        /// <param name="ep">�����ս���б�</param>
        /// <param name="acceptableClients">�ɽ��ܵĿͻ����б�Ϊ������ʱ��ʾ�������пͻ��ˣ���Ϊ��ʱ��ʾ���Խ��ܵĿͻ����б�</param>
        public RsspServerConfig(uint localID, byte localEquipType,
            byte appType, IEnumerable<IPEndPoint> ep, 
            IEnumerable<KeyValuePair<uint, List<IPEndPoint>>> acceptableClients)
        {
            this.IsInitiator = false;
            this.LocalID = localID;
            this.LocalEquipType = localEquipType;
            this.ApplicationType = appType;

            this.ListenEndPoints = new List<IPEndPoint>(ep);

            if (acceptableClients != null)
            {
                this.AcceptableClients = new List<KeyValuePair<uint, List<IPEndPoint>>>(acceptableClients);
            }
        }
    }
}
