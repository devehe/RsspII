/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-16 8:18:55 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using BJMT.RsspII4net.ALE.State;
using BJMT.RsspII4net.Infrastructure.Services;

namespace BJMT.RsspII4net.ALE
{
    class AleConnectionServer : AleConnection, IAleServerTunnelObserver
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        
        /// <summary>
        /// ����һ�������ڱ�������ALE Connection��
        /// </summary>
        public AleConnectionServer(RsspEndPoint rsspEP,
            IAuMessageBuilder auMsgProvider,
            IAleConnectionObserver observer,
            IAleTunnelEventNotifier tunnelEventNotifier)
            : base(rsspEP, auMsgProvider, observer, tunnelEventNotifier)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"

        protected override AleState GetInitialState()
        {
            return new AleWaitingForCrState(this);            
        }

        protected override void HandleTunnelDisconnected(AleTunnel theConnection, string reason)
        {
            try
            {
                // �������ˣ��Ƴ����رմ����ӡ�
                this.RemoveCloseConnection(theConnection);
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());
            }
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

        #region "IAleServerTunnel �ӿ�ʵ��"
        #endregion
    }
}
