using nng.Native;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace nng
{
    using static nng.Native.Defines;
    using static nng.Native.Protocols.UnsafeNativeMethods;

    /// <summary>
    /// Respondent version 0 socket for survey protocol
    /// </summary>
    public class RespondentSocket : NngSocket, IRespondentSocket
    {
        /// <summary>
        /// Create a respondent socket
        /// </summary>
        /// <returns>The open.</returns>
        public static NngResult<IRespondentSocket> Open()
        {
            int res = nng_respondent0_open(out var socket);
            if (res != NNG_OK)
            {
                return NngResult<IRespondentSocket>.Fail(res);
            }
            return NngResult<IRespondentSocket>.Ok(new RespondentSocket { NativeNngStruct = socket });
        }

        private RespondentSocket() { }
    }

    /// <summary>
    /// Surveyor version 0 socket for survey protocol
    /// </summary>
    public class SurveyorSocket : NngSocket, ISurveyorSocket
    {
        /// <summary>
        /// Create a surveyor socket
        /// </summary>
        /// <returns>The open.</returns>
        public static NngResult<ISurveyorSocket> Open()
        {
            int res = nng_surveyor0_open(out var socket);
            if (res != NNG_OK)
            {
                return NngResult<ISurveyorSocket>.Fail(res);
            }
            return NngResult<ISurveyorSocket>.Ok(new SurveyorSocket { NativeNngStruct = socket });
        }

        private SurveyorSocket() { }
    }
}