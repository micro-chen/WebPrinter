/****************************** Module Header ******************************\
* Module Name:  SampleService.cpp
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

#pragma region Includes
#include "SmartClientService.h"
#include "ThreadPool.h"


#pragma endregion

SmartClientService::SmartClientService(PWSTR pszServiceName,
	BOOL fCanStop,
	BOOL fCanShutdown,
	BOOL fCanPauseContinue)
	: CServiceBase(pszServiceName, fCanStop, fCanShutdown, fCanPauseContinue)
{
	m_fStopping = FALSE;

	// Create a manual-reset event that is not signaled at first to indicate 
	// the stopped signal of the service.
	m_hStoppedEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
	if (m_hStoppedEvent == NULL)
	{
		throw GetLastError();
	}

	this->IsHasRunningApplication = FALSE;
}


SmartClientService::~SmartClientService(void)
{
	if (m_hStoppedEvent)
	{
		CloseHandle(m_hStoppedEvent);
		m_hStoppedEvent = NULL;
	}
}


//
//   FUNCTION: SmartClientService::OnStart(DWORD, LPWSTR *)
//
//   PURPOSE: The function is executed when a Start command is sent to the 
//   service by the SCM or when the operating system starts (for a service 
//   that starts automatically). It specifies actions to take when the 
//   service starts. In this code sample, OnStart logs a service-start 
//   message to the Application log, and queues the main service function for 
//   execution in a thread pool worker thread.
//
//   PARAMETERS:
//   * dwArgc   - number of command line arguments
//   * lpszArgv - array of command line arguments
//
//   NOTE: A service application is designed to be long running. Therefore, 
//   it usually polls or monitors something in the system. The monitoring is 
//   set up in the OnStart method. However, OnStart does not actually do the 
//   monitoring. The OnStart method must return to the operating system after 
//   the service's operation has begun. It must not loop forever or block. To 
//   set up a simple monitoring mechanism, one general solution is to create 
//   a timer in OnStart. The timer would then raise events in your code 
//   periodically, at which time your service could do its monitoring. The 
//   other solution is to spawn a new thread to perform the main service 
//   functions, which is demonstrated in this code sample.
//
void SmartClientService::OnStart(DWORD dwArgc, LPWSTR *lpszArgv)
{
	// Log a service start message to the Application log.
	WriteEventLogEntry(L"CppWindowsService in OnStart",
		EVENTLOG_INFORMATION_TYPE);

	// Queue the main service function for execution in a worker thread.
	CThreadPool::QueueUserWorkItem(&SmartClientService::ServiceWorkerThread, this);
}




/*调用SDK运行时*/
void SmartClientService::HookTheRunTime(void) {

	//0 检测是否安装了 .net framework 4.0
	//1将IE 的跨域访问数据注册表修改
	//2 启动进程
	BOOL hasInstalledNetFramework = Process::isHasInstalledNetFramework();


	if (Process::isProcessRunning(this->Application_NAME.c_str()))
	{
		//this->AppProcessHanlde = nullptr;
		return;
	}



	//获取当前程序的执行目录-
	wchar_t bufferOfCrurrentServiceBinPathW[_MAX_PATH];

	GetModuleFileName(NULL, bufferOfCrurrentServiceBinPathW, ARRAYSIZE(bufferOfCrurrentServiceBinPathW));


	//截取当前bin路径 获取文件夹
	wstring binPathStr(bufferOfCrurrentServiceBinPathW);
	const size_t last_slash_idx = binPathStr.rfind('\\');
	wstring dirCurr = binPathStr.substr(0, last_slash_idx);
	//开启新的进程
	wstring cmd = dirCurr + L"\\app\\SmartClient.ServerHost.exe";


	//cmd = dirCurr + L"\\app\\SmartClient.ServerHost.exe";
	//if (!hasInstalledNetFramework)
	//{
	//	//未安装.net framework  那么使用mono 
	//	cmd=dirCurr + L"\\MonoVM.exe --runtime=v4.0 \"" + dirCurr + L"\\app\\SmartClient.ServerHost.exe\"";
	//}
	//else
	//{
		//直接启动程序
	//cmd = dirCurr + L"\\app\\SmartClient.ServerHost.exe";
	//}

	try
	{
		WriteEventLogEntry(const_cast<wchar_t*>(cmd.c_str()),
			EVENTLOG_INFORMATION_TYPE);

		Process processOfNewInstance(cmd);

		this->AppProcessHanlde = &processOfNewInstance;
		//开启菜鸟打印组件的进程



	}
	catch (const std::exception&)
	{
		//throw GetLastError();

	}


	//system("c:\\Mono\\bin\\MonoVM.exe c:\\Mono\\bin\\app\\SmartClientServerHost.exe");

}


int SmartClientService::KillApplicationProc() {
	int code = -1;
	try
	{
		////强制杀死进程

		Process::KILL_PROC_BY_NAME(this->Application_NAME.c_str());

		code = 1;
	}
	catch (...)
	{

	}

	//if (nullptr != this->AppProcessHanlde)
	//{
	//	try
	//	{
	//		////强制杀死进程

	//		Process::kill(this->AppProcessHanlde->get_id(), true);

	//		code = 1;
	//	}
	//	catch (...)
	//	{

	//	}

	//}
	//else
	//{
	//	code = 1;
	//}
	return code;

}

//
//   FUNCTION: SmartClientService::ServiceWorkerThread(void)
//
//   PURPOSE: The method performs the main function of the service. It runs 
//   on a thread pool worker thread.
//
void SmartClientService::ServiceWorkerThread(void)
{



	// Periodically check if the service is stopping.
	while (!m_fStopping)
	{


		//检测进程是否存在
		IsHasRunningApplication = Process::isProcessRunning(this->Application_NAME.c_str());

		// Perform main service function here...
		if (!IsHasRunningApplication)
		{


			try
			{
				this->HookTheRunTime();

				IsHasRunningApplication = TRUE;
			}
			catch (...)
			{
				IsHasRunningApplication = FALSE;
			}
		}

		::Sleep(2000);  // Simulate some lengthy operations.
	}

	// Signal the stopped event.
	SetEvent(m_hStoppedEvent);
}




//
//   FUNCTION: SmartClientService::OnStop(void)
//
//   PURPOSE: The function is executed when a Stop command is sent to the 
//   service by SCM. It specifies actions to take when a service stops 
//   running. In this code sample, OnStop logs a service-stop message to the 
//   Application log, and waits for the finish of the main service function.
//
//   COMMENTS:
//   Be sure to periodically call ReportServiceStatus() with 
//   SERVICE_STOP_PENDING if the procedure is going to take long time. 
//
void SmartClientService::OnStop()
{
	// Log a service stop message to the Application log.
	WriteEventLogEntry(L"CppWindowsService in OnStop",
		EVENTLOG_INFORMATION_TYPE);


	//服务停止对 时候 ，杀死对应的进程
	int result = this->KillApplicationProc();
	if (result <= 0)
	{
		throw GetLastError();
	}
	// Indicate that the service is stopping and wait for the finish of the 
	// main service function (ServiceWorkerThread).
	m_fStopping = TRUE;
	if (WaitForSingleObject(m_hStoppedEvent, INFINITE) != WAIT_OBJECT_0)
	{
		throw GetLastError();
	}
}