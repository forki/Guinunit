using System;
using System.Windows.Input;

namespace Guinunit.Utilities
{
    public static class CursorHelper
    {
        public static IDisposable UseWait()
        {
            return new CursorRestoreHandle(Cursors.Wait);
        }
    }
}
