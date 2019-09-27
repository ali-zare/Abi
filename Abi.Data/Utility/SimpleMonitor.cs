using System;

namespace Abi.Data
{
    internal class SimpleMonitor : IDisposable
    {
        public void Enter()
        {
            ++_busyCount;
        }
        public void Dispose()
        {
            --_busyCount;
        }

        public bool Busy { get { return _busyCount > 0; } }

        int _busyCount;
    }
}
