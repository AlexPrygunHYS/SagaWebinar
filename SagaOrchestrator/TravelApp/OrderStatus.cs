using System;
using System.Collections.Generic;
using System.Text;

namespace TravelApp
{
    public enum OrderStatus
    {
        NotOrderedYet,
        OrderRequestIsSent,
        Confirmed,
        CancelRequestIsSent,
        Canceled
    }
}
