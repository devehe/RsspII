/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-28 8:51:47 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;
using BJMT.RsspII4net.ALE;
using BJMT.RsspII4net.ALE.Frames;
using BJMT.RsspII4net.Config;
using BJMT.RsspII4net.Exceptions;
using BJMT.RsspII4net.Infrastructure.Services;
using BJMT.RsspII4net.MASL.Frames;
using BJMT.RsspII4net.MASL.State;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.MASL
{
    abstract class MaslConnection : IAleConnectionObserver, 
        IMaslStateContext, 
        IHandshakeTimeoutObserver,
        IDisposable
    {
        /// <summary>
        /// AU���ֳ�ʱ��
        /// </summary>
        private const int HandshakeTimeout = 3000;

        #region "Filed"
        private bool _disposed = false;

        private bool _connected = false;

        private RsspEndPoint _rsspEndPoint = null;

        private MaslState _currentState;

        private AuMessageBuilder _auMsgBuilder;

        private IMaslConnectionObserver _observer;

        private AleConnection _aleConnection;

        private TrippleDesMacCalculator _macCalculator;

        private HandshakeTimeoutManager _handshakeTimeoutMgr;
        #endregion

        #region "Constructor"
        private MaslConnection(IMaslConnectionObserver observer, RsspEndPoint rsspEP)
        {
            _observer = observer;
            _rsspEndPoint = rsspEP;
            _currentState = this.GetInitialState();

            _macCalculator = new TrippleDesMacCalculator(rsspEP.AuthenticationKeys);
            _auMsgBuilder = new AuMessageBuilder(rsspEP, _macCalculator);

            _handshakeTimeoutMgr = new HandshakeTimeoutManager(MaslConnection.HandshakeTimeout, this);
        }

        /// <summary>
        /// ����һ����������������MASL Connection��
        /// </summary>
        protected MaslConnection(RsspEndPoint rsspEP, IEnumerable<RsspTcpLinkConfig> linkConfig,
            IMaslConnectionObserver observer, 
            IAleTunnelEventNotifier tunnelEventNotifier)
            :this(observer, rsspEP)
        {
            _aleConnection = new AleConnectionClient(rsspEP, linkConfig,
                _auMsgBuilder, this, tunnelEventNotifier);
        }

        /// <summary>
        /// ����һ�������ڱ�������MASL Connection��
        /// </summary>
        protected MaslConnection(RsspEndPoint rsspEP,
            IMaslConnectionObserver observer,
            IAleTunnelEventNotifier tunnelEventNotifier)
            : this(observer, rsspEP)
        {
            _aleConnection = new AleConnectionServer(rsspEP,
                _auMsgBuilder, this, tunnelEventNotifier);
        }

        /// <summary>
        /// �սắ����
        /// </summary>
        ~MaslConnection()
        {
            this.Dispose(false);
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// ��ȡһ��ֵ�����ڱ�ʾMASL�Ƿ������ӡ�
        /// </summary>
        public bool Connected
        {
            get { return _connected; }

            set
            {
                var preValue = _connected;
                _connected = value;

                if (!preValue && value)
                {
                    LogUtility.Info(string.Format("{0}: Masl�����ӽ�����", _rsspEndPoint.ID));
                    this.OnConnectionChanged(true);
                }
                else if (preValue && !value)
                {
                    LogUtility.Info(string.Format("{0}: Masl�������жϡ�", _rsspEndPoint.ID));
                    this.OnConnectionChanged(false);
                }
            }
        }
        #endregion


        #region "IMaslStateContext �ӿ�"
        MaslState IMaslStateContext.CurrentState { get { return _currentState; } set { _currentState = value; } }
        IMaslConnectionObserver IMaslStateContext.Observer { get { return _observer; } }
        IAuMessageBuilder IMaslStateContext.AuMessageBuilder { get { return _auMsgBuilder; } }
        IAuMessageMacCaculator IMaslStateContext.AuMessageMacCalculator { get { return _auMsgBuilder; } }
        RsspEndPoint IMaslStateContext.RsspEP { get { return _rsspEndPoint; } }
        AleConnection IMaslStateContext.AleConnection { get { return _aleConnection; } }
        IMacCalculator IMaslStateContext.MacCalculator { get { return _macCalculator; } }
        void IMaslStateContext.StartHandshakeTimer()
        {
            _handshakeTimeoutMgr.Start();
        }

        void IMaslStateContext.StopHandshakeTimer()
        {
            _handshakeTimeoutMgr.Stop();
        }
        #endregion


        #region "Virtual methods"
        protected abstract MaslState GetInitialState();
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                this.Disconnect(MaslErrorCode.NormalRelease, 0);
                this.Connected = false;

                if (disposing)
                {
                    if (_handshakeTimeoutMgr != null)
                    {
                        _handshakeTimeoutMgr.Dispose();
                    }

                    _aleConnection.Dispose();

                    if (_macCalculator != null)
                    {
                        _macCalculator.Dispose();
                    }
                }
            }
        }
        #endregion


        #region "Override methods"
        public override string ToString()
        {
            var sb = new StringBuilder(200);

            sb.AppendFormat("{0}��{1}�����Ƿ�����={2}����ǰ״̬={3}��\r\n",
                this.GetType().Name, _rsspEndPoint.ID, this.Connected,
                _currentState.GetType().Name);

            sb.AppendFormat("\r\n\r\n{0}", _aleConnection);

            return sb.ToString();
        }
        #endregion

        #region "Private methods"

        /// <summary>
        /// ����MASL���ӱ仯�¼���
        /// </summary>
        /// <param name="connected">true��ʾMASL�ɶϵ�����false��ʾMASL�������ϡ�</param>
        private void OnConnectionChanged(bool connected)
        {
            try
            {
                if (connected)
                {
                    _currentState = new MaslConnectedState(this);

                    _observer.OnMaslConnected();
                }
                else
                {
                    _currentState = this.GetInitialState();

                    _observer.OnMaslDisconnected();

                    _macCalculator.InitRandomNumber();
                }
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());
            }
        }

        #endregion

        #region "Public methods"

        public void Open()
        {
            _aleConnection.Open();
        }

        public void SendUserData(byte[] saiPacket)
        {
            _currentState.SendUserData(saiPacket);
        }

        public void HandleAleConnectionRequestFrame(AleServerTunnel connection, AleFrame requestFrame)
        {
            ((IAleTunnelObserver)_aleConnection).OnAleFrameArrival(connection, requestFrame);
        }

        public void AddAleServerTunnel(AleServerTunnel tunnel)
        {
            _aleConnection.AddAleServerTunnel(tunnel);
        }

        public void Disconnect(MaslErrorCode majorReason, byte minorReason)
        {
            _currentState.Disconnect(majorReason, minorReason);

            _currentState = this.GetInitialState();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region IAleConnectionObserver�ӿ�

        void IAleConnectionObserver.OnAleConnected()
        {
            // Do nothing.
        }

        void IAleConnectionObserver.OnAleDisconnected()
        {
            _currentState = this.GetInitialState();

            this.Connected = false;
        }

        void IAleConnectionObserver.OnAleUserDataArrival(byte[] aleUserData)
        {
            try
            {
                var maslFrame = MaslFrame.Parse(aleUserData, 0, aleUserData.Length - 1);

                if (maslFrame.FrameType == MaslFrameType.AU1)
                {
                    LogUtility.Info(string.Format("{0}: Masl�㵱ǰ״̬{1}������AU1��",
                        _rsspEndPoint.ID, _currentState.GetType().Name));

                    _currentState.HandleAu1Frame(maslFrame as MaslAu1Frame);
                }
                else if(maslFrame.FrameType == MaslFrameType.AU2)
                {
                    LogUtility.Info(string.Format("{0}: Masl�㵱ǰ״̬{1}������AU2��",
                        _rsspEndPoint.ID, _currentState.GetType().Name));

                    _currentState.HandleAu2Frame(maslFrame as MaslAu2Frame);
                }
                else if (maslFrame.FrameType == MaslFrameType.AU3)
                {
                    LogUtility.Info(string.Format("{0}: Masl�㵱ǰ״̬{1}������AU3��",
                        _rsspEndPoint.ID, _currentState.GetType().Name));

                    _currentState.HandleAu3Frame(maslFrame as MaslAu3Frame);
                }
                else if (maslFrame.FrameType == MaslFrameType.AR)
                {
                    LogUtility.Info(string.Format("{0}: Masl�㵱ǰ״̬{1}������AR��",
                        _rsspEndPoint.ID, _currentState.GetType().Name));

                    _currentState.HandleArFrame(maslFrame as MaslArFrame);
                }
                else if (maslFrame.FrameType == MaslFrameType.DT)
                {
                    if (!(_currentState is MaslConnectedState))
                    {
                        LogUtility.Error(string.Format("{0}: Masl�㵱ǰ״̬{1}������DT��",
                            _rsspEndPoint.ID, _currentState.GetType().Name));
                    }

                    _currentState.HandleDtFrame(maslFrame as MaslDtFrame);
                }
                else if (maslFrame.FrameType == MaslFrameType.DI)
                {
                    LogUtility.Info(string.Format("{0}: Masl�㵱ǰ״̬{1}������DI��",
                        _rsspEndPoint.ID, _currentState.GetType().Name));
                    
                    _currentState = this.GetInitialState();
                    this.Connected = false;

                    //_currentState.HandleDiFrame(maslFrame as MaslDiFrame);
                }
            }
            catch (MaslException ex)
            {
                this.Disconnect(ex.MajorReason, ex.MinorReason);
                LogUtility.Error(ex.ToString());
            }
            catch (Exception ex)
            {
                this.Disconnect(MaslErrorCode.NotDefined, 0);
                LogUtility.Error(ex.ToString());
            }
        }
        #endregion

        #region "IHandshakeTimeoutObserver�ӿ�ʵ��"
        void IHandshakeTimeoutObserver.OnHandshakeTimeout()
        {
            try
            {
                var msg = string.Format("{0}: Masl���ּ�ʱ����ʱ����ǰ״̬={1}��",
                     _rsspEndPoint.ID, _currentState.GetType().Name);

                if (_currentState is MaslWaitingforArState)
                {
                    throw new WaitingArTimeoutException(msg);
                }
                else
                {
                    throw new MaslException(MaslErrorCode.ConnectionTimeout, MaslException.UndefineMinorReason, msg);
                }
            }
            catch (MaslException ex)
            {
                LogUtility.Error(ex.ToString());
                this.Disconnect(ex.MajorReason, ex.MinorReason);
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());
                this.Disconnect(MaslErrorCode.NotDefined, 0);
            }
        }
        #endregion
    }
}
