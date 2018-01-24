using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartClient.Common
{
    public class BusinessException:Exception
    {
        public BusinessException(string msg):base(msg)
        {
        }

        public BusinessException(string msgFormat,params object[] agrs) :base(string.Format(msgFormat, agrs))
        {
        }
    }
}