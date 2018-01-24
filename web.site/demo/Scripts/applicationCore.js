/*
定义JQuery中对选择的集合Elements 进行迭代
*/
if (jQuery && !jQuery.fn.forEach) {
    $(function () {
        (function ($) {
            $.fn.extend({
                /**
                 * 扩展Jq对象的遍历，对Jq对象进行迭代
                 * @param {} predicate 
                 * @returns {} 
                 */
                forEach: function (predicate) {

                    if (this == null) {
                        throw new TypeError(' this is null or not defined');
                    }
                    // 1. Let O be the result of calling toObject() passing the
                    // |this| value as the argument.
                    var O = Object(this);

                    // 2. If isCallable(predicate) is false, throw a TypeError exception. 
                    if (typeof predicate !== "function") {
                        throw new TypeError(predicate + ' is not a function');
                    }

                    //3 call the jq  original API  for iteror
                    $.each(O, function (index, domEle) {
                        predicate($(domEle));
                    });
                }
            })
        })(jQuery);

    });
}

/**
 * 返回 Select HTML元素的选择的项的索引
 * @param {} domObj 
 * @returns {} 
 */
var getSelectDomSelectedIndex = function (domObj) {

    var result = -1;
    if (isNullOrUndefined(domObj)) {
        return result;
    }

    result = domObj.prop('selectedIndex');
    if (isNullOrUndefined(result)) {
        result = -1;
    }

    return result;
}


/*字符串拼接 powered by 瓦力
    Append():添加待拼接的字符串
    ToString():将待拼接的字符串拼接成字符串返回  split参数：已什么符号分割
    Clear():清空待拼接对象
    Remove():移出某个待拼接的对象 n参数：索引
*/
var StringBuilderContainer = function () {
    this.data = Array("");
    this.Append = function () {
        this.data.push(arguments[0]);
        return this;
    };
    this.ToString = function (split) {
        if (typeof split == "undefined") {
            return this.data.join('');
        }
        return this.data.join(split);
    };
    this.Clear = function () {
        this.data = [];
    };
    this.Remove = function (n) {
        if (n < 0)
            return this.data;
        else {
            this.data = this.data.slice(0, n).concat(this.data.slice(n + 1, this.length));
            return this.data;
        }
    };
};

