
// BJMT.RsspII4net.Interop4cpp.ITest.h : PROJECT_NAME Ӧ�ó������ͷ�ļ�
//

#pragma once

#ifndef __AFXWIN_H__
	#error "�ڰ������ļ�֮ǰ������stdafx.h�������� PCH �ļ�"
#endif

#include "resource.h"		// ������


// CBJMTRsspII4netInterop4cppITestApp:
// �йش����ʵ�֣������ BJMT.RsspII4net.Interop4cpp.ITest.cpp
//

class CBJMTRsspII4netInterop4cppITestApp : public CWinApp
{
public:
	CBJMTRsspII4netInterop4cppITestApp();

// ��д
public:
	virtual BOOL InitInstance();

// ʵ��

	DECLARE_MESSAGE_MAP()
};

extern CBJMTRsspII4netInterop4cppITestApp theApp;