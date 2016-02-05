using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.IE.Qwiq.Exceptions
{
    internal abstract class VssExceptionMapper : IExceptionMapper
    {
        private const string ErrorCode = "ErrorCode";
        private readonly Regex _extractTfsErrorCodeRegex = new Regex("TF(?<" + ErrorCode + ">[0-9]*)", RegexOptions.Compiled);
        private readonly int[] _handledErrorCodes;
        private readonly Func<string, Exception, Exception> _newExceptionCreator;

        protected VssExceptionMapper(int[] handledErrorCodes, Func<string, Exception, Exception> newExceptionCreator)
        {
            _handledErrorCodes = handledErrorCodes;
            _newExceptionCreator = newExceptionCreator;
        }

        public Exception Map(Exception ex)
        {
            var vssException = ex as VssException;

            if (vssException == null)
            {
                return null;
            }

            var regexMatch = _extractTfsErrorCodeRegex.Match(ex.Message);
            if (!regexMatch.Success)
            {
                return null;
            }

            var errorCode = int.Parse(regexMatch.Groups[ErrorCode].Value);

            return _handledErrorCodes.Contains(errorCode) ? _newExceptionCreator(vssException.Message, vssException) : null;
        }
    }
}