//对象Obj 打印为字符串
function obj2string(o) {
    var r = [];
    if (typeof o == "string") {
        return "\"" + o.replace(/([\'\"\\])/g, "\\$1").replace(/(\n)/g, "\\n").replace(/(\r)/g, "\\r").replace(/(\t)/g, "\\t") + "\"";
    }
    if (typeof o == "object") {
        if (!o.sort) {
            for (var i in o) {
                r.push(i + ":" + obj2string(o[i]));
            }
            if (!!document.all && !/^\n?function\s*toString\(\)\s*\{\n?\s*\[native code\]\n?\s*\}\n?\s*$/.test(o.toString)) {
                r.push("toString:" + o.toString.toString());
            }
            r = "{" + r.join() + "}";
        } else {
            for (var i = 0; i < o.length; i++) {
                r.push(obj2string(o[i]))
            }
            r = "[" + r.join() + "]";
        }
        return r;
    }
    return o.toString();
}


/*
压缩json字符串
 * @param  inputString  
  * @param  ii  ；1 压缩 2 转义 3 压缩并转义 4 去除转义
 * @returns {} 
*/
    var compressJsonStr = function (inputString, ii) {
        var text = inputString;
        if ((ii == 1 || ii == 3)) {
            text = text.split("\n").join(" ");
            var t = [];
            var inString = false;
            for (var i = 0, len = text.length; i < len; i++) {
                var c = text.charAt(i);
                if (inString && c === inString) {
                    if (text.charAt(i - 1) !== '\\') {
                        inString = false;
                    }
                }
                else if (!inString && (c === '"' || c === "'")) {
                    inString = c;
                }
                else if (!inString && (c === ' ' || c === "\t")) {
                    c = '';
                }
                t.push(c);
            }
            text = t.join('');
        }
        if ((ii == 2 || ii == 3)) {
            text = text.replace(/\\/g, "\\\\").replace(/\"/g, "\\\"");
        }
        if (ii == 4) {
            text = text.replace(/\\\\/g, "\\").replace(/\\\"/g, '\"');
        }
        return text;
    };


/**
 * 扩展js 中的string 函数contains
 * @param {} substr 
 * @returns {} 
 */
String.prototype.contains = function (substr) {
    return this.indexOf(substr) >= 0;
};
/**
 * 扩展js 中的string 函数format
 * @param {} args ，参数数组
 * @returns {} 
 */
String.prototype.format = function () {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function (match, number) {
        return typeof args[number] != 'undefined'
            ? args[number]
            : match;
    });
};
// 对Date的扩展，将 Date 转化为指定格式的String
// 月(M)、日(d)、小时(H)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符，毫秒(f)只能用 1 个占位符(是 1-3 位的数字) 
// 例子： 
// (new Date()).Format("yyyy-MM-dd HH:mm:ss.f") ==> 2006-07-02 08:09:04.423 
// (new Date()).Format("yyyy-M-d H:m:s.f")      ==> 2006-7-2 8:9:4.18 
Date.prototype.Format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "H+|h+": this.getHours(), //小时 --------*********仅支持24小时制**************
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "f+": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
};

//将指定的毫秒数加到此实例的值上 
Date.prototype.addMilliseconds = function (value) {
    var millisecond = this.getMilliseconds();
    this.setMilliseconds(millisecond + value);
    return this;
};
//将指定的秒数加到此实例的值上 
Date.prototype.addSeconds = function (value) {
    var second = this.getSeconds();
    this.setSeconds(second + value);
    return this;
};
//将指定的分钟数加到此实例的值上 
Date.prototype.addMinutes = function (value) {
    var minute = this.addMinutes();
    this.setMinutes(minute + value);
    return this;
};
//将指定的小时数加到此实例的值上 
Date.prototype.addHours = function (value) {
    var hour = this.getHours();
    this.setHours(hour + value);
    return this;
};
//将指定的天数加到此实例的值上 
Date.prototype.addDays = function (value) {
    var date = this.getDate();
    this.setDate(date + value);
    return this;
};
//将指定的星期数加到此实例的值上 
Date.prototype.addWeeks = function (value) {
    return this.addDays(value * 7);
};
//将指定的月份数加到此实例的值上 
Date.prototype.addMonths = function (value) {
    var month = this.getMonth();
    this.setMonth(month + value);
    return this;
};
//将指定的年份数加到此实例的值上 
Date.prototype.addYears = function (value) {
    var year = this.getFullYear();
    this.setFullYear(year + value);
    return this;
};
//获取时间的间隔（天）
Date.prototype.dateDiff = function (endDate) {
    var startDate = this;


    var startTime = new Date(Date.parse(startDate.replace(/-/g, "/"))).getTime();
    var endTime = new Date(Date.parse(endDate.replace(/-/g, "/"))).getTime();
    var days = Math.abs((startTime - endTime)) / (1000 * 60 * 60 * 24);
    return days;
};

/**
 * 自定义数据结构-C#方式的迭代
 */

Array.prototype.add = function (obj) {
    this.push(obj);
};
Array.prototype.addRange = function (items) {
    for (var i = 0; i < items.length; i++) {
        this.add(items[i]);
    }
};
/**
 * 清空数组
 * @returns {} 
 */
Array.prototype.clear = function () {
    this.splice(0, this.length);
};
Array.prototype.contains = function (obj) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] === obj) {
            return true;
        }
    }
    return false;
};
Array.prototype.convertAll = function (converter) {
    var list = new Array();
    for (var i = 0; i < this.length; i++) {
        list.add(converter(this[i]));
    }
    return list;
};
Array.prototype.remove = function (predicate) {
    for (var i = 0; i < this.length; i++) {
        if (predicate(this[i])) {
            this.splice(i, 1);//移除这个索引的元素
        }
    }
};

