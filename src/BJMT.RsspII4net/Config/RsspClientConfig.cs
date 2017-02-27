/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-25 10:20:45 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net.Config
{
    /// <summary>
    /// RSSP�ͻ������á�
    /// </summary>
    public class RsspClientConfig : RsspConfig
    {
        /// <summary>
        /// �����������·���á�
        /// Key = ��������š�
        /// </summary>
        public Dictionary<uint, List<RsspTcpLinkConfig>> LinkInfo { get; set; }
        
        /// <summary>
        /// ����һ��RSSP�ͻ������á�
        /// </summary>
        /// <param name="localID">���ر�š�</param>
        /// <param name="localEquipType">�����豸���͡�</param>
        /// <param name="appType">Ӧ������</param>
        /// <param name="defenseTech"></param>
        /// <param name="links">��·������Ϣ��</param>
        public RsspClientConfig(uint localID, byte localEquipType, 
            byte appType,
            MessageDelayDefenseTech defenseTech,
            Dictionary<uint, List<RsspTcpLinkConfig>> links)
        {
            if (links == null || links.Count == 0)
            {
                throw new ArgumentException("��������ָ��һ��TCP��·��");
            }

            this.IsInitiator = true;
            this.LocalID = localID;
            this.LocalEquipType = localEquipType;
            this.ApplicationType = appType;
            this.DefenseTech = defenseTech;

            this.LinkInfo = new Dictionary<uint, List<RsspTcpLinkConfig>>(links);
        }
    }
}
