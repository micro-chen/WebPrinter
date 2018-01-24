#include "process.h"
#include <windows.h>
#include <cstring>
#include <TlHelp32.h>
#include <stdexcept>
#include "tchar.h"

#include "detectFX.h"

Process::Data::Data(): id(0), handle(NULL) {}

namespace {
  // Simple HANDLE wrapper to close it automatically from the destructor.
  class Handle {
  public:
    Handle() : handle(INVALID_HANDLE_VALUE) { }
    ~Handle() {
      close();
    }
    void close() {
      if (handle != INVALID_HANDLE_VALUE)
        ::CloseHandle(handle);
    }
    HANDLE detach() {
      HANDLE old_handle = handle;
      handle = INVALID_HANDLE_VALUE;
      return old_handle;
    }
    operator HANDLE() const { return handle; }
    HANDLE* operator&() { return &handle; }
  private:
    HANDLE handle;
  };
  
  //Based on the discussion thread: https://www.reddit.com/r/cpp/comments/3vpjqg/a_new_platform_independent_process_library_for_c11/cxq1wsj
  std::mutex create_process_mutex;
}

//Based on the example at https://msdn.microsoft.com/en-us/library/windows/desktop/ms682499(v=vs.85).aspx.
Process::id_type Process::open(const string_type &command, const string_type &path) {
  if(open_stdin)
    stdin_fd=std::unique_ptr<fd_type>(new fd_type(NULL));
  if(read_stdout)
    stdout_fd=std::unique_ptr<fd_type>(new fd_type(NULL));
  if(read_stderr)
    stderr_fd=std::unique_ptr<fd_type>(new fd_type(NULL));

  Handle stdin_rd_p;
  Handle stdin_wr_p;
  Handle stdout_rd_p;
  Handle stdout_wr_p;
  Handle stderr_rd_p;
  Handle stderr_wr_p;

  SECURITY_ATTRIBUTES security_attributes;

  security_attributes.nLength = sizeof(SECURITY_ATTRIBUTES);
  security_attributes.bInheritHandle = TRUE;
  security_attributes.lpSecurityDescriptor = nullptr;

  std::lock_guard<std::mutex> lock(create_process_mutex);
  if(stdin_fd) {
    if (!CreatePipe(&stdin_rd_p, &stdin_wr_p, &security_attributes, 0) ||
        !SetHandleInformation(stdin_wr_p, HANDLE_FLAG_INHERIT, 0))
      return 0;
  }
  if(stdout_fd) {
    if (!CreatePipe(&stdout_rd_p, &stdout_wr_p, &security_attributes, 0) ||
        !SetHandleInformation(stdout_rd_p, HANDLE_FLAG_INHERIT, 0)) {
      return 0;
    }
  }
  if(stderr_fd) {
    if (!CreatePipe(&stderr_rd_p, &stderr_wr_p, &security_attributes, 0) ||
        !SetHandleInformation(stderr_rd_p, HANDLE_FLAG_INHERIT, 0)) {
      return 0;
    }
  }

  PROCESS_INFORMATION process_info;
  STARTUPINFO startup_info;

  ZeroMemory(&process_info, sizeof(PROCESS_INFORMATION));

  ZeroMemory(&startup_info, sizeof(STARTUPINFO));
  startup_info.cb = sizeof(STARTUPINFO);
  startup_info.hStdInput = stdin_rd_p;
  startup_info.hStdOutput = stdout_wr_p;
  startup_info.hStdError = stderr_wr_p;
  if(stdin_fd || stdout_fd || stderr_fd)
    startup_info.dwFlags |= STARTF_USESTDHANDLES;

  string_type process_command=command;
#ifdef MSYS_PROCESS_USE_SH
  size_t pos=0;
  while((pos=process_command.find('\\', pos))!=string_type::npos) {
    process_command.replace(pos, 1, "\\\\\\\\");
    pos+=4;
  }
  pos=0;
  while((pos=process_command.find('\"', pos))!=string_type::npos) {
    process_command.replace(pos, 1, "\\\"");
    pos+=2;
  }
  process_command.insert(0, "sh -c \"");
  process_command+="\"";
#endif

  //BOOL bSuccess = CreateProcess(nullptr, process_command.empty()?nullptr:&process_command[0], nullptr, nullptr, TRUE, 0,
  //                              nullptr, path.empty()?nullptr:path.c_str(), &startup_info, &process_info);

  //上面的是执行 Normal级别，下面改正为执行时候 不显示窗口的级别
  BOOL bSuccess = CreateProcess(nullptr, 
	  process_command.empty() ? nullptr : &process_command[0],
	  nullptr,
	  nullptr,
	  TRUE,
	  CREATE_NO_WINDOW,
	  nullptr, path.empty() ? nullptr : path.c_str(), &startup_info, &process_info);

  if(!bSuccess) {
    CloseHandle(process_info.hProcess);
    CloseHandle(process_info.hThread);
    return 0;
  }
  else {
    CloseHandle(process_info.hThread);
  }

  if(stdin_fd) *stdin_fd=stdin_wr_p.detach();
  if(stdout_fd) *stdout_fd=stdout_rd_p.detach();
  if(stderr_fd) *stderr_fd=stderr_rd_p.detach();

  closed=false;
  data.id=process_info.dwProcessId;
  data.handle=process_info.hProcess;
  return process_info.dwProcessId;
}

