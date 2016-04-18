using System;
using System.Windows.Input;

namespace Guinunit.Utilities
{
    public class CursorRestoreHandle : IDisposable
    {
        private readonly Cursor savedCursor;
        private bool isRestored;

        public CursorRestoreHandle(Cursor cursor)
        {
            savedCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = cursor;
        }

        public void Restore()
        {
            if (isRestored)
                return;

            Mouse.OverrideCursor = savedCursor;

            isRestored = true;
        }

        public void Dispose()
        {
            Restore();
        }
    }
}
