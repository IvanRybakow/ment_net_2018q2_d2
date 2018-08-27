using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Newtonsoft.Json;

namespace Mentoring.D2.AOP.MergeService
{
    public class LogInterceptor : IInterceptor
    {
        public LogInterceptor()
        {
            Logger.InitLogger();
        }
        public void Intercept(IInvocation invocation)
        {
            string parameters;

            try
            {
                parameters = JsonConvert.SerializeObject(invocation.Arguments);
            }
            catch (Exception)
            {
                parameters = "Not serializable";
            }

            var reflectedType = invocation.Method.ReflectedType;

            if (reflectedType == null)
                return;

            var logMessage = $"{reflectedType.FullName}.{invocation.Method.Name} - {parameters}";

            invocation.Proceed();

            if (invocation.Method.ReturnType != typeof(void))
            {
                logMessage += $" = {JsonConvert.SerializeObject(invocation.ReturnValue)}";
            }

            Logger.Log.Info(logMessage);
        }
    }
}
