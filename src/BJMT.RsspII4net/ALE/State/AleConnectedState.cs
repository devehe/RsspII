/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-12 19:36:49 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.ALE.Frames;
using BJMT.RsspII4net.Events;
using System.Diagnostics;

namespace BJMT.RsspII4net.ALE.State
{
    /// <summary>
    /// ��ʾALE�������򱻶�����������״̬��
    /// </summary>
    class AleConnectedState : AleState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public AleConnectedState(IAleStateContext context)
            :base(context)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"

        public override void HandleTcpConnected(AleClientTunnel theConnection)
        {
            // TCP���ӳɹ�����CR��
            this.SendConnectionRequestFrame(theConnection);

            // ֪ͨ�۲���TCP�����ѽ�����
            var args = new TcpConnectedEventArgs(theConnection.ID, 
                this.Context.RsspEP.LocalID, theConnection.LocalEndPoint,
                this.Context.RsspEP.RemoteID, theConnection.RemoteEndPoint);
            this.Context.TunnelEventNotifier.NotifyTcpConnected(args);

            // ������ʱ����ʱ����
            theConnection.StartHandshakeTimer();
        }

        public override void HandleConnectionRequestFrame(AleTunnel theConnection, Frames.AleFrame theFrame)
        {
            var crData = theFrame.UserData as AleConnectionRequest;

            // ֹͣ��ʱ����ʱ����
            theConnection.StopHandshakeTimer();

            // ���CRЭ��֡��
            this.CheckCrFrame(crData, theConnection);

            // ���AU1����ȷ�ԡ�
            this.Context.AuMsgBuilder.CheckAu1Packet(crData.UserData);

            // ����CC֡��
            this.SendConnectionConfirmFrame(theConnection);

            if (!this.Context.ContainsTunnel(theConnection))
            {
                // �¼�֪ͨ�����յ�һ���µ�TCP���ӡ�
                var args = new TcpConnectedEventArgs(theConnection.ID,
                    this.Context.RsspEP.LocalID, theConnection.LocalEndPoint,
                    this.Context.RsspEP.RemoteID, theConnection.RemoteEndPoint);
                this.Context.TunnelEventNotifier.NotifyTcpConnected(args);

                // ������Ч�����Ӹ�����
                theConnection.IsHandShaken = true;
                this.Context.IncreaseValidConnection();

                // ����TCP����
                this.Context.AddConnection(theConnection);
            }
        }

        public override void HandleConnectionConfirmFrame(AleTunnel theConnection, Frames.AleFrame theFrame)
        {
            // ֹͣ��ʱ����ʱ����
            theConnection.StopHandshakeTimer();

            // ���CC֡��
            var ccData = theFrame.UserData as AleConnectionConfirm;
            this.CheckCcFrame(ccData);

            // ���CC֡�е�Ӧ�𷽱��У��ͨ����������һ����Ч�����ӡ�
            theConnection.IsHandShaken = true;
            this.Context.IncreaseValidConnection();
        }

        public override void HandleDataTransmissionFrame(AleTunnel theConnection, Frames.AleFrame theFrame)
        {
            var aleData = theFrame.UserData as AleDataTransmission;
            this.Context.Observer.OnAleUserDataArrival(aleData.UserData);
        }

        public override void SendUserData(byte[] maslPacket)
        {
            var aleData = new AleDataTransmission(maslPacket);
            var seqNo = (ushort)this.Context.SeqNoManager.GetAndUpdateSendSeq();
            var appType = this.Context.RsspEP.ApplicatinType;
            var dataFrame = new AleFrame(appType, seqNo, false, aleData);

            // send
            if (this.Context.RsspEP.ServiceType == ServiceType.A)
            {
                this.Context.Tunnels.ToList().ForEach(connection =>
                {
                    if (connection.IsActive)
                    {
                        dataFrame.IsNormal = connection.IsNormal;
                        var bytes = dataFrame.GetBytes();
                        connection.Send(bytes);
                    }
                });
            }
            else
            {
                dataFrame.IsNormal = false;
                var bytes = dataFrame.GetBytes();
                this.Context.Tunnels.ToList().ForEach(connection =>
                {
                    connection.Send(bytes);
                });
            }
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion
    }
}
