using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetOperations.Domain.Exceptions;

public sealed class InvalidMaintenanceStateException : Exception
{
    public InvalidMaintenanceStateException(string message) : base(message) { }
}

