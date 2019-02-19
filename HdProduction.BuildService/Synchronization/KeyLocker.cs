using System.Collections.Generic;
using System.Threading;

namespace HdProduction.BuildService.Synchronization
{
  public class KeyLocker<T> where T : struct
  {
    private readonly Dictionary<T, InternalLock> _locks = new Dictionary<T, InternalLock>();
    private readonly object _syncRoot = new object();

    public void Lock(T value)
    {
      InternalLock locker;
      lock (_syncRoot)
    {
      if(!_locks.TryGetValue(value, out locker))
      {
        locker = new InternalLock();
        _locks.Add(value, locker);
      }
      locker.Increment();
    }

    locker.Wait();
  }

  public void Release(T value)
  {
    lock (_syncRoot)
    {
      if (_locks[value].Release() == 0)
      {
        _locks.Remove(value);
      }
    }
  }

  private class InternalLock
  {
    private readonly SemaphoreSlim _ss;
    private int _refCount;

    public InternalLock()
    {
      _ss = new SemaphoreSlim(1, 1);
      _refCount = 0;
    }

    public void Wait()
    {
      _ss.Wait();
    }

    public void Increment()
    {
      ++_refCount;
    }

    public int Release()
    {
      _ss.Release();
      return --_refCount;
    }
  }
}
}