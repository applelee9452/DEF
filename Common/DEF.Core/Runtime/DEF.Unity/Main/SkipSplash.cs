#if DEF_CLIENT

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

[Preserve]
public class SkipSplash
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void Run()
    {
        Task.Run(() =>
        {
            SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
        });
    }
}

#endif