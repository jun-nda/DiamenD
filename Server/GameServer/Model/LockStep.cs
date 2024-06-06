using System.Diagnostics;

namespace GameServer.Model;

public class LockStep
{
    private float _remainTime = 0f;
    private const float DeltaTime = 0.03f;
    private float _accmulatedTime = 0f;
    public void Start()
    {
        // while (true)
        // {
        //     _remainTime
        //     if (_accmulatedTime)
        //     _accmulatedTime += DeltaTime;
        //
        // }
    }

    // private void doUpdate()
    // {
    //     _remainTime;
    // }
    
    
    public void Destroy()
    {
        _remainTime = 0f;
        _accmulatedTime = 0f;
    } 
}