
// BJMT.RsspII4cplus.ITest.h : PROJECT_NAME Ӧ�ó������ͷ�ļ�
//

#pragma once

#ifndef __AFXWIN_H__
	#error "�ڰ������ļ�֮ǰ������stdafx.h�������� PCH �ļ�"
#endif

#include "resource.h"		// ������


// CBJMTRsspII4cplusITestApp:
// �йش����ʵ�֣������ BJMT.RsspII4cplus.ITest.cpp
//

class CBJMTRsspII4cplusITestApp : public CWinApp
{
public:
	CBJMTRsspII4cplusITestApp();

// ��д
public:
	virtual BOOL InitInstance();

// ʵ��

	DECLARE_MESSAGE_MAP()
};

extern CBJMTRsspII4cplusITestApp theApp;