Array.prototype.removeAt = function (index) {
    if ((this[index])) {
        this.splice(index, 1);//移除这个索引的元素
    }

};



Array.prototype.find = function (predicate) {
    for (var i = 0; i < this.length; i++) {
        if (predicate(this[i])) {
            return this[i];
        }
    }
    return null;
};
Array.prototype.findAll = function (predicate) {
    var results = new Array();
    for (var i = 0; i < this.length; i++) {
        if (predicate(this[i])) {
            results.add(this[i]);
        }
    }
    return results;
};
Array.prototype.findIndex = function (predicate, index) {
    if (index === void 0) { index = 0; }
    for (var i = index || 0; i < this.length; i++) {
        if (predicate(this[i])) {
            return i;
        }
    }
    return -1;
};
Array.prototype.findLastIndex = function (predicate, index) {
    if (index === void 0) { index = this.length; }
    for (var i = index; i > -1; i--) {
        if (predicate(this[i])) {
            return i;
        }
    }
    return -1;
};
Array.prototype.forEach = function (action) {
    for (var i = 0; i < this.length; i++) {
        action(this[i]);
    }
};
/**
 * 以jqery 对象的方式迭代对象
 * @param {} action 
 * @returns {} 
 */
Array.prototype.forEachJq = function (action) {
    for (var i = 0; i < this.length; i++) {
        action($(this[i]));
    }
};
Array.prototype.getItem = function (index) {
    if (this[index]) {
        return this[index];
    }
};
Array.prototype.setItem = function (index, obj) {
    this[index] = obj;
};
Array.prototype.toArray = function () {
    var arr = [];
    for (var i = 0; i < this.length; i++) {
        arr.push(this[i]);
    }
    return arr;
};



/**
 * 分组
 * Demo:
 * var arr=[{"id":1,"name":"A"},
{"id":1,"name":"B"},
{"id":2,"name":"C"},];

    var g=arr.groupBy("id");
    for (var i = 0; i < g.Keys.length; i++) {
             var gv=g.GetItem(g.Keys[i]);
		     for (var k = 0; k < gv.length; k++) {
			     document.writeln(gv[k].name)
		     }
    }
 * @param {} key 属性（仅仅支持单个属性），并且key的值属性，不可为空
 * @returns {}  返回Map字典
 */
Array.prototype.groupBy = function (key) {

    if (isNullOrUndefined(key)) {
        throw new error("key is null!");
        return null;
    }

    var dicMap = new Map();

    for (var i = 0; i < this.length; i++) {
        var item = this[i];
        if (item && item[key]) {
            //存在这个属性，并有值
            var gKey = item[key];
            if (dicMap.Has(gKey)) {
                var hashObj = dicMap.GetItem(gKey);
                hashObj.push(item);//存在键，追加
            } else {
                //不存在存在键，创建
                var value = [];
                value.push(item);
                dicMap.Add(gKey, value);
            }
        }



    }
    return dicMap;
};

Array.prototype.trueForAll = function (predicate) {
    var results = new Array();
    for (var i = 0; i < this.length; i++) {
        if (!predicate(this[i])) {
            return false;
        }
    }
    return true;
};

