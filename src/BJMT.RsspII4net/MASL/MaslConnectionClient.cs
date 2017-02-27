/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-16 8:59:05 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.Config;
using BJMT.RsspII4net.Infrastructure.Services;
using BJMT.RsspII4net.MASL.State;

namespace BJMT.RsspII4net.MASL
{
    class MaslConnectionClient : MaslConnection
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        /// <summary>
        /// ����һ����������������MASL Connection��
        /// </summary>
        public MaslConnectionClient(RsspEndPoint rsspEP, IEnumerable<RsspTcpLinkConfig> linkConfig,
            IMaslConnectionObserver observer, 
            IAleTunnelEventNotifier tunnelEventNotifier)
            : base(rsspEP, linkConfig, observer, tunnelEventNotifier)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        protected override MaslState GetInitialState()
        {
            return new MaslWaitingforAu2State(this);
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
