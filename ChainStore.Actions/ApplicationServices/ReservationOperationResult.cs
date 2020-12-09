using System;
using System.Collections.Generic;
using System.Text;

namespace ChainStore.Actions.ApplicationServices
{
    public enum ReservationOperationResult
    {
        Success,
        LimitExceeded,
        InvalidParameters,
        Fail
    }
}
