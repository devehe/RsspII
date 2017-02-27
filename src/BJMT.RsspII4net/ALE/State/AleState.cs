/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-12 19:38:33 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Diagnostics;
using System.Linq;
using BJMT.RsspII4net.ALE.Frames;

namespace BJMT.RsspII4net.ALE.State
{
    abstract class AleState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        protected AleState(IAleStateContext context)
        {
            LogUtility.Info(string.Format("{0}��ALE����״̬= {1}", context.RsspEP.ID, this.GetType().Name));

            this.Context = context;
        }
        #endregion

        #region "Properties"
        protected IAleStateContext Context { get; private set; }
        #endregion

        #region "Virtual methods"
        public virtual void HandleTcpConnected(AleClientTunnel theConnection)
        {
            LogUtility.Error(string.Format("{0}: {1}.{2} not implement!",
                this.Context.RsspEP.ID, this.GetType().Name,
                new StackFrame(0).GetMethod().Name.Split('.').Last()));
        }

        public virtual void HandleConnectionRequestFrame(AleTunnel theConnection, AleFrame theFrame)
        {
            LogUtility.Error(string.Format("{0}: {1}.{2} not implement!",
                this.Context.RsspEP.ID, this.GetType().Name,
                new StackFrame(0).GetMethod().Name.Split('.').Last()));
        }

        public virtual void HandleConnectionConfirmFrame(AleTunnel theConnection, AleFrame theFrame)
        {
            LogUtility.Error(string.Format("{0}: {1}.{2} not implement!",
                this.Context.RsspEP.ID, this.GetType().Name,
                new StackFrame(0).GetMethod().Name.Split('.').Last()));
        }

        public virtual void HandleDiFrame(AleTunnel theConnection, AleFrame theFrame)
        {
            var diData = theFrame.UserData as AleDisconnect;
            this.Context.Observer.OnAleUserDataArrival(diData.UserData);

            theConnection.Disconnect();
        }


        public virtual void HandleDataTransmissionFrame(AleTunnel theConnection, AleFrame theFrame)
        {
            LogUtility.Error(string.Format("{0}: {1}.{2} not implement!",
                this.Context.RsspEP.ID, this.GetType().Name,
                new StackFrame(0).GetMethod().Name.Split('.').Last()));
        }


        public virtual void SendUserData(byte[] maslPacket)
        {
            LogUtility.Error(string.Format("{0}: {1}.{2} not implement!",
                this.Context.RsspEP.ID, this.GetType().Name,
                new StackFrame(0).GetMethod().Name.Split('.').Last()));
        }


        public virtual void Disconnect(byte[] diSaPDU)
        {
            var seqNo = (ushort)this.Context.SeqNoManager.GetAndUpdateSendSeq();
            var appType = this.Context.RsspEP.ApplicatinType;
            var aleData = new AleDisconnect(diSaPDU);

            var dataFrame = new AleFrame(appType, seqNo, false, aleData);
            var bytes = dataFrame.GetBytes();

            var tcpLinks = this.Context.Tunnels.ToList();
            tcpLinks.ForEach(connection =>
            {
                connection.Send(bytes);
                connection.Disconnect();
            });
        }
        #endregion

        #region "Override methods"
        #endregion

        #region "Private/protected methods"

        protected void CheckCrFrame(AleConnectionRequest crData, AleTunnel theTunnel)
        {
            if (crData.ServiceType == ServiceType.A)
            {
                throw new Exception(string.Format("�յ�CR֡������A����񣬲�֧�ִ����ͣ��Ͽ����ӡ�"));
            }

            if (crData.ResponderID != this.Context.RsspEP.LocalID)
            {
                throw new Exception(string.Format("CR֡�еı�������ţ�{0}���뱾�ر�ţ�{1}����һ�£��Ͽ������ӡ�",
                    crData.ResponderID, this.Context.RsspEP.LocalID));
            }

            if (crData.InitiatorID == this.Context.RsspEP.LocalID)
            {
                throw new Exception(string.Format("CR֡�е���������ţ�{0}���뱾�ر�ţ�{1}����ͬ���Ͽ������ӡ�",
                    crData.InitiatorID, this.Context.RsspEP.LocalID));
            }

            if (!this.Context.RsspEP.IsClientAcceptable(crData.InitiatorID, theTunnel.RemoteEndPoint))
            {
                throw new Exception(string.Format("CR֡�е���������ţ�{0}�����ս�㣨{1}�����ڿɽ��ܵķ�Χ�ڣ��Ͽ������ӡ�",
                    crData.InitiatorID, theTunnel.RemoteEndPoint));
            }
        }

        protected void CheckCcFrame(AleConnectionConfirm ccData)
        {
            if (ccData.ServerID != this.Context.RsspEP.RemoteID)
            {
                throw new Exception(string.Format("CC֡�е�Ӧ�𷽱���������Ĳ�һ�£�����ֵ={0}��ʵ��ֵ={1}",
                    this.Context.RsspEP.RemoteID, ccData.ServerID));
            }
        }

        protected void SendConnectionRequestFrame(AleClientTunnel theConnection)
        {
            var au1Packet = this.Context.AuMsgBuilder.BuildAu1Packet();

            var crData = new AleConnectionRequest(this.Context.RsspEP.LocalID, this.Context.RsspEP.RemoteID,
                this.Context.RsspEP.ServiceType, au1Packet);

            // ����AleFrame��
            ushort seqNo = 0;
            var appType = this.Context.RsspEP.ApplicatinType;
            var crFrame = new AleFrame(appType, seqNo, theConnection.IsNormal, crData);

            // ���͡�
            var bytes = crFrame.GetBytes();
            theConnection.Send(bytes);

            LogUtility.Info(string.Format("{0}: Send CR frame via tcp connection(LEP = {1}, REP={2}).",
                this.Context.RsspEP.ID, theConnection.LocalEndPoint, theConnection.RemoteEndPoint));
        }

        protected void SendConnectionConfirmFrame(AleTunnel theConnection)
        {
            var au2Packet = this.Context.AuMsgBuilder.BuildAu2Packet();

            var ccData = new AleConnectionConfirm(this.Context.RsspEP.LocalID, au2Packet);

            // ����AleFrame��
            ushort seqNo = 0;
            var appType = this.Context.RsspEP.ApplicatinType;
            var ccFrame = new AleFrame(appType, seqNo, theConnection.IsNormal, ccData);

            // ���͡�
            var bytes = ccFrame.GetBytes();
            theConnection.Send(bytes);

            LogUtility.Info(string.Format("{0}: Send CC frame via tcp connection(LEP = {1}, REP={2}).",
                this.Context.RsspEP.ID, theConnection.LocalEndPoint, theConnection.RemoteEndPoint));
        }
        #endregion

        #region "Public methods"
        #endregion

    }
}