/** 
 * Map 字典类，使用方式：
 *   var map = new Map<string>();
       map.Add("a","111");
       map.Add("b", "222");
       map.Add("c", "333");

       var keys = map.getKeys();

       for (var i = 0; i < keys.length; i++) {
           document.write(map.GetItem(keys[i]));
       }
*/
var Map = (function () {
    function Map() {
        this.Items = {};
        this._Count = 0;
        this._Keys = [];
        this._Values = [];
    }
   
    /**
*获取数目
*/
    Map.prototype.getCount = function () {
        return this._Count;
    };
   
    /**
*获取所有的键
*/
    Map.prototype.getKeys = function () {
        return this._Keys;
    };
    /**
    *获取所有的值
    */
    Map.prototype.getValues = function () {
        return this._Values;
    };

    /**
     * 添加减值对象
     * @param {} key 
     * @param {} value 
     * @returns {} 
     */
    Map.prototype.Add = function (key, value) {
        try {
            this._Count += 1;
            this._Keys.push(key);
            this._Values.push(value);
            this.Items[key] = value;
        }
        catch (e) {
        }
    };
    Map.prototype.Has = function (key) {
        return key in this.Items;
    };
    Map.prototype.GetItem = function (key) {
        return this.Items[key];
    };
    return Map;
})();


var List = (function () {
    function List(list) {
        this.list = [];
        this.list = list || [];
    }
    //Object.defineProperty(List.prototype, "length", {
    //    get: function () {
    //        return this.list.length;
    //    },
    //    enumerable: true,
    //    configurable: true
    //});

    List.prototype.getCount = function () {
        return this.list.length;
    };

    List.prototype.add = function (obj) {
        this.list.push(obj);
    };
    List.prototype.addRange = function (items) {
        this.list = this.list.concat(items);
    };
    List.prototype.clear = function () {
        this.list = [];
    };
    List.prototype.contains = function (obj) {
        for (var i = 0; i < this.list.length; i++) {
            if (this.list[i] === obj) {
                return true;
            }
        }
        return false;
    };
    List.prototype.convertAll = function (converter) {
        var list = new List();
        for (var i = 0; i < this.list.length; i++) {
            list.add(converter(this.list[i]));
        }
        return list;
    };
    List.prototype.remove = function (predicate) {
        for (var i = 0; i < this.list.length; i++) {
            if (predicate(this.list[i])) {
                this.list.splice(i, 1);//移除这个索引的元素
            }
        }
    };

    List.prototype.removeAt = function (index) {
        if ((this.list[index])) {
            this.list.splice(index, 1);//移除这个索引的元素
        }

    };



    List.prototype.find = function (predicate) {
        for (var i = 0; i < this.list.length; i++) {
            if (predicate(this.list[i])) {
                return this.list[i];
            }
        }
        return null;
    };
    List.prototype.findAll = function (predicate) {
        var results = new List();
        for (var i = 0; i < this.list.length; i++) {
            if (predicate(this.list[i])) {
                results.add(this.list[i]);
            }
        }
        return results;
    };
    List.prototype.findIndex = function (predicate, index) {
        if (index === void 0) { index = 0; }
        for (var i = index || 0; i < this.list.length; i++) {
            if (predicate(this.list[i])) {
                return i;
            }
        }
        return -1;
    };
    List.prototype.findLastIndex = function (predicate, index) {
        if (index === void 0) { index = this.length; }
        for (var i = index; i > -1; i--) {
            if (predicate(this.list[i])) {
                return i;
            }
        }
        return -1;
    };
    List.prototype.forEach = function (action) {
        for (var i = 0; i < this.list.length; i++) {
            action(this.list[i]);
        }
    };

    /**
     * 以jqery 对象的方式迭代对象
     * @param {} action 
     * @returns {} 
     */
    List.prototype.forEachJq = function (action) {
        for (var i = 0; i < this.list.length; i++) {
            action($(this.list[i]));
        }
    };

    List.prototype.getItem = function (index) {
        if (this.list[index]) {
            return this.list[index];
        }
    };
    List.prototype.setItem = function (index, obj) {
        this.list[index] = obj;
    };
    List.prototype.toArray = function () {
        var arr = [];
        for (var i = 0; i < this.list.length; i++) {
            arr.push(this.list[i]);
        }
        return arr;
    };
    /**
     * 分组
     * Demo:
     * var arr=[{"id":1,"name":"A"},
    {"id":1,"name":"B"},
    {"id":2,"name":"C"},];
    
        var g=arr.groupBy("id");
        for (var i = 0; i < g.Keys.length; i++) {
                 var gv=g.GetItem(g.Keys[i]);
                 for (var k = 0; k < gv.length; k++) {
                     document.writeln(gv[k].name)
                 }
        }
     * @param {} key 属性（仅仅支持单个属性），并且key的值属性，不可为空
     * @returns {}  返回Map字典
     */
    List.prototype.groupBy = function (key) {

        if (isNullOrUndefined(key)) {
            throw new error("key is null!");
            return null;
        }

        var dicMap = new Map();

        for (var i = 0; i < this.list.length; i++) {
            var item = this.list[i];
            if (item && item[key]) {
                //存在这个属性，并有值
                var gKey = item[key];
                if (dicMap.Has(gKey)) {
                    var hashObj = dicMap.GetItem(gKey);
                    hashObj.push(item);//存在键，追加
                } else {
                    //不存在存在键，创建
                    var value = [];
                    value.push(item);
                    dicMap.Add(gKey, value);
                }
            }



        }
        return dicMap;
    };


    List.prototype.trueForAll = function (predicate) {
        var results = new List();
        for (var i = 0; i < this.list.length; i++) {
            if (!predicate(this.list[i])) {
                return false;
            }
        }
        return true;
    };
    return List;
}());




