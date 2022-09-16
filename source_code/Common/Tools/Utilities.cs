using System;
using System.Text;
using System.ServiceModel;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace Dlubal.WS.Common.Tools
{
    public class Utilities
    {
        /// <summary>
        /// This creates message containing description of the exception.
        /// </summary>
        /// <param name="exception">Reference to an instance of the exception.</param>
        /// <returns>Message containing description of the exception.</returns>
        public static string ParseException(Exception exception)
        {
            if (exception == null)
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder(2048);

            // Handles communication exception
            if (exception is CommunicationException communicationException)
            {
                // Handles WEB exception
                if (communicationException.InnerException is WebException webException)
                {
                    stringBuilder.Append(webException.Message);

                    if ((webException.Response != null) && (webException.Response.ContentLength > 0))
                    {
                        Stream responseStream = webException.Response.GetResponseStream();

                        if (responseStream.CanRead)
                        {
                            using (StreamReader streamReader = new StreamReader(responseStream))
                            {
                                stringBuilder.AppendFormat($"{Environment.NewLine}{streamReader.ReadToEnd()}");
                            }
                        }
                        else
                        {
                            responseStream.Close();
                        }
                    }
                }
                else
                {
                    stringBuilder.Append(communicationException.Message);
                }
            }
            else
            {
                // Handles base exception
                stringBuilder.Append(exception.Message);

                if (exception.InnerException != null)
                {
                    stringBuilder.AppendFormat($"{Environment.NewLine}Inner Exception: {exception.InnerException.Message}");
                }
            }

            stringBuilder.AppendFormat($"{Environment.NewLine}{Environment.NewLine}StackTrace:{Environment.NewLine}");
            stringBuilder.Append(GetStackTrace(exception));

            return stringBuilder.ToString();
        }

        // This sets maximal count of stack trace records in error report.
        private const int MAX_STACK_TRACE_LENGTH = 5;

        public static string GetStackTrace(Exception exception = null)
        {
            StringBuilder stringBuilder = new StringBuilder(2048);

            StackTrace stackTrace = (exception == null) ? new StackTrace(2, true) : new StackTrace(exception, true);

            for (int index = 0; index < stackTrace.FrameCount; index++)
            {
                if (index == MAX_STACK_TRACE_LENGTH)
                {
                    break;
                }

                StackFrame frame = stackTrace.GetFrame(index);
                MethodBase method = frame.GetMethod();

                if (method.DeclaringType == null)
                {
                    stringBuilder.AppendFormat($"  at {method.Name}(");
                }
                else
                {
                    stringBuilder.AppendFormat($"  at {method.DeclaringType.FullName}.{method.Name}(");
                }

                bool hasParameters = false;

                foreach (ParameterInfo parameter in method.GetParameters())
                {
                    stringBuilder.AppendFormat($"{parameter.ParameterType.Name} {parameter.Name}, ");
                    hasParameters = true;
                }

                if (hasParameters)
                {
                    stringBuilder.Length -= 2;
                }

                if (!string.IsNullOrEmpty(frame.GetFileName()))
                {
                    stringBuilder.AppendFormat($") in {frame.GetFileName()} --> line: {frame.GetFileLineNumber()}, column: {frame.GetFileColumnNumber()}{Environment.NewLine}");
                }
                else
                {
                    stringBuilder.AppendLine(")");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
