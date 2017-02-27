/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-22 15:42:55 
// ��    �䣺zhh_217@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using BJMT.RsspII4net.Events;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.Controls
{
    /// <summary>
    /// RSSP-IIͨѶ���ӿؼ���
    /// </summary>
    public partial class RsspMonitorControl : UserControl
    {
        #region "Field"
        /// <summary>
        /// ͨѶ�ӿ��б�
        /// </summary>
        private List<IRsspNode> _commList = new List<IRsspNode>();

        /// <summary>
        /// �¼�����ء�
        /// </summary>
        private ProductCache<EventArgs> _productCache = new ProductCache<EventArgs>();

        /// <summary>
        /// ��ǰѡ���Զ���豸���ơ�
        /// </summary>
        private string _selectedDeviceName = "";

        /// <summary>
        /// �������ؼ���
        /// </summary>
        private FilterControl _filterControl = null;

        /// <summary>
        /// ���������ʾ�ؼ���
        /// </summary>
        private CacheCountControl _cacheCountCtrl = new CacheCountControl() { Dock = DockStyle.Fill };

        #endregion

        #region "Property"

        /// <summary>
        /// �ڻ��ڵ�ID��
        /// </summary>
        public uint SiblingID { get; private set; }

        /// <summary>
        /// �Ƿ���ʾ��������
        /// </summary>
        public bool IncomingStreamVisable
        {
            get { return _IncomingStreamVisable; }
            set
            {
                _IncomingStreamVisable = value;
                this.chkInputStreamVisable.Checked = value;
            }
        }
        private bool _IncomingStreamVisable = false;

        /// <summary>
        /// �Ƿ���ʾ�������
        /// </summary>
        public bool OutgoingStreamVisable
        {
            get { return _OutgoingStreamVisable; }
            set
            {
                _OutgoingStreamVisable = value;
                this.chkOutputStreamVisable.Checked = value;
            }
        }
        private bool _OutgoingStreamVisable = false;

        /// <summary>
        /// ��ȡ/����һ��ֵ�����ڱ�ʾ�Ƿ�ͬ��ˢ��Э��֡����ϸ��Ϣ��
        /// </summary>
        public bool SynchronousRefresh
        {
            get { return this.chkSyncRefresh.Checked; }
            set { this.chkSyncRefresh.Checked = value; }
        }

        /// <summary>
        /// ��ȡ����״̬�ؼ������Ĳ˵���������
        /// </summary>
        public ToolStripItemCollection StateContextMenuItems
        {
            get { return this.contextMenuCommStatus.Items; }
        }
        /// <summary>
        /// ��ȡʵʱ���ݿؼ������Ĳ˵���������
        /// </summary>
        public ToolStripItemCollection AliveDataContextMenuItems { get { return this.contextMenuAliveData.Items; } }
        /// <summary>
        /// ��ȡ��־�ؼ������Ĳ˵���������
        /// </summary>
        public ToolStripItemCollection LogContextMenuItems { get { return this.contextMenuLog.Items; } }

        /// <summary>
        /// ��ȡ����TabPage��
        /// </summary>
        public TabPage[] TabPages
        {
            get 
            {
                var result = new List<TabPage>();
                var count = this.tabControl1.TabPages.Count;

                for (int i = 0; i < count;  i++)
                {
                    result.Add(this.tabControl1.TabPages[i]);
                }
                
                return result.ToArray();
            }
        }

        /// <summary>
        /// ��ȡ���ؼ�������ͼ���б� 
        /// </summary>
        public ImageList ImageList { get { return this.imgListState; } }


        /// <summary>
        /// ��ȡ/�����豸���ƽ������ӿڡ�
        /// </summary>
        public IRsspNodeNameResolver NameResolver { get; set; }
        /// <summary>
        /// ��ȡ/�����û�Э��֡�������ӿڡ�
        /// </summary>
        public IRsspUserDataResolver UserDataResolver { get; set; }
        #endregion


        #region "Constructor && Destructor"
        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public RsspMonitorControl()
        {
            InitializeComponent();

            this.splitContainer2.Dock = DockStyle.Fill;
            this.treeViewNetwork.Dock = DockStyle.Fill;
            this.listViewConnectionLog.Dock = DockStyle.Fill;
            this.treeViewDataSummary.Dock = DockStyle.Fill;
            this.txtDataDetailed.Dock = DockStyle.Fill;
            this.listViewLog.Dock = DockStyle.Fill;
            this.splitContainer4.IsSplitterFixed = true;

            if (cbxCurrentNodeID.Items.Count > 0)
            {
                cbxCurrentNodeID.SelectedIndex = 0;
            }
            
            _filterControl = new FilterControl() { Dock = DockStyle.Fill };
            groupBoxFilter.Controls.Add(_filterControl);
            
            this.splitContainer3.Panel2.Controls.Add(_cacheCountCtrl);

            // �򿪻���ء�
            _productCache.ThreadName = "RsspMonitorControl������߳�";
            _productCache.ProductCreated += OnCacheProductCreated;
            _productCache.Open();
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="siblingId">�ڻ��豸ID������������ڻ�����ʹ�ÿ��ַ���������á�</param>
        /// <param name="nameReolver">�豸���ƽ������ӿڣ�����Ϊ�����á�</param>
        /// <param name="frameResolver">Э��������ӿڣ�����Ϊ�����á�</param>
        public RsspMonitorControl(uint siblingId,
            IRsspNodeNameResolver nameReolver,
            IRsspUserDataResolver frameResolver) : this()
        {
            NameResolver = nameReolver;
            UserDataResolver = frameResolver;
            this.SiblingID = siblingId;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="siblingId">�ڻ��豸ID������������ڻ�����ʹ�ÿ��ַ���������á�</param>
        /// <param name="nameReolver">�豸���ƽ������ӿڣ�����Ϊ�����á�</param>
        /// <param name="frameResolver">Э��������ӿڣ�����Ϊ�����á�</param>
        /// <param name="filterProvider">�������ṩ�߽ӿڣ�Ϊ������ʱʹ��Ĭ�Ϲ��˿ؼ���</param>
        public RsspMonitorControl(uint siblingId,
            IRsspNodeNameResolver nameReolver,
            IRsspUserDataResolver frameResolver,
            IFilterControlProvider filterProvider)
            : this(siblingId, nameReolver, frameResolver)
        {
            if (filterProvider == null) throw new ArgumentNullException();            

            // ��ʼ�����˿ؼ���
            _filterControl.AddCustomFilter(filterProvider);
        }
        #endregion


        #region "public mehtods"
        
        /// <summary>
        /// ���ͨѶ�ӿ�
        /// </summary>
        /// <param name="handlers">��Ҫ��ӵ�ͨѶ�ӿڡ�</param>
        public void AddCommHandler(IEnumerable<IRsspNode> handlers)
        {
            foreach (var item in handlers)
            {
                this.AddCommHandler(item);
            }
        }

        /// <summary>
        /// ���ͨѶ�ӿ�
        /// </summary>
        /// <param name="handler"></param>
        public void AddCommHandler(IRsspNode handler)
        {
            if (handler != null)
            {
                if (!_commList.Contains(handler))
                {
                    _commList.Add(handler);

                    handler.TcpEndPointListening += OnTcpEndPointListening;
                    handler.TcpEndPointListenFailed += OnTcpEndPointListenFailed;
                    
                    handler.TcpConnecting += OnTcpConnecting;
                    handler.TcpConnected += OnTcpConnected;
                    handler.TcpConnectFailed += OnTcpConnectFailed;
                    handler.TcpDisconnected += OnTcpDisconnected;

                    handler.NodeConnected += OnNodeConnected;
                    handler.NodeDisconnected += OnNodeInterruption;

                    handler.UserDataIncoming += OnUserDataIncoming;
                    handler.UserDataOutgoing += OnUserDataOutgoing;
                    
                    handler.IncomingCacheCountChanged += OnIncomingCacheCountChanged;
                    handler.OutgoingCacheCountChanged += OnOutgoingCacheCountChanged;
                }
            }
        }

        /// <summary>
        /// �Ƴ�ָ����ͨѶ�ӿ�
        /// </summary>
        /// <param name="handler">��Ҫ�Ƴ���ͨѶ�ӿڡ�</param>
        public void RemoveCommHandler(IRsspNode handler)
        {
            if (_commList != null)
            {
                if (_commList.Contains(handler))
                {
                    handler.TcpEndPointListening -= OnTcpEndPointListening;
                    handler.TcpEndPointListenFailed -= OnTcpEndPointListenFailed;

                    handler.TcpConnecting -= OnTcpConnecting;
                    handler.TcpConnected -= OnTcpConnected;
                    handler.TcpConnectFailed -= OnTcpConnectFailed;
                    handler.TcpDisconnected -= OnTcpDisconnected;

                    handler.NodeConnected -= OnNodeConnected;
                    handler.NodeDisconnected -= OnNodeInterruption;

                    handler.UserDataIncoming -= OnUserDataIncoming;
                    handler.UserDataOutgoing -= OnUserDataOutgoing;

                    handler.IncomingCacheCountChanged -= OnIncomingCacheCountChanged;
                    handler.OutgoingCacheCountChanged -= OnOutgoingCacheCountChanged;

                    _commList.Remove(handler);
                }
            }
        }
        /// <summary>
        /// ���ͨѶ�ӿ�
        /// </summary>
        public void ClearCommHandler()
        {
            if (_commList != null)
            {
                // ����UI
                this.treeViewNetwork.Nodes.Clear();
                this.cbxCurrentNodeID.Items.Clear();
                _cacheCountCtrl.Reset();

                // �Ƴ�����ͨѶ�ӿ�
                _commList.ForEach(p => this.RemoveCommHandler(p));
                _commList.Clear();
            }
        }
        
        /// <summary>
        /// ���ӱ��ؼ�����
        /// </summary>
        public TreeNode AddListeningEndPointToTreeview(uint deviceId)
        {
            try
            {
                string deviceListenKey = string.Format("{0}", deviceId);

                // ��Ӽ����豸
                TreeNode[] nodesTemp = treeViewNetwork.Nodes.Find(deviceListenKey, false);

                TreeNode nodeListenDevice = null;
                if (nodesTemp.Length == 0)
                {
                    string showInfo = String.Format("���ؼ�����: {0}", DecorateDeviceID(deviceId));

                    nodeListenDevice = treeViewNetwork.Nodes.Add(deviceListenKey, showInfo);

                    nodeListenDevice.ForeColor = Color.Blue;
                    nodeListenDevice.ImageKey = "DeviceOnline";
                    nodeListenDevice.SelectedImageKey = "DeviceOnline";
                }
                else
                {
                    nodeListenDevice = nodesTemp[0];
                }

                return nodeListenDevice;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException(string.Format("����豸������{0}ʱ��������", deviceId), ex);
            }
        }

        /// <summary>
        /// ����һ���������״̬
        /// </summary>
        /// <param name="deviceId">������ĸ����豸ID</param>
        /// <param name="endPoint">������</param>
        /// <param name="success">�Ƿ�����ɹ�</param>
        public void UpdateListeningPoint(uint deviceId, IPEndPoint endPoint, bool success)
        {
            try
            {
                var deviceListenKey = string.Format("{0}", deviceId);
                var listenKey = String.Format("{0}", endPoint);

                // ���Ҹ���Node
                var nodesTemp = treeViewNetwork.Nodes.Find(deviceListenKey, false);

                TreeNode nodeListenDevice = null;
                if (nodesTemp.Length == 0) // ���û���ҵ������ڵ㣬�򴴽��˽ڵ㡣
                {
                    nodeListenDevice = this.AddListeningEndPointToTreeview(deviceId); 
                }
                else
                {
                    nodeListenDevice = nodesTemp[0];
                }


                // ����Endpoint�ڵ�
                TreeNode endPointListen = null;
                nodesTemp = nodeListenDevice.Nodes.Find(listenKey, false);
                if (nodesTemp.Length == 0) // ���û���ҵ�Endpoint�����㣬�򴴽��˽ڵ�
                {
                    endPointListen = nodeListenDevice.Nodes.Add(listenKey, "");
                }
                else
                {
                    endPointListen = nodesTemp[0];
                }

                // ���ü���״̬
                if (success)
                {
                    endPointListen.Text = String.Format("Listening {0} since {1}.", endPoint, DateTime.Now);
                    endPointListen.ImageKey = "listen";
                    endPointListen.SelectedImageKey = "listen";
                }
                else
                {
                    endPointListen.Text = String.Format("Listening {0} failed at {1}.", endPoint, DateTime.Now);
                    endPointListen.ImageKey = "fail";
                    endPointListen.SelectedImageKey = "fail";
                }
            }
            catch (System.Exception ex)
            {
                throw new Exception(string.Format("���EndPoint������{0}ʱ��������", deviceId), ex);
            }
        }

        /// <summary>
        /// ���һ���ڻ��ڵ㡣
        /// </summary>
        public TreeNode AddSibling(uint deviceId)
        {
            try
            {
                string fixedName = DecorateDeviceID(deviceId);

                // ����Device�ڵ�
                TreeNode[] nodesTemp = treeViewNetwork.Nodes.Find(deviceId.ToString(), false);

                // ���TreeNode
                TreeNode nodeRemoteDevice;
                if (nodesTemp.Length == 0)
                {
                    string showInfo = String.Format("�ڻ�: {0}", fixedName);

                    nodeRemoteDevice = treeViewNetwork.Nodes.Add(deviceId.ToString(), showInfo);

                    // setting
                    nodeRemoteDevice.ForeColor = Color.Red;
                    nodeRemoteDevice.ImageKey = "DeviceOffline";
                    nodeRemoteDevice.SelectedImageKey = "DeviceOffline";
                }
                else
                {
                    nodeRemoteDevice = nodesTemp[0];
                }

                // ���ComboBox Item
                this.AddSelectableDevice(fixedName);

                return nodeRemoteDevice;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("����ڻ��ڵ�ʱ����", ex);
            }
        }

        /// <summary>
        /// ��TreeView�����ӷ������ڵ�
        /// </summary>
        public TreeNode AddServer(uint deviceId)
        {
            try
            {
                string fixedName = DecorateDeviceID(deviceId);

                // ����Device�ڵ�
                TreeNode[] nodesTemp = treeViewNetwork.Nodes.Find(deviceId.ToString(), false);

                // ���TreeNode
                TreeNode nodeRemoteDevice = null;
                if (nodesTemp.Length == 0)
                {
                    string showInfo = String.Format("������: {0}", DecorateDeviceID(deviceId));

                    nodeRemoteDevice = treeViewNetwork.Nodes.Add(deviceId.ToString(), showInfo);

                    // setting
                    nodeRemoteDevice.ForeColor = Color.Red;
                    nodeRemoteDevice.ImageKey = "DeviceOffline";
                    nodeRemoteDevice.SelectedImageKey = "DeviceOffline";
                }
                else
                {
                    nodeRemoteDevice = nodesTemp[0];
                }

                // ���ComboBox Item
                this.AddSelectableDevice(fixedName);

                return nodeRemoteDevice;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("��ӷ������ڵ�ʱ����", ex);
            }
        }

        /// <summary>
        /// ����һ���ͻ��˽ڵ�
        /// </summary>
        public TreeNode AddClient(uint deviceId)
        {
            try
            {
                string fixedName = DecorateDeviceID(deviceId);

                // ����Device�ڵ�
                TreeNode[] nodesTemp = treeViewNetwork.Nodes.Find(deviceId.ToString(), false);

                // ���TreeNode
                TreeNode nodeRemoteDevice = null;
                if (nodesTemp.Length == 0)
                {
                    string showInfo = String.Format("�ͻ���: {0}", DecorateDeviceID(deviceId));

                    nodeRemoteDevice = treeViewNetwork.Nodes.Add(deviceId.ToString(), showInfo);

                    // setting
                    nodeRemoteDevice.ForeColor = Color.Red;
                    nodeRemoteDevice.ImageKey = "DeviceOffline";
                    nodeRemoteDevice.SelectedImageKey = "DeviceOffline";
                }
                else
                {
                    nodeRemoteDevice = nodesTemp[0];
                }

                // ���ComboBox Item
                this.AddSelectableDevice(fixedName);
                return nodeRemoteDevice;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("��ӿͻ��˽ڵ�ʱ����", ex);
            }
        }

        #endregion


        #region "private methods"
        /// <summary>
        /// װ���豸ID
        /// </summary>
        private string DecorateDeviceID(uint deviceId)
        {
            if (NameResolver != null)
            {
                return string.Format("{0} - {1}", deviceId, NameResolver.Convert(deviceId));
            }
            else
            {
                return deviceId.ToString();
            }
        }
        /// <summary>
        /// ����ָ���Ŀ�ѡ�豸
        /// </summary>
        private object FindSelectableDevice(string deviceId)
        {
            object result = null;

            foreach (object item in cbxCurrentNodeID.Items)
            {
                if (item.ToString().CompareTo(deviceId) == 0)
                {
                    result = item;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// ���һ����ѡ�豸
        /// </summary>
        private void AddSelectableDevice(string fixedName)
        {
            if (this.FindSelectableDevice(fixedName) == null)
            {
                cbxCurrentNodeID.Items.Add(fixedName);
            }

            if (cbxCurrentNodeID.Items.Count > 0 && cbxCurrentNodeID.SelectedIndex == -1)
            {
                cbxCurrentNodeID.SelectedIndex = 0;
            }
        }
        


        /// <summary>
        /// ��ָ������־��ʾ���ؼ�
        /// </summary>
        private void ShowLog(string log)
        {
            try
            {
                // clear old data.
                if (listViewLog.Items.Count > 200)
                {
                    listViewLog.Items.Clear();
                }

                // Create one item and three sets of subitems for it. 
                ListViewItem item = new ListViewItem(DateTime.Now.ToString());
                item.SubItems.Add("Info");
                item.SubItems.Add(log);
                listViewLog.Items.Add(item);
            }
            catch (System.Exception)
            {
            }
        }



        private void OnTcpConnecting(object sender, TcpConnectingEventArgs e)
        {
            try
            {
                _productCache.AddTail(e);
            }
            catch (System.Exception)
            {
            }
        }
        private void OnTcpConnected(object sender, TcpConnectedEventArgs e)
        {
            try
            {
                _productCache.AddTail(e);
            }
            catch (System.Exception)
            {
            }
        }
        private void OnTcpConnectFailed(object sender, TcpConnectFailedEventArgs e)
        {
            try
            {
                _productCache.AddTail(e);
            }
            catch (System.Exception)
            {
            }
        }
        private void OnTcpDisconnected(object sender, TcpDisconnectedEventArgs e)
        {
            try
            {
                _productCache.AddTail(e);
            }
            catch (System.Exception)
            {
            }
        }
        private void OnTcpEndPointListening(object sender, TcpEndPointListeningEventArgs e)
        {
            try
            {
                _productCache.AddTail(e);
            }
            catch (System.Exception)
            {
            }
        }
        private void OnTcpEndPointListenFailed(object sender, TcpEndPointListenFailedEventArgs e)
        {
            try
            {
                _productCache.AddTail(e);
            }
            catch (System.Exception)
            {
            }
        }

        private void OnNodeConnected(object sender, NodeConnectedEventArgs e)
        {
            try
            {
                _productCache.AddTail(e);
            }
            catch (System.Exception)
            {
            }
        }
        private void OnNodeInterruption(object sender, NodeInterruptionEventArgs e)
        {
            try
            {
                _productCache.AddTail(e);
            }
            catch (System.Exception)
            {
            }
        }

        private void OnUserDataOutgoing(object sender, UserDataOutgoingEventArgs args)
        {
            try
            {
                _productCache.AddTail(args);
            }
            catch (System.Exception)
            {
            }
        }
        private void OnUserDataIncoming(object sender, UserDataIncomingEventArgs e)
        {
            try
            {
                _productCache.AddTail(e);
            }
            catch (System.Exception)
            {
            }
        }

        private void OnOutgoingCacheCountChanged(object sender, OutgoingCacheCountChangedEventArgs args)
        {
            try
            {
                var name = string.Format("{0}���ͻ���", args.Name);
                _cacheCountCtrl.ShowCount(name, args.Count);
            }
            catch (System.Exception)
            {
            }
        }
        private void OnIncomingCacheCountChanged(object sender, IncomingCacheCountChangedEventArgs args)
        {
            try
            {
                var name = string.Format("{0}���ջ���", args.Name);
                _cacheCountCtrl.ShowCount(name, args.Count);
            }
            catch (System.Exception)
            {
            }
        }

        private void OnCacheProductCreated(object sender, ProductCreatedEventArgs<EventArgs> e)
        {
            try
            {
                foreach (var args in e.Products)
                {
                    if (args is TcpConnectingEventArgs)
                    {
                        this.ShowTcpConnectingEvent(args as TcpConnectingEventArgs);
                    }
                    else if (args is TcpConnectedEventArgs)
                    {
                        this.ShowTcpConnectedEvent(args as TcpConnectedEventArgs);
                    }
                    else if (args is TcpConnectFailedEventArgs)
                    {
                        this.ShowTcpConnectFailedEvent(args as TcpConnectFailedEventArgs);
                    }
                    else if (args is TcpDisconnectedEventArgs)
                    {
                        this.ShowTcpDisconnectedEvent(args as TcpDisconnectedEventArgs);
                    }
                    else if (args is TcpEndPointListeningEventArgs)
                    {
                        this.ShowTcpEndPointListeningEvent(args as TcpEndPointListeningEventArgs);
                    }
                    else if (args is TcpEndPointListenFailedEventArgs)
                    {
                        this.ShowTcpEndPointListenFailedEvent(args as TcpEndPointListenFailedEventArgs);
                    }
                    else if (args is NodeConnectedEventArgs)
                    {
                        var theArgs = args as NodeConnectedEventArgs;
                        this.ShowNodeConnectedEvent(theArgs);
                        this.ShowNodeConnectionChangedOnLogListView(theArgs.LocalID, theArgs.RemoteID, true);
                    }
                    else if (args is NodeInterruptionEventArgs)
                    {
                        var theArgs = args as NodeInterruptionEventArgs;
                        this.ShowNodeInterruptionEvent(theArgs);
                        this.ShowNodeConnectionChangedOnLogListView(theArgs.LocalID, theArgs.RemoteID, false);
                    }
                    else if (args is UserDataOutgoingEventArgs)
                    {
                        this.ShowOutgoingUserDataEvent(args as UserDataOutgoingEventArgs);
                    }
                    else if (args is UserDataIncomingEventArgs)
                    {
                        this.ShowIncomingUserDataEvent(args as UserDataIncomingEventArgs);
                    }
                }
            }
            catch (System.Exception ex)
            {
                this.ShowLog(ex.Message);
            }
        }
        
        private void ShowTcpEndPointListeningEvent(TcpEndPointListeningEventArgs args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    this.UpdateListeningPoint(args.LocalID, args.EndPoint, true);
                }));
            }
            catch (System.Exception ex)
            {
                this.ShowLog(ex.Message);
            }
        }
        private void ShowTcpEndPointListenFailedEvent(TcpEndPointListenFailedEventArgs args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    this.UpdateListeningPoint(args.LocalID, args.EndPoint, false);
                }));
            }
            catch (System.Exception ex)
            {
                this.ShowLog(ex.Message);
            }
        }
        private void ShowTcpConnectingEvent(TcpConnectingEventArgs args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    // ���Device�ڵ�
                    TreeNode nodeRemoteDevice = null;
                    if (args.RemoteID == this.SiblingID)
                    {
                        nodeRemoteDevice = this.AddSibling(this.SiblingID);
                    }
                    else
                    {
                        nodeRemoteDevice = this.AddServer(args.RemoteID);
                    }

                    // ���Connection�ڵ�
                    var nodeKey = args.ConnectionID;
                    var linkContent = String.Format("����������: LEP_{0} ---> REP_{1} ��ʼ�� {2}",
                                args.LocalEndPoint.Address, args.RemoteEndPoint, DateTime.Now.ToString("HH:mm:ss"));
                    var nodesTemp = nodeRemoteDevice.Nodes.Find(nodeKey, false);
                    TreeNode nodeConnection = null;
                    if (nodesTemp.Length > 0)
                    {
                        nodeConnection = nodesTemp[0];
                        nodeConnection.Text = linkContent;
                    }
                    else
                    {
                        nodeConnection = nodeRemoteDevice.Nodes.Add(nodeKey, linkContent);
                    }
                    nodeConnection.ForeColor = Color.Gray;
                    nodeConnection.ImageKey = "connecting";
                    nodeConnection.SelectedImageKey = "connecting";

                    //
                    nodeRemoteDevice.ExpandAll();
                }));
            }
            catch (System.Exception ex)
            {
                this.ShowLog(ex.Message);
            }
        }
        private void ShowTcpConnectedEvent(TcpConnectedEventArgs args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    var nodeKey = args.ConnectionID;
                    var nodeText = String.Format("LEP_{0} + REP_{1}, Established at {2}.", args.LocalEndPoint, args.RemoteEndPoint, DateTime.Now.ToString());

                    // ���RemoteNode
                    TreeNode theRemoteNode = null;
                    var theRemoteNodes = treeViewNetwork.Nodes.Find(args.RemoteID.ToString(), false);

                    // ��������״̬ͼ
                    TreeNode theLinkNode = null;

                    if (theRemoteNodes.Length == 0)
                    {
                        if (args.RemoteID == this.SiblingID)
                        {
                            theRemoteNode = this.AddSibling(this.SiblingID);
                        }
                        else
                        {
                            theRemoteNode = this.AddClient(args.RemoteID);
                        }
                    }
                    else
                    {
                        theRemoteNode = theRemoteNodes[0];
                    }

                    // ����TcpLink��Ӧ�Ľڵ㡣
                    var nodesTemp = theRemoteNode.Nodes.Find(nodeKey, false);
                    if (nodesTemp.Length > 0)
                    {
                        theLinkNode = nodesTemp[0];
                        theLinkNode.Text = nodeText;
                    }
                    else
                    {
                        theLinkNode = theRemoteNode.Nodes.Add(nodeKey, nodeText);
                    }
                    theLinkNode.ForeColor = Color.Black;
                    theLinkNode.ImageKey = "success";
                    theLinkNode.SelectedImageKey = "success";                    
                }));
            }
            catch (System.Exception ex)
            {
                this.ShowLog(ex.Message);
            }
        }
        private void ShowTcpConnectFailedEvent(TcpConnectFailedEventArgs args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {

                }));
            }
            catch (System.Exception ex)
            {
                this.ShowLog(ex.Message);
            }
        }
        private void ShowTcpDisconnectedEvent(TcpDisconnectedEventArgs args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    var nodeKey = args.ConnectionID;

                    // ��������״̬ͼ
                    var nodeDevice = treeViewNetwork.Nodes.Find(args.RemoteID.ToString(), false);
                    if (nodeDevice.Length != 0)
                    {
                        TreeNode[] nodeSub = nodeDevice[0].Nodes.Find(nodeKey, false);
                        if (nodeSub.Length > 0)
                        {
                            nodeDevice[0].Nodes.Remove(nodeSub[0]);
                        }
                    }
                }));
            }
            catch (System.Exception ex)
            {
                this.ShowLog(ex.Message);
            }
        }

        private void ShowNodeConnectedEvent(NodeConnectedEventArgs args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    // ���
                    var fixedName = this.DecorateDeviceID(args.RemoteID);
                    this.AddSelectableDevice(fixedName);

                    // ���/����Device�ڵ�
                    TreeNode theRemoteNode = null;

                    // ��������״̬ͼ
                    var nodeDevice = this.treeViewNetwork.Nodes.Find(args.RemoteID.ToString(), false);
                    if (nodeDevice.Length != 0)
                    {
                        theRemoteNode = nodeDevice[0];
                    }
                    else
                    {
                        if (args.RemoteID == this.SiblingID)
                        {
                            theRemoteNode = this.AddSibling(args.RemoteID);
                        }
                        else
                        {
                            theRemoteNode = this.AddClient(args.RemoteID);
                        }
                    }

                    theRemoteNode.ImageKey = "DeviceOnline";
                    theRemoteNode.SelectedImageKey = "DeviceOnline";
                    theRemoteNode.ForeColor = Color.Blue;
                }));
            }
            catch (System.Exception ex)
            {
                this.ShowLog(ex.Message);
            }
        }
        private void ShowNodeInterruptionEvent(NodeInterruptionEventArgs args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    // ��������״̬ͼ
                    var nodeDevice = treeViewNetwork.Nodes.Find(args.RemoteID.ToString(), false);
                    if (nodeDevice.Length != 0)
                    {
                        nodeDevice[0].ImageKey = "DeviceOffline";
                        nodeDevice[0].SelectedImageKey = "DeviceOffline";
                        nodeDevice[0].BackColor = treeViewNetwork.BackColor;
                        nodeDevice[0].ForeColor = Color.Red;
                    }
                }));
            }
            catch (System.Exception ex)
            {
                this.ShowLog(ex.Message);
            }
        }
        private void ShowNodeConnectionChangedOnLogListView(uint localID, uint remoteID, bool connected)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    if (listViewConnectionLog.Items.Count > 200)
                    {
                        listViewConnectionLog.Items.RemoveAt(0);
                    }

                    // 
                    string fixedName = this.DecorateDeviceID(remoteID);

                    var lvItem = new ListViewItem(DateTime.Now.ToString());
                    if (connected)
                    {
                        lvItem.SubItems.Add(string.Format("���豸({0})��������", fixedName));
                    }
                    else
                    {
                        lvItem.SubItems.Add(string.Format("���豸({0})�Ͽ�����", fixedName));
                        lvItem.BackColor = Color.Yellow;
                    }

                    this.listViewConnectionLog.Items.Add(lvItem);
                }));
            }
            catch (System.Exception ex)
            {
                this.ShowLog(ex.Message);
            }
        }

        private void ShowIncomingUserDataEvent(UserDataIncomingEventArgs args)
        {
            try
            {
                if (this.IncomingStreamVisable)
                {
                    this.Invoke(new Action(() =>
                    {
                        // ��ǰѡ����豸��args�е��豸�Ƿ�һ��
                        string fixedName = this.DecorateDeviceID(args.Package.RemoteID);
                        bool selected = (_selectedDeviceName.CompareTo(fixedName) == 0);

                        if (selected && _filterControl.Filter(args.Package.UserData))
                        {
                            // clear 
                            if (treeViewDataSummary.Nodes.Count > 100)
                            {
                                treeViewDataSummary.Nodes.RemoveAt(0);
                            }

                            // add
                            TreeNode node = new TreeNode();
                            node.Tag = args;

                            // ��ȡ�Զ����ǩ
                            string customLable;
                            if (UserDataResolver != null)
                            {
                                customLable = string.Format("{0},{1}",
                                    UserDataResolver.GetLabel(args.Package.UserData, true),
                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            }
                            else
                            {
                                customLable = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            }

                            // set node text
                            node.Text = String.Format("{0}, Delay = {1}", customLable, args.Package.TransmissionDelay);

                            // set node image.
                            node.ImageKey = "InputStream";
                            node.SelectedImageKey = node.ImageKey;

                            // Add Node
                            treeViewDataSummary.Nodes.Add(node);
                        }
                    }));
                }
            }
            catch (System.Exception ex)
            {
                this.ShowLog(ex.Message);
            }
        }        
        private void ShowOutgoingUserDataEvent(UserDataOutgoingEventArgs args)
        {
            try
            {
                if (this.OutgoingStreamVisable)
                {
                    this.Invoke(new Action(() =>
                    {
                        #region "��ǰѡ����豸�Ƿ�Ϊָ����Ŀ���豸֮һ"
                        bool selected = false;

                        foreach (var item in args.Package.DestID)
                        {
                            var fixedName = this.DecorateDeviceID(item);
                            selected = (_selectedDeviceName.CompareTo(fixedName) == 0);

                            if (selected) break;
                        }
                        #endregion

                        if (selected && _filterControl.Filter(args.Package.UserData))
                        {
                            // clear 
                            if (treeViewDataSummary.Nodes.Count > 100)
                            {
                                treeViewDataSummary.Nodes.RemoveAt(0);
                            }

                            // add
                            var node = new TreeNode();
                            node.Tag = args;

                            // ��ȡ�Զ����ǩ
                            if (UserDataResolver != null)
                            {
                                node.Text = string.Format("{0},{1}",
                                    UserDataResolver.GetLabel(args.Package.UserData, false),
                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            }
                            else
                            {
                                node.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            }

                            node.ForeColor = Color.Blue;

                            // set node image.
                            node.ImageKey = "OutputStream";
                            node.SelectedImageKey = node.ImageKey;

                            // Add Node
                            treeViewDataSummary.Nodes.Add(node);
                        }
                    }));
                }
            }
            catch (System.Exception ex)
            {
                this.ShowLog(ex.Message);
            }
        }

        private void ShowSelectedNodeAttachedData()
        {
            if (this.treeViewDataSummary.SelectedNode == null) return;

            try
            {
                var iArgs = treeViewDataSummary.SelectedNode.Tag as UserDataIncomingEventArgs;
                IEnumerable<byte> userData = null;

                if (iArgs != null)
                {
                    userData = iArgs.Package.UserData;
                    txtDataDetailed.Text = string.Format("{0}\r\n", iArgs.Package);
                }
                else
                {
                    var oArgs = treeViewDataSummary.SelectedNode.Tag as UserDataOutgoingEventArgs;
                    userData = oArgs.Package.UserData;

                    txtDataDetailed.Text = String.Format("{0} \r\n", oArgs.Package);
                }

                // ��ʾ�û���������
                try
                {
                    if (UserDataResolver != null)
                    {
                        txtDataDetailed.Text += UserDataResolver.GetDescription(userData, iArgs != null);
                    }
                }
                catch (System.Exception ex)
                {
                    txtDataDetailed.Text += "�����û�����ʧ�ܣ�" + ex.Message;
                }
            }
            catch (System.Exception ex)
            {
                txtDataDetailed.Text += ex.ToString();
            }
        }
        #endregion


        #region "�ؼ��¼�"

        /// <summary>
        /// ״̬���ڵ㵥���¼�
        /// </summary>
        private void OnCommStateTreeViewNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                // ����������Ż���Ӧ�����̵����¼�����������Ӧ��
            }
            catch (System.Exception)
            {
                //txtDataDetailed.Text += ex.ToString();
            }
        }

        /// <summary>
        /// ��ǰѡ�е��豸�����仯
        /// </summary>
        private void OnCurrentNodeSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                treeViewDataSummary.Nodes.Clear();
                txtDataDetailed.Clear();
                _selectedDeviceName = cbxCurrentNodeID.Text;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// ����ժҪ���οؼ�ѡ���¼�
        /// </summary>
        private void OnCommStatusTreeViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (this.chkSyncRefresh.Checked)
                {
                    ShowSelectedNodeAttachedData();
                }
            }
            catch (System.Exception ex)
            {
                txtDataDetailed.Text += ex.ToString();
            }
        }

        private void chkInputStreamVisable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this._IncomingStreamVisable = this.chkInputStreamVisable.Checked;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkOutputStreamVisable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this._OutgoingStreamVisable = this.chkOutputStreamVisable.Checked;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void chkSyncRefresh_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.chkSyncRefresh.Checked)
                {
                    ShowSelectedNodeAttachedData();
                }
                else
                {
                    txtDataDetailed.Clear();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion


        #region "ContexMenu�¼�"

        private void contextMenuAliveData_Opening(object sender, CancelEventArgs e)
        {
            try
            {

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// ȫ��չ��
        /// </summary>
        private void ToolStripMenuItemExpandAll_Click(object sender, EventArgs e)
        {
            try
            {
                treeViewNetwork.ExpandAll();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ȫ���۵�
        /// </summary>
        private void ToolStripMenuItemCollapseAll_Click(object sender, EventArgs e)
        {
            try
            {
                treeViewNetwork.CollapseAll();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ���ժҪ��Ϣ��
        /// </summary>
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.treeViewDataSummary.Nodes.Clear();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// �����־
        /// </summary>
        private void clearlogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.listViewLog.Items.Clear();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
