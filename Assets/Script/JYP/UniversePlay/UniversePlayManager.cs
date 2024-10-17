using System.Collections.Generic;
using UnityEngine;

public class UniversePlayManager : MonoBehaviour
{
    private static UniversePlayManager instance;

    [SerializeField]
    private PlayBackgroundManager playBackgroundManager;

    public PlayBackgroundManager BackgroundManager => playBackgroundManager;

    public static UniversePlayManager Instance
    {
        get
        {
            if (instance == null)
            {
                var universePrefab = Resources.Load<GameObject>("UniversePlay/UniversePlayManager");
                var go = Instantiate(universePrefab);
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        TestingByKey();
    }

    #region Test

    private void TestingByKey()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            var portalList1 = new List<PortalData>()
            {
                new PortalData()
                {
                    position = new Vector3(0, 0, 0),
                    targetBackgroundId = 1,
                },
            };

            var portalList2 = new List<PortalData>()
            {
                new PortalData()
                {
                    position = new Vector3(0, 0, 0),
                    targetBackgroundId = 0,
                },
            };
            var backgroundList = new List<BackgroundPartData>()
            {
                new BackgroundPartData()
                {
                    id = 0,
                    Name = "Town 0",
                    Type = EBackgroundPartType.Town,
                    universeId = 0,
                    portalList = portalList1,
                },
                new BackgroundPartData()
                {
                    id = 1,
                    Name = "Dungeon 0",
                    Type = EBackgroundPartType.Dungeon,
                    universeId = 0,
                    portalList = portalList2,
                },
            };

            var universe = new UniverseData()
            {
                id = 0,
                name = "Universe 0",
                startBackground = backgroundList[0],
            };

            playBackgroundManager.Init(universe, backgroundList);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playBackgroundManager.MoveTo(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playBackgroundManager.MoveTo(1);
        }
    }

    #endregion
}