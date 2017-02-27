using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using BJMT.Log.Presentation;
using BJMT.RsspII4net.Controls;
using BJMT.RsspII4net.Events;
using BJMT.RsspII4net.ITest.Infrastructure;
using BJMT.RsspII4net.ITest.Utilities;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.ITest.Presentation
{
    partial class MainForm : Form
    {
        #region "Field"
        /// <summary>
        /// Comm�ӿ�������
        /// </summary>
        private CommFactory _commFactory;
        /// <summary>
        /// ͨѶ�ӿ�
        /// </summary>
        private IRsspNode _rsspNodeComm = null;

        /// <summary>
        /// ����ҳ��
        /// </summary>
        private ICommConfigProvider _settingPage;

        /// <summary>
        /// ͨѶ�ӿ�״̬��ʾ�ؼ�
        /// </summary>
        private RsspMonitorControl _commMonitorControl = new RsspMonitorControl() { Dock = DockStyle.Fill };
        /// <summary>
        /// ���ݷ��Ϳؼ�
        /// </summary>
        private DataSendingControl _dataSendingControl = new DataSendingControl() { Dock = DockStyle.Fill };

        private CommSnapshotUserControl _snapshotControl = new CommSnapshotUserControl() { Dock = DockStyle.Fill };

        private FileManager _fileManager = new FileManager();
        #endregion

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="tcpClient">true��ʾ��ʾClient���� ��������ʾServer����</param>
        public MainForm(bool tcpClient)
        {
            InitializeComponent();
            this.CreateGraphics();

            // ��������ӿؼ�
            CreateSubControls(tcpClient);

            // ���ñ���
            this.UpdateCaptital();
        }

        #region "�ؼ��¼�"
                         
        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (_rsspNodeComm != null)
                {
                    throw new ApplicationException("ͨ�Žӿ��Ѿ����ڴ�״̬��");
                }    
                
                // Create
                _rsspNodeComm = _commFactory.Create();
                _rsspNodeComm.LogCreated += OnRsspCommLogCreated;
                _rsspNodeComm.UserDataIncoming += this.OnUserDataIncoming;

                _commMonitorControl.ClearCommHandler();
                _commMonitorControl.AddCommHandler(_rsspNodeComm);

                // Open
                _rsspNodeComm.Open();

                // �������ݷ��Ϳؼ�
                _dataSendingControl.NodeComm = _rsspNodeComm;
                _snapshotControl.CommNode = _rsspNodeComm;

                // ���½���
                _settingPage.UpdateControl(true);

                // 
                btnOpen.Enabled = false;
            }
            catch (System.Exception ex)
            {
                _settingPage.UpdateControl(false);
                btnOpen.Enabled = true;

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                CloseResource();

                // ���½���
                _settingPage.UpdateControl(false);
                btnOpen.Enabled = true;
            }
            catch (System.Exception /*ex*/)
            {
                btnOpen.Enabled = true;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                _dataSendingControl.StartSending();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        /// <summary>
        /// �رմ���ʱ
        /// </summary>
        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseResource();
        }

        #endregion

        #region "private methods"

        private void CreateSubControls(bool isTcpClient)
        {
            // ����ҳ���빤��
            if (isTcpClient)
            {
                _settingPage = new ClientConfigControl();
                _commFactory = new ClientCommFactory(_settingPage as IClientConfigProvider);
            }
            else
            {
                _settingPage = new ServerConfigControl();
                _commFactory = new ServerCommFactory(_settingPage as IServerConfigProvider);
            }
            _settingPage.LocalNodeIdChanged += this.OnLocalNodeIdChanged;
            this.AddTabPage("����", _settingPage.View);


            // ����״̬�ؼ���������ز���
            _commMonitorControl.CreateGraphics();
            _commMonitorControl.Dock = DockStyle.Fill;
            this.AddTabPage("״̬", _commMonitorControl);
            //this.tabControlMain.ImageList = _commMonitorControl.ImageList;
            //this.tabControlMain.TabPages.AddRange(_commMonitorControl.TabPages);

            // �������ݷ��Ϳؼ�
            this.AddTabPage("���ݷ���", _dataSendingControl);

            // ������־�ؼ�
            this.AddTabPage("��־", new LogControlMultiPages() { Dock = DockStyle.Fill });
            
            // ����
            this.AddTabPage("����", _snapshotControl);
        }

        private void AddTabPage(string text, Control control)
        {
            var newTabPage = new TabPage(text);
            newTabPage.Controls.Add(control);
            this.tabControlMain.TabPages.Add(newTabPage);
        }

        private void OnLocalNodeIdChanged(object sender, EventArgs args)
        {
            try
            {
                UpdateCaptital();
            }
            catch (Exception ex)
            {
                LogUtility.Error(ex);
            }
        }

        private void UpdateCaptital()
        {
            this.Text = String.Format("RSSP-IIͨ��������� v{0}��{1}��",
                Assembly.GetExecutingAssembly().GetName().Version, _settingPage.Title);
        }
        
        private void CloseResource()
        {
            try
            {
                _dataSendingControl.StopSending();

                if (_rsspNodeComm != null)
                {
                    _rsspNodeComm.Dispose();
                    _rsspNodeComm.LogCreated -= OnRsspCommLogCreated;
                    _rsspNodeComm.UserDataIncoming -= this.OnUserDataIncoming;
                }

                _commMonitorControl.ClearCommHandler();

                _fileManager.Clear();
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex);
            }
            finally
            {
                _rsspNodeComm = null;
            }
        }
        #endregion


        #region "IRsspNode�¼�������"

        private void OnRsspCommLogCreated(object sender, LogCreatedEventArgs e)
        {
            if (e.IsInfo)
            {
                LogUtility.Info(e.Message);
            }
            else if (e.IsWarning)
            {
                LogUtility.Warning(e.Message);
            }
            else
            {
                LogUtility.Error(e.Message);
            }
        }

        /// <summary>
        /// �����յ����û�����
        /// </summary>
        private void OnUserDataIncoming(object sender, UserDataIncomingEventArgs args)
        {
            try
            {
                // ����
                if (_settingPage.UserDataStorageEnabled)
                {
                    _fileManager.SaveUserData(args.Package.RemoteID, args.Package.UserData);
                }
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex);
            }
        }
        #endregion
    }
}