/**
 * @ngdoc function
 * @name sUndefined
 * @module ng
 * @kind function
 *
 * @description
 * Determines if a reference is undefined.
 *
 * @param {*} value Reference to check.
 * @returns {boolean} True if `value` is undefined.
 */
function isUndefined(value) { return typeof value === 'undefined'; }


/**
 * @ngdoc function
 * @name sDefined
 * @module ng
 * @kind function
 *
 * @description
 * Determines if a reference is defined.
 *
 * @param {*} value Reference to check.
 * @returns {boolean} True if `value` is defined.
 */
function isDefined(value) { return typeof value !== 'undefined'; }


/**
 * @ngdoc function
 * @name sObject
 * @module ng
 * @kind function
 *
 * @description
 * Determines if a reference is an `Object`. Unlike `typeof` in JavaScript, `null`s are not
 * considered to be objects. Note that JavaScript arrays are objects.
 *
 * @param {*} value Reference to check.
 * @returns {boolean} True if `value` is an `Object` but not `null`.
 */
function isObject(value) {
    // http://jsperf.com/isobject4
    return value !== null && typeof value === 'object';
}


/**
 * Determine if a value is an object with a null prototype
 *
 * @returns {boolean} True if `value` is an `Object` with a null prototype
 */
function isBlankObject(value) {
    return value !== null && typeof value === 'object' && !getPrototypeOf(value);
}


/**
 * @ngdoc function
 * @name sString
 * @module ng
 * @kind function
 *
 * @description
 * Determines if a reference is a `String`.
 *
 * @param {*} value Reference to check.
 * @returns {boolean} True if `value` is a `String`.
 */
function isString(value) { return typeof value === 'string'; }


/**
 * @ngdoc function
 * @name sNumber
 * @module ng
 * @kind function
 *
 * @description
 * Determines if a reference is a `Number`.
 *
 * This includes the "special" numbers `NaN`, `+Infinity` and `-Infinity`.
 *
 * If you wish to exclude these then you can use the native
 * [`isFinite'](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/isFinite)
 * method.
 *
 * @param {*} value Reference to check.
 * @returns {boolean} True if `value` is a `Number`.
 */

function isNumber(value) {

    return !isNaN(parseFloat(value)) && isFinite(value);

}


/**
 * @ngdoc function
 * @name sDate
 * @module ng
 * @kind function
 *
 * @description
 * Determines if a value is a date.
 *
 * @param {*} value Reference to check.
 * @returns {boolean} True if `value` is a `Date`.
 */
function isDate(value) {
    return Object.prototype.toString.call(value) === '[object Date]';
}


/**
 * @ngdoc function
 * @name sArray
 * @module ng
 * @kind function
 *
 * @description
 * Determines if a reference is an `Array`.
 *
 * @param {*} value Reference to check.
 * @returns {boolean} True if `value` is an `Array`.
 */