void Process::async_read() {
  if(data.id==0)
    return;
  if(stdout_fd) {
    stdout_thread=std::thread([this](){
      DWORD n;
      std::unique_ptr<char[]> buffer(new char[buffer_size]);
      for (;;) {
        BOOL bSuccess = ReadFile(*stdout_fd, static_cast<CHAR*>(buffer.get()), static_cast<DWORD>(buffer_size), &n, nullptr);
        if(!bSuccess || n == 0)
          break;
        read_stdout(buffer.get(), static_cast<size_t>(n));
      }
    });
  }
  if(stderr_fd) {
    stderr_thread=std::thread([this](){
      DWORD n;
      std::unique_ptr<char[]> buffer(new char[buffer_size]);
      for (;;) {
        BOOL bSuccess = ReadFile(*stderr_fd, static_cast<CHAR*>(buffer.get()), static_cast<DWORD>(buffer_size), &n, nullptr);
        if(!bSuccess || n == 0)
          break;
        read_stderr(buffer.get(), static_cast<size_t>(n));
      }
    });
  }
}

int Process::get_exit_status() {
  if(data.id==0)
    return -1;
  DWORD exit_status;
  WaitForSingleObject(data.handle, INFINITE);
  if(!GetExitCodeProcess(data.handle, &exit_status))
    exit_status=-1;
  {
    std::lock_guard<std::mutex> lock(close_mutex);
    CloseHandle(data.handle);
    closed=true;
  }
  close_fds();

  return static_cast<int>(exit_status);
}

void Process::close_fds() {
  if(stdout_thread.joinable())
    stdout_thread.join();
  if(stderr_thread.joinable())
    stderr_thread.join();
  
  if(stdin_fd)
    close_stdin();
  if(stdout_fd) {
    if(*stdout_fd!=NULL) CloseHandle(*stdout_fd);
    stdout_fd.reset();
  }
  if(stderr_fd) {
    if(*stderr_fd!=NULL) CloseHandle(*stderr_fd);
    stderr_fd.reset();
  }
}

bool Process::write(const char *bytes, size_t n) {
  if(!open_stdin)
    throw std::invalid_argument("Can't write to an unopened stdin pipe. Please set open_stdin=true when constructing the process.");

  std::lock_guard<std::mutex> lock(stdin_mutex);
  if(stdin_fd) {
    DWORD written;
    BOOL bSuccess=WriteFile(*stdin_fd, bytes, static_cast<DWORD>(n), &written, nullptr);
    if(!bSuccess || written==0) {
      return false;
    }
    else {
      return true;
    }
  }
  return false;
}

void Process::close_stdin() {
  std::lock_guard<std::mutex> lock(stdin_mutex);
  if(stdin_fd) {
    if(*stdin_fd!=NULL) CloseHandle(*stdin_fd);
    stdin_fd.reset();
  }
}

//Based on http://stackoverflow.com/a/1173396
void Process::kill_tree(bool force) {
  std::lock_guard<std::mutex> lock(close_mutex);
  if(data.id>0 && !closed) {
    HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    if(snapshot) {
      PROCESSENTRY32 process;
      ZeroMemory(&process, sizeof(process));
      process.dwSize = sizeof(process);
      if(Process32First(snapshot, &process)) {
        do {
          if(process.th32ParentProcessID==data.id) {
            HANDLE process_handle = OpenProcess(PROCESS_TERMINATE, FALSE, process.th32ProcessID);
            if(process_handle) {
              TerminateProcess(process_handle, 2);
              CloseHandle(process_handle);
            }
          }
        } while (Process32Next(snapshot, &process));
      }
      CloseHandle(snapshot);
    }
    TerminateProcess(data.handle, 2);
  }
}


