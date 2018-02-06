/// <reference path="plugins/compress/lz-string.js" />
/// <reference path="jquery-1.6.4-vsdoc.js" />
/// <reference path="applicationCore.js" />

/*注册Js全局对象，用来封装常量上下文信息*/
var SmartPrint;
SmartPrint = {

    /*客户端远程调用的server地址*/
    RemoteServerBaseAddress: "http://127.0.0.1:6699",
    /*远程System-API基础地址*/
    RemoteServerSystemWebAPIAddress: "http://127.0.0.1:6699/api",
    /*远程WorkBench-API基础地址*/
    RemoteServerBaseWorkBenchWebAPIAddress: "http://127.0.0.1:6670/api",
    /*远程Server上注册的Hubs地址*/
    RemoteServerHubs: "http://127.0.0.1:6699/signalr/hubs",
    /*通信的信道基础*/
    RemoteServerChannel: "http://127.0.0.1:6699/signalr",
    /*菜鸟打印组件的固定地址端口*/
    CaiNiaoSocketAddress: "ws://127.0.0.1:13528",
    /*登录凭证-正式环境需要进行判断*/
    Ticket: "C7BD4844D095D95B",
    /*是否在JsonP模式运行*/
    JsonpMode:false
};

/*对不不支持WebSocket的 开始加载本地模拟WebSocket代理*/
//if (!window.WebSocket) {
// <!--注册基于总线的js引用 SignalR hub script. -->
document.write("<script src='" + SmartPrint.RemoteServerHubs + "'><\/script>");

//} 
/*对不支持CORS头的 jsonp格式的请求 启动数据压缩，jsonp只能get 请求 防止数据过大无法请求*/
var jsonpMode = !$.support.cors;
if (jsonpMode == true) {
    SmartPrint.JsonpMode = true;
    document.write("<script src='Scripts/plugins/compress/lz-string.js'><\/script>");

}

/*
用来实现HTTP方式的调用菜鸟打印组件
//注意：如果检测到浏览器支持WebSocket  那么返回原生的WebSocket对象
*/