function isArray(value) {
    return Object.prototype.toString.call(value) === '[object Array]';
}
/**
 * 是否为List类型
 * @param {} value 
 * @returns {} 
 */
function isList(value) {
    return value instanceof List;
}

/**
 * @ngdoc function
 * @name sFunction
 * @module ng
 * @kind function
 *
 * @description
 * Determines if a reference is a `Function`.
 *
 * @param {*} value Reference to check.
 * @returns {boolean} True if `value` is a `Function`.
 */
function isFunction(value) { return typeof value === 'function'; }


/**
 * Determines if a value is a regular expression object.
 *
 * @private
 * @param {*} value Reference to check.
 * @returns {boolean} True if `value` is a `RegExp`.
 */
function isRegExp(value) {
    return Object.prototype.toString.call(value) === '[object RegExp]';
}


/**
 * Checks if `obj` is a window object.
 *
 * @private
 * @param {*} obj Object to check
 * @returns {boolean} True if `obj` is a window obj.
 */
function isWindow(obj) {
    return obj && obj.window === obj;
}


function isScope(obj) {
    return obj && obj.$evalAsync && obj.$watch;
}


function isFile(obj) {
    return Object.prototype.toString.call(obj) === '[object File]';
}


function isFormData(obj) {
    return Object.prototype.toString.call(obj) === '[object FormData]';
}


function isBlob(obj) {
    return Object.prototype.toString.call(obj) === '[object Blob]';
}


function isBoolean(value) {
    return typeof value === 'boolean';
}

function isNullOrUndefined(obj) {

    if (isUndefined(obj)) {
        return true;
    } else if (null == obj) {
        return true;
    }
    return false;
}

function isNullOrEmpty(str) {


    if (!isNullOrUndefined(str)) {
        if (!isString(str) || str.length > 0) {
            return false;
        }

    }
    return true;
}

function trimSpecialStr(str) {
    return str.replace("&#", "");
}

function trim(str) {
    return str.replace(/(^\s*)|(\s*$)/g, '');
}


function ltrim(str) {
    return str.replace(/^\s*/g, '');
}


function rtrim(str) {
    return str.replace(/\s*$/, '');
}


function equals(str1, str2) {
    if (str1 == str2) {
        return true;
    }
    return false;
}


function equalsIgnoreCase(str1, str2) {
    if (str1.toUpperCase() == str2.toUpperCase()) {
        return true;
    }
    return false;
}


function isChinese(str) {
    var str = str.replace(/(^\s*)|(\s*$)/g, '');
    if (!(/^[\u4E00-\uFA29]*$/.test(str)
            && (!/^[\uE7C7-\uE7F3]*$/.test(str)))) {
        return false;
    }
    return true;
}


function isEmail(str) {
    if (/^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$/.test(str)) {
        return true
    }
    return false;
}
/**
 * 
 * @param {} str image url or name
 * @returns {} 
 */
function isImgOfJpeg(str) {
    var objReg = new RegExp("[.]+(jpg|jpeg)$", "gi");
    if (objReg.test(str)) {
        return true;
    }
    return false;
}

function isImg(str) {
    var objReg = new RegExp("[.]+(jpg|jpeg|swf|gif|png)$", "gi");
    if (objReg.test(str)) {
        return true;
    }
    return false;
}


function isInteger(str) {
    if (/^-?\d+$/.test(str)) {
        return true;
    }
    return false;
}
/**
 * 是否为拉丁字符
 * @param {} str 
 * @returns {} 
 */
function isEnglishChar(str) {
    if (/^[a-z|A-Z]$/.test(str)) {
        return true;
    }
    return false;
}

function isFloat(str) {
    if (/^(-?\d+)(\.\d+)?$/.test(str)) {
        return true;
    }
    return false;
}


function isMobile(str) {
    if (/^1[35]\d{9}/.test(str)) {
        return true;
    }
    return false;
}


