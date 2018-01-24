


// 对Date的扩展，将 Date 转化为指定格式的String
// 月(M)、日(d)、小时(H)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
// 例子： 
// (new Date()).Format("yyyy-MM-dd HH:mm:ss.S") ==> 2006-07-02 08:09:04.423 
// (new Date()).Format("yyyy-M-d H:m:s.S")      ==> 2006-7-2 8:9:4.18 
Date.prototype.Format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "H+": this.getHours(), //小时 --------*********仅支持24小时制**************
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
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
Array.prototype.trueForAll = function (predicate) {
    var results = new Array();
    for (var i = 0; i < this.length; i++) {
        if (!predicate(this[i])) {
            return false;
        }
    }
    return true;
};



var List = (function () {
    function List(list) {
        this.list = [];
        this.list = list || [];
    }
    Object.defineProperty(List.prototype, "length", {
        get: function () {
            return this.list.length;
        },
        enumerable: true,
        configurable: true
    });
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
                this.list.splice(i,1);//移除这个索引的元素
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
function isNumber(value) { return typeof value === 'number'; }


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


function isNullOrEmpty(str) {
    if (str && str.length > 0) {
        return false;
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


function isImg(str) {
    var objReg = new RegExp("[.]+(jpg|jpeg|swf|gif)$", "gi");
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

function showInfoMessage(message) {
    if (WebFox.Dialog) {
        WebFox.Dialog.Alert({ title: "提示信息", msg: message });
    } else {
        alert(message);
    }
}


function showErrorMessage(message) {
    if (WebFox.Dialog) {
        WebFox.Dialog.Alert({ title: "错误信息", msg: message });
    } else {
        alert(message);
    }
}

function showSuccessMessage(message) {
    if (WebFox.Dialog) {
        WebFox.Dialog.Alert({ title: "操作成功", msg: message });
    } else {
        alert(message);
    }
}

function showHtmlDialogMessage(htmlContainerId, useZheZhao) {

    //判断当前展示的窗口id  元素 是否已经显示，如果已经显示 那么返回
    var domOfContainer = $("#" + htmlContainerId);
    if (!domOfContainer.is(':hidden')) {
        return;
    }

    var isUseLayer = false;
    if (useZheZhao) {
        isUseLayer = useZheZhao;
    }
    if (WebFox.Dialog) {
        var option = {
            id: htmlContainerId,
            isUseZheZhao: isUseLayer
        };
        WebFox.Dialog.Html(option);
    } else {
        alert(message);
    }
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
    // converts a string containing U+... escapes to a string of characters
    // str: string, the input

    // first convert the 6 digit escapes to characters
    str = str.replace(/[Uu]\+10([A-Fa-f0-9]{4})/g,
					function (matchstr, parens) {
					    return hex2char('10' + parens);
					}
						);
    // next convert up to 5 digit escapes to characters
    str = str.replace(/[Uu]\+([A-Fa-f0-9]{1,5})/g,
					function (matchstr, parens) {
					    return hex2char(parens);
					}
						);
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

function getFullHoseName() {
    var proto = window.location.protocol + "//";
    var port = window.location.port;
    var domain = window.location.hostname;
    if (port) {
        port = ":"+port;
    } else {
        port = "";
    }
    var fullName = proto + domain+port  ;
    return fullName;
}
function goToHome() {
    var homeUrl = getFullHoseName();
    window.location.href=homeUrl;
}
