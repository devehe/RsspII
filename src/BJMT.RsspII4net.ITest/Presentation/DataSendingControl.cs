using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BJMT.RsspII4net.ITest.Utilities;
using BJMT.RsspII4net.Events;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.ITest.Presentation
{
    partial class DataSendingControl : UserControl
    {
        #region Filed
        /// <summary>
        /// 
        /// </summary>
        private IRsspNode _nodeComm;

        /// <summary>
        /// ��Ҫ���͵��ļ�·����
        /// </summary>
        private string _fileNameToSend = "";
        /// <summary>
        /// ���ͼ��
        /// </summary>
        private int _sendInterval = 20;
        /// <summary>
        /// ���ʹ���
        /// </summary>
        private int _sendTimes = 1;
        /// <summary>
        /// ���ݰ���С
        /// </summary>
        private int _packetSize = 50;

        /// <summary>
        /// �����Զ�������ʱ�Ƿ����ļ�ʼĩ��־
        /// </summary>
        private bool _fileBeginFlagEnabled = false;
        /// <summary>
        /// �����¼������ڷ�������ʱΪ���ź�״̬
        /// </summary>
        private ManualResetEvent _sendExitEvent = new ManualResetEvent(false);
        /// <summary>
        /// �����߳�
        /// </summary>
        private Thread _sendThread = null;
        #endregion

        #region "Properties"
        public IRsspNode NodeComm
        {
            get { return _nodeComm; }

            set
            {
                if (_nodeComm != null)
                {
                    _nodeComm.NodeConnected -= this.OnNodeConnected;
                    _nodeComm.NodeDisconnected -= this.OnNodeInterruption;
                }

                _nodeComm = value;

                if (_nodeComm != null)
                {
                    _nodeComm.NodeConnected += this.OnNodeConnected;
                    _nodeComm.NodeDisconnected += this.OnNodeInterruption;
                }
            }
        }
        #endregion

        public DataSendingControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.txtUIData.Dock = DockStyle.Fill;

            this.chkFileBeginFlag.Enabled = this.rdoCustomFile.Checked;
            _fileBeginFlagEnabled = this.chkFileBeginFlag.Checked;
            this.txtUIData.Enabled = this.rdoUIData.Checked;
        }

        #region "public methods"
        /// <summary>
        /// �������ÿ�ʼ��������
        /// </summary>
        public void StartSending()
        {
            if (_nodeComm == null)
            {
                throw new InvalidOperationException("û�п��õ�ͨѶ�ӿ�");
            }

            if (_sendThread != null)
            {
                throw new InvalidOperationException("���ڷ������ݣ����Ժ�����...");
            }

            if (clbDests.CheckedItems.Count == 0)
            {
                throw new InvalidOperationException("��ѡ��һ��Ŀ�ĵ�");
            }

            // update
            _fileNameToSend = txtTestFile.Text;
            _packetSize = (int)nudPacketSize.Value;
            _sendTimes = (int)ctrlRepeatNum.Value;
            _sendInterval = Convert.ToInt32(nudInterval.Value);

            // ���������߳�
            _sendExitEvent.Reset();
            if (this.rdoCustomFile.Checked)
            {
                if (txtTestFile.Text == "")
                {
                    throw new InvalidOperationException("��ѡ��һ���ļ���");
                }
                _sendThread = new Thread(new ThreadStart(SendCustomFile));
                _sendThread.IsBackground = true;
                _sendThread.Start();
            }
            else
            {
                var userData = this.GetUserData();
                _sendThread = new Thread(new ParameterizedThreadStart(SendUIData));
                _sendThread.IsBackground = true;
                _sendThread.Start(userData);
            }
        }
        /// <summary>
        /// ֹͣ��������
        /// </summary>
        public void StopSending()
        {
            try
            {
                if (_sendThread != null)
                {
                    _sendExitEvent.Set();
                    _sendThread.Join(1000);
                    _sendThread = null;
                }
            }
            catch (System.Exception /*ex*/)
            {
            }
        }
        #endregion


        #region "private methods"
        private List<uint> GetDest()
        {
            var result = new List<uint>();

            foreach (var item in clbDests.CheckedItems)
            {
                result.Add(uint.Parse(item.ToString()));
            }

            return result;
        }

        private byte[] GetUserData()
        {
            if (txtUIData.Text.Length == 0) return new byte[0];

            if (chkAsciiData.Checked)
            {
                return Encoding.GetEncoding("gb2312").GetBytes(txtUIData.Text);
            }
            else
            {
                return HelperTools.SplitHexText(txtUIData.Text);
            }
        }

        private void SendCustomFile()
        {
            FileStream fs = null;
            BinaryReader br = null;
            Byte[] userData = null;
            var dest = this.GetDest();

            try
            {
                // Create the reader for data.
                fs = new FileStream(_fileNameToSend, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);

                for (int n = 0; n < _sendTimes; n++)
                {
                    try
                    {
                        // �����ļ���ʼ��־����չ��
                        if (_fileBeginFlagEnabled)
                        {
                            userData = RsspEncoding.ToNetString(CustomFile.StartFlag + Path.GetExtension(_fileNameToSend));
                            
                            var outPkg = new OutgoingPackage(dest, userData);
                            _nodeComm.Send(outPkg);
                        }

                        // Read and send data
                        fs.Position = 0;
                        do
                        {
                            userData = br.ReadBytes(_packetSize);

                            var outPkg = new OutgoingPackage(dest, userData);
                            _nodeComm.Send(outPkg);

                            // ����ļ�������ɣ�����������
                            if (userData.Length < _packetSize)
                            {
                                break;
                            }

                            // ���ͼ��
                            if (_sendExitEvent.WaitOne(_sendInterval, false))
                            {
                                break;
                            }
                        } while (fs.Length != fs.Position);
                    }
                    finally
                    {
                        userData = RsspEncoding.ToNetString(CustomFile.EndFlag);

                        // �����ļ����ͽ�����־
                        if (_fileBeginFlagEnabled)
                        {
                            var outPkg = new OutgoingPackage(dest, userData);
                            _nodeComm.Send(outPkg);
                        }
                    }
                }
            }
            catch (Exception /*ex*/)
            {
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }

                _sendThread = null;
            }
        }
        
        private void SendUIData(object state)
        {
            try
            {
                var userData = state as byte[];
                var dest = this.GetDest();

                for (int n = 0; n < _sendTimes; n++)
                {
                    var outPkg = new OutgoingPackage(dest, userData);
                    _nodeComm.Send(outPkg);
                    
                    // ���ͼ��
                    if (_sendExitEvent.WaitOne(_sendInterval, false)) break;
                }
            }
            catch (System.Exception /*ex*/)
            {
            }
            finally
            {
                _sendThread = null;
            }
        }


        #endregion


        #region "Control event"
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            DialogResult dr = openTestFileDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtTestFile.Text = openTestFileDialog.FileName;
            }
        }

        private void nudInterval_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                _sendInterval = Convert.ToInt32(nudInterval.Value);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void nudPacketSize_ValueChanged(object sender, EventArgs e)
        {
            _packetSize = (int)nudPacketSize.Value;
        }

        private void ctrlRepeatNum_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                _sendTimes = (int)ctrlRepeatNum.Value;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rdoCustomFile_CheckedChanged(object sender, EventArgs e)
        {
            this.chkFileBeginFlag.Enabled = this.rdoCustomFile.Checked;
            this.nudPacketSize.Enabled = this.rdoCustomFile.Checked;
            this.txtUIData.Enabled = this.rdoUIData.Checked;
        }

        private void rdoUIData_CheckedChanged(object sender, EventArgs e)
        {
            this.chkHexData.Enabled = this.rdoUIData.Checked;
            this.chkAsciiData.Enabled = this.rdoUIData.Checked;
        }

        private void chkHexData_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                chkAsciiData.Checked = !chkHexData.Checked;
            }
            catch (System.Exception /*ex*/)
            {

            }
        }

        private void chkAsciiData_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                chkHexData.Checked = !chkAsciiData.Checked;
            }
            catch (System.Exception /*ex*/)
            {

            }
        }

        private void chkFileBeginFlag_CheckedChanged(object sender, EventArgs e)
        {
            _fileBeginFlagEnabled = this.chkFileBeginFlag.Checked;
        }
        #endregion


        #region "ͨѶ�ӿ��¼�������"
        
        private void OnNodeConnected(object sender, NodeConnectedEventArgs args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    clbDests.Items.Add(args.RemoteID);

                    if (clbDests.Items.Count == 1)
                    {
                        clbDests.SetItemCheckState(0, CheckState.Checked);
                    }
                }));
            }
            catch(Exception ex)
            {
                LogUtility.Error(ex);
            }
        }


        private void OnNodeInterruption(object sender, NodeInterruptionEventArgs args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    clbDests.Items.Remove(args.RemoteID);
                }));
            }
            catch (Exception ex)
            {
                LogUtility.Error(ex);
            }
        }

        #endregion
    }
}