function isPhone(str) {
    if (/^(0[1-9]\d{1,2}-)\d{7,8}(-\d{1,8})?/.test(str)) {
        return true;
    }
    return false;
}

function isIP(str) {
    var reg = /^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$/;
    if (reg.test(str)) {
        return true;
    }
    return false;
}


function isDateTimeString(str) {
    var reg = /^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$/;
    if (reg.test(str)) {
        return true;
    }
    return false;
}

/**
*仅仅把字符串中包含的中文转化为 unicode
 */
function GB2312ToUnicode(theString) {
    var theString = escape(theString).replace(/%u/gi, '\\u');
    return theString.replace(/%7b/gi, '{').replace(/%7d/gi, '}').replace(/%3a/gi, ':').replace(/%2c/gi, ',').replace(/%27/gi, '\'').replace(/%22/gi, '"').replace(/%5b/gi, '[').replace(/%5d/gi, ']');
}


/**
*字符串转化为 unicode
 */
function converCharToUnicode(theString) {
    var unicodeString = '';
    for (var i = 0; i < theString.length; i++) {
        var theUnicode = theString.charCodeAt(i).toString(16).toUpperCase();
        while (theUnicode.length < 4) {
            theUnicode = '0' + theUnicode;
        }
        theUnicode = '\\u' + theUnicode;
        unicodeString += theUnicode;
    }
    return unicodeString;
}

/**
* unicode转化为字符串
 */
function convertUnicodeToChar(str) {
    str = eval("'" + str + "'");
    return str;
}
function dec2hex(textString) {
    return (textString + 0).toString(16).toUpperCase();
}

// converts a single hex number to a character
// note that no checking is performed to ensure that this is just a hex number, eg. no spaces etc
// hex: string, the hex codepoint to be converted
function hex2char(hex) {
    var result = '';
    var n = parseInt(hex, 16);
    if (n <= 0xFFFF) { result += String.fromCharCode(n); }
    else if (n <= 0x10FFFF) {
        n -= 0x10000
        result += String.fromCharCode(0xD800 | (n >> 10)) + String.fromCharCode(0xDC00 | (n & 0x3FF));
    }
    else { result += 'hex2Char error: Code point out of range: ' + dec2hex(n); }
    return result;
}

/**
 * 生成一个guid标识
*如：e6ba7194-0d69-4499-bd36-710dc9c08a70
 * @returns {} 
 */
function generateUUID() {

    function randCode() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }

    var guid = (randCode() + randCode() + "-" + randCode() + "-4" + randCode().substr(0, 3) + "-" + randCode() + "-" + randCode() + randCode() + randCode()).toLowerCase();
    return guid;
}

/*获取时间戳*/
function getTimeToken(){
 var timestamp=new Date().getTime();
 return timestamp;
 
}

/*!
 * console.js v0.2.0 (https://github.com/yanhaijing/console.js)
 * Copyright 2013 yanhaijing. All Rights Reserved
 * Licensed under MIT (https://github.com/yanhaijing/console.js/blob/master/MIT-LICENSE.txt)
 */
; (function (g) {
    'use strict';
    var _console = g.console || {};
    var methods = ['assert', 'clear', 'count', 'debug', 'dir', 'dirxml', 'exception', 'error', 'group', 'groupCollapsed', 'groupEnd', 'info', 'log', 'profile', 'profileEnd', 'table', 'time', 'timeEnd', 'timeStamp', 'trace', 'warn'];

    var console = { version: '0.2.0' };
    var key;
    for (var i = 0, len = methods.length; i < len; i++) {
        key = methods[i];
        console[key] = function (key) {
            return function () {
                if (typeof _console[key] === 'undefined') {
                    return 0;
                }
                // 添加容错处理
                try {
                    Function.prototype.apply.call(_console[key], _console, arguments);
                } catch (exp) {
                }
            };
        }(key);
    }

    g.console = console;
}(window));