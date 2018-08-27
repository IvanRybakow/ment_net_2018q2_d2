using System;
using Newtonsoft.Json;
using PostSharp.Aspects;

namespace Mentoring.D2.AOP.MergeService
{
    [Serializable]
    public class CustomLogAttribute : OnMethodBoundaryAspect
    {
        private string logMessage;
        public CustomLogAttribute()
        {
            Logger.InitLogger();
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            string parameters;

            try
            {
                parameters = JsonConvert.SerializeObject(args.Arguments);
            }
            catch (Exception)
            {
                parameters = "Not serializable";
            }

            var reflectedType = args.Method.ReflectedType;

            if (reflectedType == null)
                return;

            logMessage = $"{reflectedType.FullName}.{args.Method.Name} - {parameters}";
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            if (args.Method.ReflectedType != Type.GetType("Void"))
            {
                logMessage += $" = {JsonConvert.SerializeObject(args.ReturnValue)}";
            }

            Logger.Log.Info(logMessage);
        }

    }
}
