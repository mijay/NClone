using System;
using System.Threading;

namespace mijay.Utils
{
    /// <summary>
    /// Adapter that expose <see cref="IDisposable"/>, but proxies the <see cref="Dispose"/> call to provided delegate.
    /// </summary>
    /// <remarks>
    /// Provided delegate will be called at most once.
    /// </remarks>
    public class DelegateDisposable: IDisposable
    {
        private readonly Action onDispose;
        private int disposed;

        public DelegateDisposable(Action onDispose)
        {
            Guard.AgainstNull(onDispose, "onDispose");
            this.onDispose = onDispose;
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref disposed, 1, 0) == 0)
                onDispose();
        }
    }
}