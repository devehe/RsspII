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
    /// ��ʾ���������ڵȴ�EcStart�����ڶ���EcStart��
    /// </summary>
    class EcWaitingforStart2State : EcState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public EcWaitingforStart2State(EcState preState)
            : base(preState)
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
            // �������ر�EC Tsync.
            this.Context.StopHandshakeTimer();
            LogUtility.Info(string.Format("{0}: �������յ�EcStart2���ر�Tsync����״̬��EcConnectedState",
                this.Context.RsspEP.ID));

            // ����Remote EcCounter��
            this.DefenseStrategy.StartRemoteCounter(this.Context.RsspEP.RemoteID, frame.Interval, frame.InitialValue);

            // ������һ״̬ΪEcConnectedState
            this.Context.CurrentState = new EcConnectedState(this);

            // 
            this.Context.Connected = true;
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
