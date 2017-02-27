/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-16 10:33:28 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using BJMT.RsspII4net.ALE;
using BJMT.RsspII4net.ALE.Frames;
using BJMT.RsspII4net.Infrastructure.Services;

namespace BJMT.RsspII4net.SAI
{
    class SaiConnectionServer : SaiConnection
    {
        #region "Filed"
        #endregion

        #region "Constructor"

        /// <summary>
        /// ����һ�������ڱ�������SAI Connection��
        /// </summary>
        public SaiConnectionServer(RsspEndPoint rsspEP,
            ISaiConnectionObserver observer,
            IAleTunnelEventNotifier tunnelEventNotifier)
            : base(rsspEP, observer, tunnelEventNotifier)
        {
        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        protected override SaiState GetInitialState(DefenseStrategy strategy)
        {
            return new SaiInvalidState(this);
        }

        protected override DefenseStrategy GetDefenseStrategy()
        {
            return null;
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"

        public void HandleAleConnectionRequestFrame(AleServerTunnel connection, AleFrame requestFrame)
        {
            _maslConnection.HandleAleConnectionRequestFrame(connection, requestFrame);
        }


        public void AddAleServerTunnel(AleServerTunnel tunnel)
        {
            _maslConnection.AddAleServerTunnel(tunnel);
        }
        #endregion

    }
}