/*!
\brief Check if a process is running
\param [in] processName Name of process to check if is running
\returns \c True if the process is running, or \c False if the process is not running
*/
bool Process::isProcessRunning(const wchar_t *processName)
{
	bool exists = false;
	PROCESSENTRY32 entry;
	entry.dwSize = sizeof(PROCESSENTRY32);

	HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);

	if (Process32First(snapshot, &entry))
		while (Process32Next(snapshot, &entry))
			if (!wcsicmp(entry.szExeFile, processName))
				exists = true;

	CloseHandle(snapshot);
	return exists;
}

///是否安装了 .net framework 4.0+
BOOL Process::isHasInstalledNetFramework() {
	BOOL result{ false };
	if (IsNetfx40FullInstalled() || IsNetfx45Installed() || IsNetfx451Installed() || IsNetfx452Installed() || IsNetfx46Installed() || IsNetfx461Installed() || IsNetfx462Installed())
	{
		result = TRUE;
	}
	return result;
}

int  Process::KILL_PROC_BY_NAME(const TCHAR *szToTerminate)
// Created: 6/23/2000  (RK)
// Last modified: 3/10/2002  (RK)
// Please report any problems or bugs to kochhar@physiology.wisc.edu
// The latest version of this routine can be found at:
//     http://www.neurophys.wisc.edu/ravi/software/killproc/
// Terminate the process "szToTerminate" if it is currently running
// This works for Win/95/98/ME and also Win/NT/2000/XP
// The process name is case-insensitive, i.e. "notepad.exe" and "NOTEPAD.EXE"
// will both work (for szToTerminate)
// Return codes are as follows:
//   0   = Process was successfully terminated
//   603 = Process was not currently running
//   604 = No permission to terminate process
//   605 = Unable to load PSAPI.DLL
//   602 = Unable to terminate process for some other reason
//   606 = Unable to identify system type
//   607 = Unsupported OS
//   632 = Invalid process name
//   700 = Unable to get procedure address from PSAPI.DLL
//   701 = Unable to get process list, EnumProcesses failed
//   702 = Unable to load KERNEL32.DLL
//   703 = Unable to get procedure address from KERNEL32.DLL
//   704 = CreateToolhelp32Snapshot failed
// Change history:
//   modified 3/8/2002  - Borland-C compatible if BORLANDC is defined as
//                        suggested by Bob Christensen
//   modified 3/10/2002 - Removed memory leaks as suggested by
//					      Jonathan Richard-Brochu (handles to Proc and Snapshot
//                        were not getting closed properly in some cases)
{
	BOOL bResult, bResultm;
	DWORD aiPID[1000], iCb = 1000, iNumProc, iV2000 = 0;
	DWORD iCbneeded, i, iFound = 0;
	TCHAR szName[MAX_PATH], szToTermUpper[MAX_PATH];
	HANDLE hProc, hSnapShot, hSnapShotm;
	OSVERSIONINFO osvi;
	HINSTANCE hInstLib;
	int iLen, iLenP, indx;
	HMODULE hMod;
	PROCESSENTRY32 procentry;
	MODULEENTRY32 modentry;

	// Transfer Process name into "szToTermUpper" and
	// convert it to upper case
	iLenP = _tcslen(szToTerminate);
	if (iLenP<1 || iLenP>MAX_PATH) return 632;
	for (indx = 0; indx<iLenP; indx++)
		szToTermUpper[indx] = _totupper(szToTerminate[indx]);
	szToTermUpper[iLenP] = 0;

	// PSAPI Function Pointers.
	BOOL(WINAPI *lpfEnumProcesses)(DWORD *, DWORD cb, DWORD *);
	BOOL(WINAPI *lpfEnumProcessModules)(HANDLE, HMODULE *,
		DWORD, LPDWORD);
	DWORD(WINAPI *lpfGetModuleBaseName)(HANDLE, HMODULE,
		LPTSTR, DWORD);

	// ToolHelp Function Pointers.
	HANDLE(WINAPI *lpfCreateToolhelp32Snapshot)(DWORD, DWORD);
	BOOL(WINAPI *lpfProcess32First)(HANDLE, LPPROCESSENTRY32);
	BOOL(WINAPI *lpfProcess32Next)(HANDLE, LPPROCESSENTRY32);
	BOOL(WINAPI *lpfModule32First)(HANDLE, LPMODULEENTRY32);
	BOOL(WINAPI *lpfModule32Next)(HANDLE, LPMODULEENTRY32);

	// First check what version of Windows we're in
	osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
	bResult = GetVersionEx(&osvi);
	if (!bResult)     // Unable to identify system version
		return 606;

	// At Present we only support Win/NT/2000/XP or Win/9x/ME
	if ((osvi.dwPlatformId != VER_PLATFORM_WIN32_NT) &&
		(osvi.dwPlatformId != VER_PLATFORM_WIN32_WINDOWS))
		return 607;

	if (osvi.dwPlatformId == VER_PLATFORM_WIN32_NT)
	{
		// Win/NT or 2000 or XP

		// Load library and get the procedures explicitly. We do
		// this so that we don't have to worry about modules using
		// this code failing to load under Windows 9x, because
		// it can't resolve references to the PSAPI.DLL.
		hInstLib = LoadLibraryW(_T("PSAPI.DLL"));
		if (hInstLib == NULL)
			return 605;

		// Get procedure addresses.
		lpfEnumProcesses = (BOOL(WINAPI *)(DWORD *, DWORD, DWORD*))
			GetProcAddress(hInstLib, "EnumProcesses");
		lpfEnumProcessModules = (BOOL(WINAPI *)(HANDLE, HMODULE *,
			DWORD, LPDWORD)) GetProcAddress(hInstLib,
				"EnumProcessModules");
		lpfGetModuleBaseName = (DWORD(WINAPI *)(HANDLE, HMODULE,
			LPTSTR, DWORD)) GetProcAddress(hInstLib,
				"GetModuleBaseNameW");

		if (lpfEnumProcesses == NULL ||
			lpfEnumProcessModules == NULL ||
			lpfGetModuleBaseName == NULL)
		{
			FreeLibrary(hInstLib);
			return 700;
		}

		bResult = lpfEnumProcesses(aiPID, iCb, &iCbneeded);
		if (!bResult)
		{
			// Unable to get process list, EnumProcesses failed
			FreeLibrary(hInstLib);
			return 701;
		}

		// How many processes are there?
		iNumProc = iCbneeded / sizeof(DWORD);

		// Get and match the name of each process
		for (i = 0; i<iNumProc; i++)
		{
			// Get the (module) name for this process

			_tcscpy(szName, _T("Unknown"));
			// First, get a handle to the process
			hProc = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE,
				aiPID[i]);
			// Now, get the process name
			if (hProc)
			{
				if (lpfEnumProcessModules(hProc, &hMod, sizeof(hMod), &iCbneeded))
				{
					iLen = lpfGetModuleBaseName(hProc, hMod, szName, MAX_PATH);
				}
			}
			CloseHandle(hProc);
			// We will match regardless of lower or upper case
#ifdef BORLANDC
			if (_tcscmp(_tcscmp(szName), szToTermUpper) == 0)
#else
			if (_tcscmp(_tcsupr(szName), szToTermUpper) == 0)
#endif
			{
				// Process found, now terminate it
				iFound = 1;
				// First open for termination
				hProc = OpenProcess(PROCESS_TERMINATE, FALSE, aiPID[i]);
				if (hProc)
				{
					if (TerminateProcess(hProc, 0))
					{
						// process terminated
						CloseHandle(hProc);
						FreeLibrary(hInstLib);
						return 0;
					}
					else
					{
						// Unable to terminate process
						CloseHandle(hProc);
						FreeLibrary(hInstLib);
						return 602;
					}
				}
				else
				{
					// Unable to open process for termination
					FreeLibrary(hInstLib);
					return 604;
				}
			}
		}
	}

	if (osvi.dwPlatformId == VER_PLATFORM_WIN32_WINDOWS)
	{
		// Win/95 or 98 or ME

		hInstLib = LoadLibraryW(_T("Kernel32.DLL"));
		if (hInstLib == NULL)
			return 702;

		// Get procedure addresses.
		// We are linking to these functions of Kernel32
		// explicitly, because otherwise a module using
		// this code would fail to load under Windows NT,
		// which does not have the Toolhelp32
		// functions in the Kernel 32.
		lpfCreateToolhelp32Snapshot =
			(HANDLE(WINAPI *)(DWORD, DWORD))
			GetProcAddress(hInstLib,
				"CreateToolhelp32Snapshot");
		lpfProcess32First =
			(BOOL(WINAPI *)(HANDLE, LPPROCESSENTRY32))
			GetProcAddress(hInstLib, "Process32First");
		lpfProcess32Next =
			(BOOL(WINAPI *)(HANDLE, LPPROCESSENTRY32))
			GetProcAddress(hInstLib, "Process32Next");
		lpfModule32First =
			(BOOL(WINAPI *)(HANDLE, LPMODULEENTRY32))
			GetProcAddress(hInstLib, "Module32First");
		lpfModule32Next =
			(BOOL(WINAPI *)(HANDLE, LPMODULEENTRY32))
			GetProcAddress(hInstLib, "Module32Next");
		if (lpfProcess32Next == NULL ||
			lpfProcess32First == NULL ||
			lpfModule32Next == NULL ||
			lpfModule32First == NULL ||
			lpfCreateToolhelp32Snapshot == NULL)
		{
			FreeLibrary(hInstLib);
			return 703;
		}

		// The Process32.. and Module32.. routines return names in all uppercase

		// Get a handle to a Toolhelp snapshot of all the systems processes.

		hSnapShot = lpfCreateToolhelp32Snapshot(
			TH32CS_SNAPPROCESS, 0);
		if (hSnapShot == INVALID_HANDLE_VALUE)
		{
			FreeLibrary(hInstLib);
			return 704;
		}

		// Get the first process' information.
		procentry.dwSize = sizeof(PROCESSENTRY32);
		bResult = lpfProcess32First(hSnapShot, &procentry);

		// While there are processes, keep looping and checking.
		while (bResult)
		{
			// Get a handle to a Toolhelp snapshot of this process.
			hSnapShotm = lpfCreateToolhelp32Snapshot(
				TH32CS_SNAPMODULE, procentry.th32ProcessID);
			if (hSnapShotm == INVALID_HANDLE_VALUE)
			{
				CloseHandle(hSnapShot);
				FreeLibrary(hInstLib);
				return 704;
			}
			// Get the module list for this process
			modentry.dwSize = sizeof(MODULEENTRY32);
			bResultm = lpfModule32First(hSnapShotm, &modentry);

			// While there are modules, keep looping and checking
			while (bResultm)
			{
				if (_tcscmp(modentry.szModule, szToTermUpper) == 0)
				{
					// Process found, now terminate it
					iFound = 1;
					// First open for termination
					hProc = OpenProcess(PROCESS_TERMINATE, FALSE, procentry.th32ProcessID);
					if (hProc)
					{
						if (TerminateProcess(hProc, 0))
						{
							// process terminated
							CloseHandle(hSnapShotm);
							CloseHandle(hSnapShot);
							CloseHandle(hProc);
							FreeLibrary(hInstLib);
							return 0;
						}
						else
						{
							// Unable to terminate process
							CloseHandle(hSnapShotm);
							CloseHandle(hSnapShot);
							CloseHandle(hProc);
							FreeLibrary(hInstLib);
							return 602;
						}
					}
					else
					{
						// Unable to open process for termination
						CloseHandle(hSnapShotm);
						CloseHandle(hSnapShot);
						FreeLibrary(hInstLib);
						return 604;
					}
				}
				else
				{  // Look for next modules for this process
					modentry.dwSize = sizeof(MODULEENTRY32);
					bResultm = lpfModule32Next(hSnapShotm, &modentry);
				}
			}

			//Keep looking
			CloseHandle(hSnapShotm);
			procentry.dwSize = sizeof(PROCESSENTRY32);
			bResult = lpfProcess32Next(hSnapShot, &procentry);
		}
		CloseHandle(hSnapShot);
	}
	if (iFound == 0)
	{
		FreeLibrary(hInstLib);
		return 603;
	}
	FreeLibrary(hInstLib);
	return 0;
}


//Based on http://stackoverflow.com/a/1173396
void Process::kill(id_type id, bool force) {
  if(id==0)
    return;
  HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
  if(snapshot) {
    PROCESSENTRY32 process;
    ZeroMemory(&process, sizeof(process));
    process.dwSize = sizeof(process);
    if(Process32First(snapshot, &process)) {
      do {
        if(process.th32ParentProcessID==id) {
          HANDLE process_handle = OpenProcess(PROCESS_TERMINATE, FALSE, process.th32ProcessID);
          if(process_handle) {
            TerminateProcess(process_handle, 2);
            CloseHandle(process_handle);
          }
        }
      } while (Process32Next(snapshot, &process));
    }
    CloseHandle(snapshot);
  }
  HANDLE process_handle = OpenProcess(PROCESS_TERMINATE, FALSE, id);
  if(process_handle) TerminateProcess(process_handle, 2);
}
