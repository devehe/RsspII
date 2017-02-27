/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-16 10:32:07 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using BJMT.RsspII4net.Config;
using BJMT.RsspII4net.Infrastructure.Services;
using BJMT.RsspII4net.SAI.EC;
using BJMT.RsspII4net.SAI.EC.State;
using BJMT.RsspII4net.SAI.TTS;
using BJMT.RsspII4net.SAI.TTS.State;

namespace BJMT.RsspII4net.SAI
{
    class SaiConnectionClient : SaiConnection
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        /// <summary>
        /// ����һ����������������SAI Connection��
        /// </summary>
        public SaiConnectionClient(RsspEndPoint rsspEP, IEnumerable<RsspTcpLinkConfig> linkConfig,
            ISaiConnectionObserver observer,
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
        protected override SaiState GetInitialState(DefenseStrategy strategy)
        {
            if (this.RsspEP.DefenseTech == MessageDelayDefenseTech.EC)
            {
                var ecStrategy = strategy as EcDefenseStrategy;
                if (ecStrategy == null)
                {
                    throw new InvalidCastException("ָ���Ĳ����޷�ת��ΪEcDefenseStrategy��");
                }

                return new EcDisconnectedState(this, ecStrategy);
            }
            else if (this.RsspEP.DefenseTech == MessageDelayDefenseTech.TTS)
            {
                var ttsStrategy = strategy as TtsDefenseStrategy;
                if (ttsStrategy == null)
                {
                    throw new InvalidCastException("ָ���Ĳ����޷�ת��ΪTtsDefenseStrategy��");
                }
                
                return new TtsDisconnectedState(this, ttsStrategy);
            }
            else
            {
                throw new InvalidOperationException("����������ָ��һ����Ч����Ϣ�ӳٷ���������");
            }
        }

        protected override DefenseStrategy GetDefenseStrategy()
        {
            if (this.RsspEP.DefenseTech == MessageDelayDefenseTech.EC)
            {
                return new EcDefenseStrategy(this.RsspEP.LocalID, this.RsspEP.EcInterval);
            }
            else if (this.RsspEP.DefenseTech == MessageDelayDefenseTech.TTS)
            {
                return new TtsDefenseStrategy(this, true);
            }
            else
            {
                throw new InvalidOperationException("ָ������Ϣ�ӳٷ���������Ч��");
            }
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
