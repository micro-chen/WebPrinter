/****************************** Module Header ******************************\
* Module Name:  SampleService.h
* Project:      CppWindowsService
* Copyright (c) Microsoft Corporation.
* 
* Provides a sample service class that derives from the service base class - 
* CServiceBase. The sample service logs the service start and stop 
* information to the Application event log, and shows how to run the main 
* function of the service in a thread pool worker thread.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

#include "ServiceBase.h"
#include "process.h"

class SmartClientService : public CServiceBase
{

public:

    SmartClientService(PWSTR pszServiceName, 
        BOOL fCanStop = TRUE, 
        BOOL fCanShutdown = TRUE, 
        BOOL fCanPauseContinue = FALSE);
    virtual ~SmartClientService(void);

protected:

    virtual void OnStart(DWORD dwArgc, PWSTR *pszArgv);
    virtual void OnStop();

    void ServiceWorkerThread(void);
	void HookTheRunTime(void);
	int KillApplicationProc();
	//初始化运行时环境
	BOOL InitRunTime();
private:

	//如果安装了.net framework4+  那么直接运行程序
     wstring Application_NAME=L"SmartClient.ServerHost.exe";
	//启动程序的进程
	Process* AppProcessHanlde=nullptr;
    BOOL m_fStopping;
    HANDLE m_hStoppedEvent;
	BOOL IsHasRunningApplication;
	
};