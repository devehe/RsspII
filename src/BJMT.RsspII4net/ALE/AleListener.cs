/*----------------------------------------------------------------
// 公司名称：北京交大微联科技有限公司
// 
// 项目名称：BJMT Platform Library
//
// 创 建 人：zhangheng
// 创建日期：2016-11-23 9:03:50 
// 邮    箱：zhangheng@bjmut.com
//
// Copyright (C) 北京交大微联科技有限公司，保留所有权利。
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BJMT.RsspII4net.ALE
{
    interface IAleListenerObserver
    {
        void OnEndPointListening(TcpListener endPoint);
        void OnEndPointListenFailed(TcpListener endPoint, string message);

        void OnAcceptTcpClient(TcpClient tcpClient);
    }

    /// <summary>
    /// ALE监听器。
    /// </summary>
    class AleListener : IDisposable
    {
        #region "Filed"
        private bool _disposed = false;
        private List<TcpListener> _tcpListeners = new List<TcpListener>();
        private IAleListenerObserver _observer;
        #endregion

        #region "Constructor"
        public AleListener(IEnumerable<IPEndPoint> ipEndPoints, IAleListenerObserver observer)
        {
            if (observer == null || ipEndPoints == null || ipEndPoints.Count() == 0)
            {
                throw new ArgumentException();
            }

            _observer = observer;

            ipEndPoints.ToList().ForEach(p =>
            {
                var item = new TcpListener(p);
                _tcpListeners.Add(item);
            });
        }

        ~AleListener()
        {
            this.Dispose(false);
        }
        #endregion

        #region "Properties"

        #endregion

        #region "Virtual methods"
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _tcpListeners.ToList().ForEach(p => p.Stop());
                    _tcpListeners.Clear();
                }
                _disposed = true;
            }
        }
        #endregion

        #region "Override methods"
        #endregion

        #region "Private methods"

        private void BeginAccept(TcpListener theListener)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (!_disposed)
                    {
                        theListener.Start();
                        theListener.BeginAcceptTcpClient(AcceptCallback, theListener);

                        // 事件通知。                    
                        _observer.OnEndPointListening(theListener);
                    }
                }
                catch (System.Exception ex)
                {
                    // 事件通知。                
                    _observer.OnEndPointListenFailed(theListener, ex.Message);

                    // 重新尝试
                    Thread.Sleep(5000);
                    this.BeginAccept(theListener);
                }
            });
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            var theListener = ar.AsyncState as TcpListener;
            TcpClient tcpClient = null;

            try
            {
                tcpClient = theListener.EndAcceptTcpClient(ar);     

                // 继续监听
                this.BeginAccept(theListener);
            }
            catch (ObjectDisposedException ex)
            {
                _observer.OnEndPointListenFailed(theListener, ex.Message);
            }
            catch (System.Exception /*ex*/)
            {
                this.BeginAccept(theListener);
            }

            try
            {
                if (tcpClient != null)
                {
                    _observer.OnAcceptTcpClient(tcpClient);
                }
            }
            catch (System.Exception /*ex*/)
            {
                tcpClient.Close();
            }
        }

        #endregion

        #region "Public methods"
        public void Start()
        {
            _tcpListeners.ToList().ForEach(p =>
            {
                this.BeginAccept(p);
            });
        }
        
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
