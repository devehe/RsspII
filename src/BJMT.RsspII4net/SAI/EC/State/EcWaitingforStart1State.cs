/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-15 16:43:03 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.SAI.EC.Frames;

namespace BJMT.RsspII4net.SAI.EC.State
{
    /// <summary>
    /// ��ʾ���������ڵȴ�EcStart������һ��EcStart��
    /// </summary>
    class EcWaitingforStart1State : EcState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public EcWaitingforStart1State(ISaiStateContext context, EcDefenseStrategy strategy)
            : base(context, strategy)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"

        protected override void HandleEcStartFrame(SaiEcFrameStart frame)
        {
            this.Context.StopHandshakeTimer();
            LogUtility.Info(string.Format("{0}: �������յ�EcStart1����ӦEC Start2����״̬��EcConnectedState",
                this.Context.RsspEP.ID));

            // ����Remote EcCounter��
            this.DefenseStrategy.StartRemoteCounter(this.Context.RsspEP.RemoteID, frame.Interval, frame.InitialValue);

            // ����������EcStart�ظ���
            this.SendEcStartFrame();

            // ������һ״̬ΪEcConnectedState
            this.Context.CurrentState = new EcConnectedState(this);

            this.Context.Connected = true;
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
