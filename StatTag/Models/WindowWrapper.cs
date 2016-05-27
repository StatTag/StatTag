using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StatTag.Models
{
    /// <summary>
    /// Taken from: http://ryanfarley.com/blog/archive/2004/03/23/465.aspx
    /// </summary>
    public class WindowWrapper : IWin32Window
    {
        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private readonly IntPtr _hwnd;
    }
}
