编译环境
请在32位机器上编译。最好是XP下编译x86版本

 基于VC++2015运行时私有化，将运行时放置到bin目录下，在程序启动的时候 从当前程序域中发现C++运行时！
exe程序执行的时候 ，引导程序首先加载引用的程序集支撑。C++程序 启动的时候，先检索当前文件夹，也就是当前的APPDomain

注意：---------------------
hook  作为Windows  服务 ，local system的权限运行
2  创建Windows 服务的时候 ，参数 CreateService方法中Service type ：SERVICE_WIN32_OWN_PROCESS|SERVICE_INTERACTIVE_PROCESS, 可交互的服务

3 The NoInteractiveServices value defaults to 1, which means that no service is allowed to run interactively, 
regardless of whether it has SERVICE_INTERACTIVE_PROCESS. When NoInteractiveServices is set to a 0,
 services with SERVICE_INTERACTIVE_PROCESS are allowed to run interactively.

Windows 7, Windows Server 2008 R2, Windows XP and Windows Server 2003:  
The NoInteractiveServices value defaults to 0, which means that services 
with SERVICE_INTERACTIVE_PROCESS are allowed to run interactively. 
When NoInteractiveServices is set to a nonzero value, no service started thereafter is allowed to run interactively, 
regardless of whether it has SERVICE_INTERACTIVE_PROCESS.


4 HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows 的项 NoInteractiveServices 需要设置为0

5 需要启动的程序 不能用UAC 权限控制。否则需要提权操作。如：运行需要管理员 等某个特定角色的



而在Windows Vista中，服务在一个叫做Session 0 的特殊Session中承载。
由于应用程序运行在用户登录到系统后所创建的Session 0 之后的Session中，
所以应用程序和服务也就隔离开来：第一个登录的用户在Session 1中，第二个在Session 2中，以此类推。
事实上运行在不同的Session中，如果没有特别将其放入全局命名空间（并且设置了相应的访问控制配置），
是不能互相传递窗体消息，共享UI元素或者共享kernel对象。


参考文献：
https://code.msdn.microsoft.com/windowsapps/CSUACSelfElevation-644673d3
https://msdn.microsoft.com/en-us/library/ms682429.aspx
https://msdn.microsoft.com/en-us/library/aa379608.aspx
https://www.codeproject.com/articles/35773/subverting-vista-uac-in-both-and-bit-archite
https://msdn.microsoft.com/en-us/library/windows/desktop/ms684190%28v=vs.85%29.aspx
http://www.howtogeek.com/school/using-windows-admin-tools-like-a-pro/lesson8/
https://msdn.microsoft.com/en-us/library/windows/desktop/ms683502%28v=vs.85%29.aspx
https://msdn.microsoft.com/en-us/library/windows/desktop/ms682450%28v=vs.85%29.aspx
http://blog.csdn.net/c0ast/article/details/18285645
http://blog.csdn.net/highyyy/article/details/6132099
http://www.cnblogs.com/gnielee/archive/2010/04/07/session0-isolation-part1.html
http://www.cnblogs.com/gnielee/archive/2010/04/08/1707169.html