using System;

namespace HdProduction.BuildService.Synchronization
{
  public class AppBuildLocker : IDisposable
  {
    private readonly int _key;
    private static readonly KeyLocker<int> Locker = new KeyLocker<int>();

    public AppBuildLocker(int key)
    {
      _key = key;
      Locker.Lock(_key);
    }

    public void Dispose()
    {
      Locker.Release(_key);
    }
  }
}