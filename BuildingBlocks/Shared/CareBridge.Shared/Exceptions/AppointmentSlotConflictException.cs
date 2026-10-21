using System;
using System.Collections.Generic;
using System.Text;

namespace CareBridge.Shared.Exceptions
{
    public sealed class AppointmentSlotConflictException:Exception
    {
        public AppointmentSlotConflictException(string message) : base(message)
        {

        }
    }
}
