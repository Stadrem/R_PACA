using System;
using System.Collections;

public class MockServer
{
    private static MockServer _instance;

    public static MockServer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MockServer();
            }

            return _instance;
        }
    }

    private MockServer()
    {
        // Constructor
    }

    public IEnumerator Get<T>(Action<T> callback)
    {
        yield return null;
        callback(default);
    }

    public IEnumerator PostDiceRequired(Action<bool> callback)
    {
        yield return null;
        callback(true);
    }
    
}