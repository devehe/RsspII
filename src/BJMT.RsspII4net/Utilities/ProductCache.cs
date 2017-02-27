/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�Microunion Foundation Component Library
//
// �� �� �ˣ�zhh_217
// �������ڣ�08/31/2011 08:53:27 
// ��    �䣺zhh_217@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾ 2009-2015 ��������Ȩ��
//
//----------------------------------------------------------------*/

using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace BJMT.RsspII4net.Utilities
{

    /// <summary>
    /// ��Ʒ������ࡣ�μ�OS��������-�����������еĻ���ء�
    /// </summary>
    class ProductCache<TProduct> : IDisposable
    {
        #region "Filed"

        private bool _disposed = false;

        /// <summary>
        /// ��Ʒ����
        /// </summary>
        private ThreadSafetyList<TProduct> _productQueue = null;

        /// <summary>
        /// ������󳤶�
        /// </summary>
        private UInt32 _queueMaxLength = UInt32.MaxValue;

        /// <summary>
        /// -1��ʾ����֪ͨ�����ߣ�����0��ʾ��Ʒ���뻺�����ӳ�֪ͨʱ�䣨ms����
        /// </summary>
        private Int32 _semephoreWatiTime = Timeout.Infinite;

        /// <summary>
        /// �����߳�
        /// </summary>
        private Thread _dataHandleThread = null;
        /// <summary>
        /// �߳�����
        /// </summary>
        private string _dataHandleThreadName = "����ش����߳�";

        /// <summary>
        /// �ź���
        /// </summary>
        private Semaphore _cacheSemaphore = null;

        #endregion

        #region "Constructor"
        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public ProductCache()
        {
        }

        /// <summary>
        /// �սắ��
        /// </summary>
        ~ProductCache()
        {
            this.Dispose(false);
        }
        /// <summary>
        /// �������Ĺ��캯��
        /// </summary>
        /// <param name="timeout">��ʾ��Ʒ�ӷ��뻺��ص�֪ͨ�����ѵ�ʱ��(ms)����ֵ��ʾ����֪ͨ��</param>
        public ProductCache(Int32 timeout)
        {
            if (timeout < 0)
            {
                throw new ArgumentException("��Ʒ�ӷ��뻺��ص�֪ͨ�����ѵ�ʱ��ֵ����Ϊһ���Ǹ�ֵ��");
            }

            this.TimeOut = timeout;
        }
        /// <summary>
        /// �������Ĺ��캯��
        /// </summary>
        /// <param name="capacity">���������Ʒ����</param>
        /// <param name="timeout">��ʾ��Ʒ�ӷ��뻺��ص�֪ͨ�����ѵ�ʱ��(ms)����ֵ��ʾ����֪ͨ��</param>
        public ProductCache(UInt32 capacity, Int32 timeout)
        {
            if (timeout < 0)
            {
                throw new ArgumentException("��Ʒ�ӷ��뻺��ص�֪ͨ�����ѵ�ʱ��ֵ����Ϊһ���Ǹ�ֵ��");
            }

            this.Capacity = capacity;
            this.TimeOut = timeout;
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// һ���¼������в�Ʒ����ʱ����
        /// </summary>
        public event EventHandler<ProductCreatedEventArgs<TProduct>> ProductCreated;

        /// <summary>
        /// ��ȡһ��ֵ�����ڱ�ʾ������Ƿ��ڴ�״̬��
        /// </summary>
        public bool IsOpen
        {
            get { return _dataHandleThread != null; }
        }

        /// <summary>
        /// ��ȡ������в�Ʒ�ĸ���
        /// </summary>
        public Int32 Count
        {
            get
            {
                if (_productQueue != null)
                {
                    return _productQueue.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// ��ȡ/����һ��ֵ�����ڱ�ʾ������пɴ�Ų�Ʒ��������
        /// </summary>
        public UInt32 Capacity
        {
            get { return _queueMaxLength; }
            set { _queueMaxLength = value; }
        }
        /// <summary>
        /// �Ƿ�Ϊ�ӳ�֪ͨ
        /// </summary>
        public bool DelayNotify
        {
            get { return _semephoreWatiTime > 0; }
        }

        /// <summary>
        /// ��ȡ/����һ��ֵ�����ڱ�ʾ��Ʒ�ӷ��뻺��ص�֪ͨ�����ѵ�ʱ��(ms)��
        /// </summary>
        public Int32 TimeOut
        {
            get 
            { 
                if (_semephoreWatiTime == -1)
                {
                    return 0;
                }
                else
                {
                    return _semephoreWatiTime; 
                }
            }

            set
            {
                if (this.IsOpen)
                {
                    throw new InvalidOperationException("�����Դ�֮�����ô�ֵ��");
                }

                if (value == 0)
                {
                    _semephoreWatiTime = -1; 
                }
                else
                {
                    _semephoreWatiTime = value; 
                }
            }
        }

        /// <summary>
        /// ��ȡ/����һ��ֵ�����ڱ�ʾ���ݴ����̵߳����ơ�
        /// </summary>
        public string ThreadName
        {
            get { return _dataHandleThreadName; }
            set { _dataHandleThreadName = (value != null) ? value : ""; }
        }
        #endregion

        #region "Override methods"
        #endregion

        #region "Private methods"
        /// <summary>
        /// �����߳�
        /// </summary>
        private void ThreadEntry()
        {
            try
            {
                while (true)
                {                    
                    _cacheSemaphore.WaitOne(_semephoreWatiTime);
                    
                    List<TProduct> products = this.DequeueAll();

                    if (products.Count > 0)
                    {
                        ProductCreatedEventArgs<TProduct> args = new ProductCreatedEventArgs<TProduct>(products);
                        NotifyEvent(args);
                    }
                }
            }
            catch (System.Exception)
            {
            }
        }

        /// <summary>
        /// ֪ͨ�¼�
        /// </summary>
        private void NotifyEvent(ProductCreatedEventArgs<TProduct> args)
        {
            try
            {
                if (this.ProductCreated != null)
                {
                    var operators = this.ProductCreated.GetInvocationList();

                    foreach (var item in operators)
                    {
                        try
                        {
                            item.DynamicInvoke(this, args);
                        }
                        catch (System.Exception )
                        {
                        }
                    }
                }
            }
            catch (System.Exception )
            {

            }
        }
        
        /// <summary>
        /// ȡ����һ����Ʒ
        /// </summary>
        /// <returns></returns>
        private TProduct DequeueHead()
        {
            lock (_productQueue.SyncRoot)
            {
                TProduct result = default(TProduct);

                if (_productQueue.Count > 0)
                {
                    result = _productQueue[0];
                    _productQueue.RemoveAt(0);
                }

                return result;
            }
        }

        /// <summary>
        /// ȡ�����в�Ʒ
        /// </summary>
        /// <returns></returns>
        private List<TProduct> DequeueAll()
        {
            lock (_productQueue.SyncRoot)
            {
                List<TProduct> result = new List<TProduct>();
                result.AddRange(_productQueue);

                // ��ն���
                _productQueue.Clear();

                // ����ź���
                while (_cacheSemaphore.WaitOne(0)) ;

                return result;
            }
        }
        #endregion

        #region "Public methods"

        /// <summary>
        /// �򿪻����
        /// </summary>
        public void Open()
        {
            // ����Ƿ��
            if (this.IsOpen)
            {
                throw new ApplicationException("������Ѿ���");
            }

            // Create product queue.
            _productQueue = new ThreadSafetyList<TProduct>();

            // Create semaphore
            _cacheSemaphore = new Semaphore(0, Int32.MaxValue);            

            // Create work-thread
            _dataHandleThread = new Thread(new ThreadStart(ThreadEntry));
            _dataHandleThread.Name = _dataHandleThreadName;
            _dataHandleThread.IsBackground = true;
            _dataHandleThread.Start();
        }

        /// <summary>
        /// �رջ����
        /// </summary>
        public void Close()
        {
            ((IDisposable)this).Dispose();
        }

        /// <summary>
        /// ���һ����ƷƤ����ض��ס�
        /// </summary>
        /// <param name="product"></param>
        public void AddHead(TProduct product)
        {
            // ����Ƿ��
            if (!this.IsOpen)
            {
                throw new ApplicationException("��Ӳ�Ʒǰ�����ȴ򿪻����");
            }

            if (this.Count >= this.Capacity)
            {
                throw new ApplicationException("��Ʒ���������");
            }

            // Enqueue
            lock (_productQueue.SyncRoot)
            {
                _productQueue.Insert(0, product);

                if (!this.DelayNotify)
                {
                    _cacheSemaphore.Release();
                }
            }
        }

        /// <summary>
        /// ���һ����ƷƤ����ض�β��
        /// </summary>
        /// <param name="product"></param>
        public void AddTail(TProduct product)
        {
            // ����Ƿ��
            if (!this.IsOpen)
            {
                return;
            }

            if (this.Count >= this.Capacity)
            {
                throw new ApplicationException("��Ʒ���������");
            }

            // Enqueue
            lock (_productQueue.SyncRoot)
            {
                _productQueue.Add(product);

                if (!this.DelayNotify)
                {
                    _cacheSemaphore.Release();
                }
            }
        }

        /// <summary>
        /// ��ջ���ء�
        /// </summary>
        /// <returns>�������ǰ��Ԫ�ء�</returns>
        public List<TProduct> Clear()
        {
            return this.DequeueAll();
        }

        public TProduct GetAt(int index)
        {
            return _productQueue[index];
        }

        /// <summary>
        /// ���ػ����е���������
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TProduct> GetData()
        {
            lock (_productQueue.SyncRoot)
            {
                return _productQueue.ToList();
            }
        }
        #endregion


        #region IDisposable ��Ա

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        _productQueue.Clear();

                        if (_dataHandleThread != null)
                        {
                            _dataHandleThread.Abort();
                        }

                        if (_cacheSemaphore != null)
                        {
                            _cacheSemaphore.Close();
                        }
                    }
                    catch (System.Exception )
                    {
                    }
                    finally
                    {
                        this.ProductCreated = null;
                        _dataHandleThread = null;
                        _cacheSemaphore = null;
                    }

                }

                _disposed = true;
            }
        }
        
        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
