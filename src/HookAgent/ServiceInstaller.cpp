/****************************** Module Header ******************************\
* Module Name:  ServiceInstaller.cpp
* Project:      CppWindowsService
* Copyright (c) Microsoft Corporation.
*
* The file implements functions that install and uninstall the service.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region "Includes"
#include <stdio.h>
#include <windows.h>
#include<atlbase.h>
#include "bootstrap.h"
#include"ServiceInstaller.h"
#include "SmartClientService.h"

#pragma endregion



//
//   FUNCTION: InstallService
//
//   PURPOSE: Install the current application as a service to the local 
//   service control manager database.
//
//   PARAMETERS:
//   * pszServiceName - the name of the service to be installed
//   * pszDisplayName - the display name of the service
//   * dwStartType - the service start option. This parameter can be one of 
//     the following values: SERVICE_AUTO_START, SERVICE_BOOT_START, 
//     SERVICE_DEMAND_START, SERVICE_DISABLED, SERVICE_SYSTEM_START.
//   * pszDependencies - a pointer to a double null-terminated array of null-
//     separated names of services or load ordering groups that the system 
//     must start before this service.
//   * pszAccount - the name of the account under which the service runs.
//   * pszPassword - the password to the account name.
//
//   NOTE: If the function fails to install the service, it prints the error 
//   in the standard output stream for users to diagnose the problem.
//
void InstallService(PWSTR pszServiceName,
	PWSTR pszDisplayName,
	PWSTR serviceDescription,
	DWORD dwStartType,
	PWSTR pszDependencies,
	PWSTR pszAccount,
	PWSTR pszPassword)
{

	//修改IE的跨域权限
	ChangeIECrossdomainData();

	ChangeNoInteractiveServices();

	wchar_t szPath[MAX_PATH];
	SC_HANDLE schSCManager = NULL;
	SC_HANDLE schService = NULL;

	if (GetModuleFileName(NULL, szPath, ARRAYSIZE(szPath)) == 0)
	{
		wprintf(L"GetModuleFileName failed w/err 0x%08lx\n", GetLastError());
		goto Cleanup;
	}

	// Open the local default service control manager databaseSC_MANAGER_ALL_ACCESS); //
	schSCManager = OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);//SC_MANAGER_CONNECT | SC_MANAGER_CREATE_SERVICE);

	if (schSCManager == NULL)
	{
		wprintf(L"OpenSCManager failed w/err 0x%08lx\n", GetLastError());
		goto Cleanup;
	}

	// Install the service into SCM by calling CreateService
	schService = CreateService(
		schSCManager,                   // SCManager database
		pszServiceName,                 // Name of service
		pszDisplayName,                 // Name to display
		SERVICE_QUERY_STATUS,           // Desired access
		SERVICE_WIN32_OWN_PROCESS|SERVICE_INTERACTIVE_PROCESS,      // Service type  SERVICE_INTERACTIVE_PROCESS
		dwStartType,                    // Service start type
		SERVICE_ERROR_NORMAL,           // Error control type
		szPath,                         // Service's binary
		NULL,                           // No load ordering group
		NULL,                           // No tag identifier
		pszDependencies,                // Dependencies
		pszAccount,                     // Service running account
		pszPassword                     // Password of the account
	);
	if (schService == NULL)
	{
		wprintf(L"CreateService failed w/err 0x%08lx\n", GetLastError());
		goto Cleanup;
	}


	//sucess installd ,now change the description
	if (wcslen(serviceDescription) > 0)
	{
		if (UpdateSvcDesc(pszServiceName, serviceDescription) == FALSE) {
			wprintf(L"ChangeServiceConfig2 failed w/err 0x%08lx\n", GetLastError());
		}
	}
	wprintf(L"%s is installed.\n", pszServiceName);


	

Cleanup:
	// Centralized cleanup for all allocated resources.
	if (schSCManager)
	{
		CloseServiceHandle(schSCManager);
		schSCManager = NULL;
	}
	if (schService)
	{
		CloseServiceHandle(schService);
		schService = NULL;
	}
	goto AutoStart;

AutoStart:
	AutoStartServiceBootstrap lanucher{ pszServiceName };
	lanucher.Start();

}



//修改IE的跨域访问权限--注册表修改
void ChangeIECrossdomainData() {
	//安装Windows服务之前 首先修改IE的跨域访问权限注册表
	/*HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\0\1406
	HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\1\1406
	HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\2\1406
	HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\3\1406
	HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\4\1406*/

	//通过域访问数据源:(3＝禁用、0＝启用、1＝提示)"1406"=dword:00000000 ; 
    //跨域浏览窗口和框架:(3＝禁用、0＝启用、1＝提示); IE6:跨域浏览子框架"1607" = dword : 00000000;

	CRegKey myKey{};
	LPCTSTR key = L"1406";
	DWORD value = 0;

	try
	{
	
		//通过域访问数据源
		if (myKey.Open(HKEY_CURRENT_USER, L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\0") == ERROR_SUCCESS)
		{
			myKey.SetDWORDValue(key, value);
			myKey.Close();
		}


		if (myKey.Open(HKEY_CURRENT_USER, L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\1") == ERROR_SUCCESS)
		{
			myKey.SetDWORDValue(key, value);
			myKey.Close();
		}
		if (myKey.Open(HKEY_CURRENT_USER, L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\2") == ERROR_SUCCESS)
		{
			myKey.SetDWORDValue(key, value);
			myKey.Close();
		}

	
		if (myKey.Open(HKEY_CURRENT_USER, L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3") == ERROR_SUCCESS)
		{
			myKey.SetDWORDValue(key, value);
			myKey.Close();
		}
		if (myKey.Open(HKEY_CURRENT_USER, L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\4") == ERROR_SUCCESS)
		{
			myKey.SetDWORDValue(key, value);
			myKey.Close();
		}


		//跨域浏览窗口和框架
		key = L"1607";
		if (myKey.Open(HKEY_CURRENT_USER, L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\0") == ERROR_SUCCESS)
		{
			myKey.SetDWORDValue(key, value);
			myKey.Close();
		}


		if (myKey.Open(HKEY_CURRENT_USER, L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\1") == ERROR_SUCCESS)
		{
			myKey.SetDWORDValue(key, value);
			myKey.Close();
		}
		if (myKey.Open(HKEY_CURRENT_USER, L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\2") == ERROR_SUCCESS)
		{
			myKey.SetDWORDValue(key, value);
			myKey.Close();
		}


		if (myKey.Open(HKEY_CURRENT_USER, L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3") == ERROR_SUCCESS)
		{
			myKey.SetDWORDValue(key, value);
			myKey.Close();
		}
		if (myKey.Open(HKEY_CURRENT_USER, L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\4") == ERROR_SUCCESS)
		{
			myKey.SetDWORDValue(key, value);
			myKey.Close();
		}

	}
	catch (...)
	{

	}

	myKey.Flush();

}
/*
*修改系统注册表，允许交互式的 服务运行
Win8以上的版本默认是1  需要改回0
*/
void ChangeNoInteractiveServices() {

	CRegKey myKey{};
	LPCTSTR key = L"NoInteractiveServices";
	DWORD value = 0;

	try
	{

		if (myKey.Open(HKEY_LOCAL_MACHINE, L"SYSTEM\\CurrentControlSet\\Control\\Windows") == ERROR_SUCCESS)
		{
			myKey.SetDWORDValue(key, value);
			myKey.Close();
		}


	}
	catch (...)
	{

	}

	myKey.Flush();

}



//update the service description 
BOOL  UpdateSvcDesc(PWSTR pszServiceName, PWSTR descriptioText) {

	BOOL result = FALSE;
	SC_HANDLE schSCManager;
	SC_HANDLE schService;
	SERVICE_DESCRIPTION sd;
	//LPTSTR szDesc = TEXT(pszServiceName);

	// Get a handle to the SCM database. 

	schSCManager = OpenSCManager(
		NULL,                    // local computer
		NULL,                    // ServicesActive database 
		SC_MANAGER_CONNECT | SC_MANAGER_CREATE_SERVICE);  // SC_MANAGER_ALL_ACCESSfull access rights 

	if (NULL == schSCManager)
	{
		wprintf(L"OpenSCManager failed (%d)\n", GetLastError());
		return result;
	}

	// Get a handle to the service.

	schService = OpenService(
		schSCManager,            // SCM database 
		pszServiceName,               // name of service 
		SERVICE_CHANGE_CONFIG);  // need change config access 

	if (schService == NULL)
	{
		wprintf(L"OpenService failed (%d)\n", GetLastError());
		CloseServiceHandle(schSCManager);
		return result;
	}

	// Change the service description.

	sd.lpDescription = descriptioText;

	if (!ChangeServiceConfig2(
		schService,                 // handle to service
		SERVICE_CONFIG_DESCRIPTION, // change: description
		&sd))                      // new description
	{
		wprintf(L"ChangeServiceConfig2 failed\n");
	}
	else wprintf(

		L"Service description updated successfully.\n"
	);
	result = TRUE;

	CloseServiceHandle(schService);
	CloseServiceHandle(schSCManager);

	return result;
}



//
//   FUNCTION: UninstallService
//
//   PURPOSE: Stop and remove the service from the local service control 
//   manager database.
//
//   PARAMETERS: 
//   * pszServiceName - the name of the service to be removed.
//
//   NOTE: If the function fails to uninstall the service, it prints the 
//   error in the standard output stream for users to diagnose the problem.
//
void UninstallService(PWSTR pszServiceName)
{
	SC_HANDLE schSCManager = NULL;
	SC_HANDLE schService = NULL;
	SERVICE_STATUS ssSvcStatus = {};


	// Open the local default service control manager database
	schSCManager = OpenSCManager(NULL, NULL, SC_MANAGER_CONNECT);
	if (schSCManager == NULL)
	{
		wprintf(L"OpenSCManager failed w/err 0x%08lx\n", GetLastError());
		goto Cleanup;
	}

	// Open the service with delete, stop, and query status permissions
	schService = OpenService(schSCManager, pszServiceName, SERVICE_STOP |
		SERVICE_QUERY_STATUS | DELETE);
	if (schService == NULL)
	{
		wprintf(L"OpenService failed w/err 0x%08lx\n", GetLastError());
		goto Cleanup;
	}

	// Try to stop the service
	if (ControlService(schService, SERVICE_CONTROL_STOP, &ssSvcStatus))
	{
		wprintf(L"Stopping %s.", pszServiceName);
		Sleep(1000);

		while (QueryServiceStatus(schService, &ssSvcStatus))
		{
			if (ssSvcStatus.dwCurrentState == SERVICE_STOP_PENDING)
			{
				wprintf(L".");
				Sleep(1000);
			}
			else break;
		}

		if (ssSvcStatus.dwCurrentState == SERVICE_STOPPED)
		{
			wprintf(L"\n%s is stopped.\n", pszServiceName);
		}
		else
		{
			wprintf(L"\n%s failed to stop.\n", pszServiceName);
		}
	}

	// Now remove the service by calling DeleteService.
	if (!DeleteService(schService))
	{
		wprintf(L"DeleteService failed w/err 0x%08lx\n", GetLastError());
		goto Cleanup;
	}


	


	wprintf(L"%s is removed.\n", pszServiceName);

Cleanup:
	// Centralized cleanup for all allocated resources.
	if (schSCManager)
	{
		CloseServiceHandle(schSCManager);
		schSCManager = NULL;
	}
	if (schService)
	{
		CloseServiceHandle(schService);
		schService = NULL;
	}
}
