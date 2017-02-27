/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-28 8:42:04 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.Config;
using BJMT.RsspII4net.Events;
using BJMT.RsspII4net.Exceptions;
using BJMT.RsspII4net.Infrastructure.Services;
using BJMT.RsspII4net.SAI;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net
{
    /// <summary>
    /// ����RSSP-IIͨѶЭ��Ľڵ㶨�塣
    /// </summary>
    abstract class RsspNode : 
        ISaiConnectionObserver, 
        IAleTunnelEventNotifier        
    {
        private const int CacheCountThreshold = 100;
        private const int BlockingTimeThreshold = 3;

        #region "Filed"
        private bool _disposed = false;

        private RsspConfig _rsspConfig;
        
        /// <summary>
        /// һ���¼�����أ����ڴ�š���Ҫ���͵��û����ݰ����󡱡�
        /// </summary>
        private ProductCache<OutgoingPackage> _productCacheSending = new ProductCache<OutgoingPackage>();

        /// <summary>
        /// һ���¼�����أ����ڴ�š�������û����ݰ����󡱡�
        /// </summary>
        private ProductCache<IncomingPackage> _productCacheReceive = new ProductCache<IncomingPackage>();


        /// <summary>
        /// ��һ�η��Ͷ��еĸ�����
        /// </summary>
        private int _lastSendingCacheCount = 0;
        /// <summary>
        /// ��һ�ν��ն��еĸ�����
        /// </summary>
        private int _lastReceiveCacheCount = 0;

        /// <summary>
        /// ��һ�η��Ͷ��е�����ʱ�䡣����λ���룩
        /// </summary>
        private int _lastSendQueueBlockingTime = 0;
        /// <summary>
        /// ��һ�ν��ն��е�����ʱ�䡣
        /// </summary>
        private int _lastReceiveQueueBlockingTime = 0;
        #endregion

        #region "Constructor"

        /// <summary>
        /// ����һ��RsspNode��
        /// </summary>
        protected RsspNode(RsspConfig config)
        {
            _rsspConfig = config;

            // ���ñ��ڵ�����ơ�
            var prefixName = config.IsInitiator ? "RSSP�ͻ���" : "RSSP������";
            this.Name = string.Format("{0}{1}", prefixName, _rsspConfig.LocalID);

        }

        ~RsspNode()
        {
            this.Dispose(false);
        }
        #endregion

        #region "Properties"
        public string Name { get; private set; }
        #endregion

        #region "Virtual methods"
        protected abstract void OnOpen(); 

        protected abstract SaiConnection GetSaiConnection(string key);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    // �رջ���ء�
                    _productCacheReceive.Close();
                    _productCacheSending.Close();
                }
            }
        }
        #endregion

        #region "Override methods"
        #endregion

        #region "Private methods"

        private void CreateProductCache()
        {
            // �򿪡�RDT�¼�����ء���
            _productCacheReceive.ThreadName = string.Format("{0}���ն����߳�", this.Name);
            _productCacheReceive.ProductCreated += OnIncomingUserDataProductCreated;
            _productCacheReceive.Open();

            // �򿪷��ͻ���ء�
            _productCacheSending.ThreadName = string.Format("{0}���Ͷ����߳�", this.Name);
            _productCacheSending.ProductCreated += OnOutgoingUserDataProductCreated;
            _productCacheSending.Open();
        }

        private void OnOutgoingUserDataProductCreated(object sender, ProductCreatedEventArgs<OutgoingPackage> e)
        {
            try
            {
                e.Products.ToList().ForEach(package =>
                {
                    try
                    {
                        this.SendOutgoingPackage(package);

                        this.NotityfyOutgoingUserDataEvent(package);
                    }
                    catch (System.Exception ex)
                    {
                        LogUtility.Error(string.Format("{0}", ex));
                    }
                });
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(string.Format("{0}", ex));
            }
        }

        private void NotityfyOutgoingUserDataEvent(OutgoingPackage package)
        {
            if (this.UserDataOutgoing != null)
            {
                var args = new UserDataOutgoingEventArgs(package);

                this.UserDataOutgoing.GetInvocationList().ToList().ForEach(handler =>
                {
                    try
                    {                        
                        handler.DynamicInvoke(null, args);
                    }
                    catch (System.Exception)
                    {
                    }
                });
            }
        }

        private void SendOutgoingPackage(OutgoingPackage package)
        {
            package.DestID.ToList().ForEach(remoteID =>
            {
                try
                {
                    var saiID = this.BuildSaiConnectionID(_rsspConfig.LocalID, remoteID);

                    var theSaiConnection = this.GetSaiConnection(saiID);

                    if (theSaiConnection != null && theSaiConnection.Connected)
                    {
                        theSaiConnection.SendUserData(package);
                    }
                }
                catch (System.Exception ex)
                {
                    LogUtility.Error(ex.ToString());
                }
            });
        }

        private void OnIncomingUserDataProductCreated(object sender, ProductCreatedEventArgs<IncomingPackage> e)
        {
            try
            {
                e.Products.ToList().ForEach(pkg =>
                {
                    NotifyIncomingUserDataEvent(pkg);
                });
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(string.Format("{0}", ex));
            }
        }

        private void NotifyIncomingUserDataEvent(IncomingPackage pkg)
        {
            if (this.UserDataIncoming != null)
            {
                this.UserDataIncoming.GetInvocationList().ToList().ForEach(handler =>
                {
                    try
                    {
                        var args = new UserDataIncomingEventArgs(pkg);
                        args.Package.QueuingDelay = (UInt32)((DateTime.Now - args.Package.CreationTime).TotalMilliseconds / 10);

                        handler.DynamicInvoke(null, args);
                    }
                    catch (System.Exception)
                    {
                    }
                });
            }
        }

        private void CheckOutgoingCacheThreshold()
        {
            try
            {
                var allPkg = _productCacheSending.GetData();
                int currentCount = allPkg.Count();

                #region "���и������"
                try
                {
                    if (currentCount >= CacheCountThreshold && currentCount % CacheCountThreshold == 0)
                    {
                        LogUtility.Warn(string.Format("δ���͵��û����ݸ����Ѵﵽ��ֵ({0})��{1}����",
                            CacheCountThreshold, currentCount / CacheCountThreshold));
                    }

                    // �����Ͷ��и����ı䡱�¼�֪ͨ��
                    if (currentCount != _lastSendingCacheCount
                        && this.OutgoingCacheCountChanged != null)
                    {
                        _lastSendingCacheCount = currentCount;
                        this.OutgoingCacheCountChanged(null, new OutgoingCacheCountChangedEventArgs(this.Name, currentCount));
                    }
                }
                catch (System.Exception /*ex*/)
                {
                }
                #endregion

                #region "ʱ����"
                try
                {
                    if (currentCount > 0)
                    {
                        var farthestTime = allPkg.Min(p => p.CreationTime);
                        var blockingTime = Convert.ToInt32((DateTime.Now - farthestTime).TotalSeconds);

                        if (this.OutgoingCacheDelayed != null
                            && blockingTime > BlockingTimeThreshold
                            && (blockingTime != _lastSendQueueBlockingTime))
                        {
                            _lastSendQueueBlockingTime = blockingTime;

                            var args = new OutgoingCacheDelayedEventArgs(this.Name, currentCount, CacheCountThreshold,
                                farthestTime);
                            this.OutgoingCacheDelayed(null, args);
                        }
                    }
                }
                catch (System.Exception /*ex*/)
                {
                }
                #endregion
            }
            catch (System.Exception)
            {
            }
        }

        private void CheckIncomgCacheThreshold()
        {
            try
            {
                var incomingPackages = _productCacheReceive.GetData();
                var currentCount = incomingPackages.Count();

                #region "���и������"
                try
                {
                    if (currentCount >= CacheCountThreshold && currentCount % CacheCountThreshold == 0)
                    {
                        var info = new StringBuilder(string.Format("δ������û����ݸ����Ѵﵽ��ֵ({0})��{1}����",
                            CacheCountThreshold, currentCount / CacheCountThreshold));
                    }

                    // �����ն��и����ı䡱�¼�֪ͨ��
                    if (currentCount != _lastReceiveCacheCount
                        && this.IncomingCacheCountChanged != null)
                    {
                        _lastReceiveCacheCount = currentCount;
                        this.IncomingCacheCountChanged(null, new IncomingCacheCountChangedEventArgs(this.Name, currentCount));
                    }
                }
                catch (System.Exception /*ex*/)
                {
                }
                #endregion

                #region "ʱ����"
                try
                {
                    if (currentCount > 0)
                    {
                        var farthestTime = incomingPackages.Min(p => p.CreationTime);
                        var blockingTime = Convert.ToInt32((DateTime.Now - farthestTime).TotalSeconds);

                        // �����ն��г�����ֵ���¼�֪ͨ��
                        if (this.IncomingCacheDelayed != null
                            && (blockingTime > BlockingTimeThreshold)
                            && (blockingTime != _lastReceiveQueueBlockingTime))
                        {
                            _lastReceiveQueueBlockingTime = blockingTime;

                            var args = new IncomingCacheDelayedEventArgs(this.Name, currentCount, CacheCountThreshold,
                                farthestTime);

                            this.IncomingCacheDelayed(null, args);
                        }
                    }
                }
                catch (System.Exception /*ex*/)
                {
                }
                #endregion
            }
            catch (System.Exception)
            {
            }
        }
        #endregion

        #region "Protected methods"
        protected string BuildSaiConnectionID(UInt32 localID, UInt32 remoteID)
        {
            return string.Format("SaiConnection_{0}: LID={1}, RID={2}.",
                _rsspConfig.IsInitiator ? "������" : "������",
                localID, remoteID);
        }


        protected void NotifyTcpEndPointListeningEvent(TcpEndPointListeningEventArgs args)
        {
            try
            {
                if (this.TcpEndPointListening != null)
                {
                    this.TcpEndPointListening(null, args);
                }
            }
            catch (System.Exception)
            {
            }
        }

        protected void NotifyTcpEndPointListenFailedEvent(TcpEndPointListenFailedEventArgs args)
        {
            try
            {
                if (this.TcpEndPointListenFailed != null)
                {
                    this.TcpEndPointListenFailed(null, args);
                }
            }
            catch (System.Exception)
            {
            }
        }

        #endregion

        #region "Public methods"

        public void Open()
        {
            CreateProductCache();

            this.OnOpen();
        }

        public void Close()
        {
            this.Dispose();
        }

        public void Send(OutgoingPackage package)
        {
            _productCacheSending.AddTail(package);

            this.CheckOutgoingCacheThreshold();
        }

        public void Disconnect(UInt32 remoteID)
        {
            var key = this.BuildSaiConnectionID(_rsspConfig.LocalID, remoteID);

            var theSaiConnection = this.GetSaiConnection(key);

            if (theSaiConnection != null)
            {
                theSaiConnection.Disconnect(MaslErrorCode.NotDefined, 0);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event EventHandler<LogCreatedEventArgs> LogCreated
        {
            add { LogUtility.LogCreated += value; }
            remove { LogUtility.LogCreated -= value; }
        }

        public event EventHandler<TcpConnectingEventArgs> TcpConnecting;
        public event EventHandler<TcpConnectedEventArgs> TcpConnected;
        public event EventHandler<TcpConnectFailedEventArgs> TcpConnectFailed;
        public event EventHandler<TcpDisconnectedEventArgs> TcpDisconnected;

        public event EventHandler<TcpEndPointListeningEventArgs> TcpEndPointListening;
        public event EventHandler<TcpEndPointListenFailedEventArgs> TcpEndPointListenFailed;

        public event EventHandler<NodeConnectedEventArgs> NodeConnected;
        public event EventHandler<NodeInterruptionEventArgs> NodeDisconnected;

        public event EventHandler<UserDataIncomingEventArgs> UserDataIncoming;
        public event EventHandler<UserDataOutgoingEventArgs> UserDataOutgoing;

        public event EventHandler<OutgoingCacheCountChangedEventArgs> OutgoingCacheCountChanged;
        public event EventHandler<IncomingCacheCountChangedEventArgs> IncomingCacheCountChanged;

        public event EventHandler<IncomingCacheDelayedEventArgs> IncomingCacheDelayed;
        public event EventHandler<OutgoingCacheDelayedEventArgs> OutgoingCacheDelayed;
        #endregion


        #region "ISaiConnectionObserver"
        void ISaiConnectionObserver.OnSaiConnected(uint localID, uint remoteID)
        {
            try
            {
                if (this.NodeConnected != null)
                {
                    var args = new NodeConnectedEventArgs(localID, remoteID);
                    this.NodeConnected(null, args);
                }
            }
            catch (System.Exception)
            {
            }
        }

        public virtual void OnSaiDisconnected(uint localID, uint remoteID)
        {
            try
            {
                if (this.NodeDisconnected != null)
                {
                    var args = new NodeInterruptionEventArgs(localID, remoteID);
                    this.NodeDisconnected(null, args);
                }
            }
            catch (System.Exception)
            {
            }
        }

        void ISaiConnectionObserver.OnSaiUserDataArrival(uint remoteID, byte[] userData, long timeDelay, MessageDelayDefenseTech defenseTech)
        {
            try
            {
                var pkg = new IncomingPackage(remoteID, userData, timeDelay, defenseTech);

                _productCacheReceive.AddTail(pkg);

                this.CheckIncomgCacheThreshold();
            }
            catch (System.Exception)
            {
            }
        }
        #endregion


        #region "ITunnelEventNotifier�ӿ�"
        void IAleTunnelEventNotifier.NotifyTcpConnecting(TcpConnectingEventArgs args)
        {
            try
            {
                if (this.TcpConnecting != null)
                {
                    this.TcpConnecting(null, args);
                }
            }
            catch (System.Exception)
            {            	
            }
        }

        void IAleTunnelEventNotifier.NotifyTcpConnected(TcpConnectedEventArgs args)
        {
            try
            {
                if (this.TcpConnected != null)
                {
                    this.TcpConnected(null, args);
                }
            }
            catch (System.Exception )
            {
            }
        }

        void IAleTunnelEventNotifier.NotifyTcpConnectFailure(TcpConnectFailedEventArgs args)
        {
            try
            {
                if (this.TcpConnectFailed != null)
                {
                    this.TcpConnectFailed(null, args);
                }
            }
            catch (System.Exception)
            {
            }
        }

        void IAleTunnelEventNotifier.NotifyTcpDisconnected(TcpDisconnectedEventArgs args)
        {
            try
            {
                if (this.TcpDisconnected != null)
                {
                    this.TcpDisconnected(null, args);
                }
            }
            catch (System.Exception)
            {
            }
        }
        #endregion
    }
}