var PrinterHttpClient = {

    /*创建新的实例*/
    createNew: function (socketAddress) {


        console.log("PrinterHttpClient new a instance....." + new Date().toLocaleString());

        /**
       输出日志消息
       */
        var logMessage = function (methodName, event) {

            console.log("PrinterHttpClient call method is: {0}.".format(methodName));
            if (!isNullOrUndefined(event)) {
                console.log(event.data);
            }
        };


        //-------------基本的事件-------------begin-----------
        //这些事件 可以被override 重写
        var fn_onopen = function (event) {
            if (!isNullOrUndefined(event)) {
                logMessage("onopen", event.data);
            }
            else {
                logMessage("onopen");
            }
        }

        var fn_onclose = function (event) {
            if (!isNullOrUndefined(event)) {
                logMessage("onclose", event.data);
            }
            else {
                logMessage("onclose");
            }
        }
        var fn_onmessage = function (event) {
            if (!isNullOrUndefined(event)) {
                logMessage("onmessage", event);

                var data = JSON.parse(event.data);
                if ("getPrinters" == data.cmd) {
                    alert('打印机列表:' + JSON.stringify(data.printers));

                    //var defaultPrinter = data.defaultPrinter;
                    //alert('默认打印机为:' + defaultPrinter);
                } else {
                    alert("返回数据:" + JSON.stringify(event.data));
                }


            } else {
                logMessage("onmessage");
            }

        }
        var fn_onerror = function (event) {
            if (!isNullOrUndefined(event)) {
                logMessage("onerror", event.data);
            } else {
                logMessage("onerror");
            }

        }
        //-------------基本的事件-------------end-----------




        //如果支持WebSocket  那么返回原生的对象
        /* if (window.WebSocket) {
             var addr = SmartPrint.CaiNiaoSocketAddress;
             if (!isNullOrEmpty(socketAddress)) {
                 addr = socketAddress;
             }
             var socket = new WebSocket(addr);
             //----------注册基本的事件--------begin-------
             socket.onopen = fn_onopen;
             socket.onclose = fn_onclose;
             socket.onmessage = fn_onmessage;
             socket.onerror = fn_onerror;
             //----------注册基本的事件--------end-------
             alert("成功建立原生WebSocket远程调用连接！");
             return socket;
         } */


        //对于不支持WebSocket的 。模拟实现WebSocket对象。保持跟原生的WebSocket的属性 方法事件的一致性

        var instance = {
            //一个唯一标识
            id: generateUUID(),
            readyState: PrinterHttpClient.readyStateType.CLOSED,//状态
            protocol: ""//使用的协议
        };
        //绑定_this 关键词为当前运行的区域的上下文
        var _this = instance;
        //客户端代理对象
        var proxyClient = null;
        /*----------------------私有函数-开始-----------------------------*/
        /*****************************私有函数在当前域调用*************************************/





        /*Ping提供套接字的SignalR 远程Server*/
        var ping = function () {

            // Start the connection.
            $.connection.hub.start().done(function () {
                //调用server的ping方法
                var addr = "";
                if (!isNullOrEmpty(socketAddress)) {
                    addr = socketAddress;
                }
                proxyClient.server.ping(instance.id, addr);

            });
        };

        /*----------------------私有函数-结束-----------------------------*/



        //注册公开public方法
        //instance.placeAt = function (postion) {
        //    if (postion == null) {
        //        throw new Error('postion 参数不能为空！');
        //        return;
        //    }


        //-------------基本的事件-------------begin-----------
        //这些事件 可以被override 重写
        instance.onopen = fn_onopen;

        instance.onclose = fn_onclose;

        instance.onmessage = fn_onmessage;

        instance.onerror = fn_onerror;
        //-------------基本的事件-------------end-----------


        //------------基本的方法---------------begin-----------
        instance.send = function (message) {
            if (null == proxyClient) {
                throw new Error("客户端代理对象已经被销毁！");
            }
            // Start the connection.
            $.connection.hub.start().done(function () {
                proxyClient.server.send(instance.id, socketAddress, message);

            });
        }
        /*向服务端发送任务号，而不是提交完整的数据，防止大数据块阻塞HTTP通信*/
        instance.sendTask = function (taskId) {
            if (null == proxyClient) {
                throw new Error("客户端代理对象已经被销毁！");
            }
            // Start the connection.
            $.connection.hub.start().done(function () {
                proxyClient.server.sendTask(instance.id, taskId);

            });
        }


        //------------基本的方法---------------end-----------


        //自动初始化
        var init = function () {


            //Set the hubs URL for the connection
            $.connection.hub.url = SmartPrint.RemoteServerChannel;//"http://localhost:6699/signalr";

            // Declare a proxy to reference the hub.
            proxyClient = $.connection.printerHub;
	    if(isNullOrUndefined(proxyClient)|| isNullOrUndefined(proxyClient.client)){
		alert('未能发现本地打印承载服务！请下载安装打印组件SDK！');
		return;
	    }
            // Create a function that the hub can call to broadcast messages.
            //设置客户端的套接字描述信息
	    proxyClient.client.setClientSocketDescription = function (status, protocol) {
                instance.readyState = status;
                instance.protocol = protocol;
            }
            // Create a function that the hub can call to broadcast messages.
            //设置客户端的套接字描述信息
            proxyClient.client.callbackOfPing = function (args) {

                if (args) {
                    if (args.data && args.data == "success") {
                        alert("成功建立Http代理远程调用连接！");
                        return;
                    }
                }
                alert("未能成功建立远程调用连接！无法正常使用！");

            }

            //设置客户端的自身消息检测
            proxyClient.client.onCheckClientStatus = function (messageId, clientId) {
                if (isNullOrEmpty(messageId) || isNullOrEmpty(clientId)) {
                    return;
                }
                //检测是否是检测自身的消息
                if (clientId != instance.id) {
                    return;
                }


                //如果是来自自身，那么调用server  声明自身未被注销，仍在使用中
                // Start the connection.
                $.connection.hub.start().done(function () {
                    //通知server  仍在使用中。
                    /***模拟场景：房东通知租客交房租，个人向房东表态已经交了租了，不交租就收回房子***/
                    proxyClient.server.giveMoneyToHouseOwner(instance.id, messageId);

                });
            };


            proxyClient.client.onopen = function (clientId, args) {
                if (isNullOrEmpty(clientId) || clientId != instance.id) {
                    return;
                }
                instance.onopen(args);
            }
            proxyClient.client.onclose = function (clientId, args) {
                if (isNullOrEmpty(clientId) || clientId != instance.id) {
                    return;
                }
                instance.onclose(args);
            }
            proxyClient.client.onmessage = function (clientId, args) {
                if (isNullOrEmpty(clientId) || clientId != instance.id) {
                    return;
                }
                instance.onmessage(args);
            }
            proxyClient.client.onerror = function (clientId, args) {
                if (isNullOrEmpty(clientId) || clientId != instance.id) {
                    return;
                }
                instance.onerror(args);
            }

            //发送一次ping握手请求
            ping();

        }();

        return instance;

    },

    /*rest get http request*/
    httpGet: function (url, callbackHandler, paras) {
	    paras = JSON.stringify(paras);//转换为字符串
		jQuery.support.cors = true;
        $.ajax({
            type: "get",
            url: url,
            data: paras,
            dataType: "json",
            success: function (msg) {
                callbackHandler(msg);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                throw "httpGet error! ";
            }
        });

    },
    /*rest post requset*/
    httpPost: function (url, callbackHandler, paras) {
        paras = JSON.stringify(paras);//转换为字符串
		jQuery.support.cors = true;
        $.ajax({
            type: "post",
            contentType: "application/json",
            url: url,
            data: paras,
            dataType: "json",
            success: function (msg) {
                callbackHandler(msg);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                throw "httpPost error! ";
            }
        });
    },
    /*获取菜鸟组件状态*/
    getCheckCaiNiaoPrinterStatus: function (callbackHanlder) {
        var route = "/system/CheckCaiNiaoPrinterStatus";//路由
        var apiAddress = SmartPrint.RemoteServerSystemWebAPIAddress + route;//绝对路径
        var para = { "timetoken": getTimeToken() };
        PrinterHttpClient.httpGet(apiAddress, callbackHanlder, para);

    },
    /*获取WorkBench是否运行中*/
    getIsWorkBenchIsRunning: function (callbackHanlder) {
        var route = "/system/IsWorkBenchIsRunning";//路由

        var apiAddress = SmartPrint.RemoteServerSystemWebAPIAddress + route;//绝对路径
        var para = { "timetoken": getTimeToken() };

        PrinterHttpClient.httpGet(apiAddress, callbackHanlder, para);

    },
    /*将打印消息发送到服务端*/
    sendTaskMessageToServer: function (msg,callbackHanlder) {
        var route = "/system/ReceiveTaskMessage";//路由

        var apiAddress = SmartPrint.RemoteServerSystemWebAPIAddress + route;//绝对路径
        var para = { "Message": msg };

        PrinterHttpClient.httpPost(apiAddress, callbackHanlder, para);

    },

    /*启动WorkBench*/
    startWorkBench: function (callbackHanlder) {
        var route = "/system/StartWorkBench";//路由
        var apiAddress = SmartPrint.RemoteServerSystemWebAPIAddress + route;//绝对路径
        var para = { "timetoken": getTimeToken() };
        PrinterHttpClient.httpPost(apiAddress, callbackHanlder, para);

    },


    /*获取菜鸟组件的安装路径*/
    getCaiNiaoInstallPath: function (callbackHanlder) {
        var route = "/workbench/GetCaiNiaoInstallPath";//路由
		
        var apiAddress = SmartPrint.RemoteServerBaseWorkBenchWebAPIAddress + route;//绝对路径
		var para={"timetoken":getTimeToken()};
		
        PrinterHttpClient.httpGet(apiAddress, callbackHanlder,para);

    },

    /*获取菜鸟扩展的版本*/
    getCaiNiaoExtensionVersion: function (callbackHanlder) {
        var route = "/workbench/GetVersionInfo";//路由
        var apiAddress = SmartPrint.RemoteServerBaseWorkBenchWebAPIAddress + route;//绝对路径
		var para={"timetoken":getTimeToken()};
		
        PrinterHttpClient.httpGet(apiAddress, callbackHanlder,para);

    },
   

    /*重启菜鸟组件*/
    restartCaiNiaoPrinter: function (callbackHanlder) {
        var route = "/workbench/RestartCaiNiaoPrinter";//路由
        var apiAddress = SmartPrint.RemoteServerBaseWorkBenchWebAPIAddress + route;//绝对路径
		var para={"timetoken":getTimeToken()};
        PrinterHttpClient.httpGet(apiAddress, callbackHanlder,para);

    },

    /*关闭菜鸟组件*/
    closeCaiNiaoPrinter: function (callbackHanlder) {
        var route = "/workbench/CloseCaiNiaoPrinter";//路由
        var apiAddress = SmartPrint.RemoteServerBaseWorkBenchWebAPIAddress + route;//绝对路径
		var para={"timetoken":getTimeToken()};
        PrinterHttpClient.httpGet(apiAddress, callbackHanlder,para);

    },
    /*启动菜鸟组件*/
    startCaiNiaoPrinter: function (callbackHanlder) {
        var route = "/workbench/StartCaiNiaoPrinter";//路由
        var apiAddress = SmartPrint.RemoteServerBaseWorkBenchWebAPIAddress + route;//绝对路径
		var para={"timetoken":getTimeToken()};
        PrinterHttpClient.httpGet(apiAddress, callbackHanlder,para);

    },


    /*设定默认打印机*/
    setDefaultPrinter: function (printerName,callbackHanlder) {
        var route = "/workbench/SetDefaultPrinter";//路由
        var apiAddress = SmartPrint.RemoteServerBaseWorkBenchWebAPIAddress + route;//绝对路径
        if (isNullOrEmpty(printerName)) {
            throw new Error("默认打印机名称设置 ，打印机名称不能为空！");
            return;
        }
        var paras = { "PrinterName": printerName };
        PrinterHttpClient.httpPost(apiAddress, callbackHanlder, paras);

    },

    // 枚举 readyState报告其连接状态
    //WebSocket通过只读特性readyState报告其连接状态，连接状态共有四个，使用者可以根据这个特性判断此时的连接状态，然后再进行下一步行动。
    readyStateType: {
        CONNECTING: "0",//连接正在进行中，但还未建立
        OPEN: "1",//连接已建立，消息可以开始传递
        CLOSING: "2",//连接正在进行关闭
        CLOSED: "3"//连接已关闭
    }

}

