﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Core.Models
{
    /// <summary>
    /// Exceptions that are purposely formatted to be displayed to the user.  If an exception
    /// is of this type, it should be displayed.
    /// </summary>
    public class StatTagUserException : Exception
    {
        public StatTagUserException(string message) : base(message)
        {
        }

        public StatTagUserException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}
