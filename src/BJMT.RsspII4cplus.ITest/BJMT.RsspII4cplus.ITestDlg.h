
// BJMT.RsspII4cplus.ITestDlg.h : ͷ�ļ�
//

#pragma once


// CBJMTRsspII4cplusITestDlg �Ի���
class CBJMTRsspII4cplusITestDlg : public CDialogEx
{
// ����
public:
	CBJMTRsspII4cplusITestDlg(CWnd* pParent = NULL);	// ��׼���캯��

// �Ի�������
	enum { IDD = IDD_BJMTRSSPII4CPLUSITEST_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV ֧��


// ʵ��
protected:
	HICON m_hIcon;

	// ���ɵ���Ϣӳ�亯��
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
};
