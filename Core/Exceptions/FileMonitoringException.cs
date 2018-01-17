using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Exceptions
{
    public class FileMonitoringException : StatTagUserException
    {
        public FileMonitoringException(string fileName, Exception innerException)
            : base(string.Format("StatTag is unable to monitor the file {0} for changes.", fileName), innerException)
        {
        }
    }
}
