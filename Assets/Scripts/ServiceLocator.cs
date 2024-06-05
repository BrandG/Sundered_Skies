using System;
using UnityEngine;

/**
 * <summary>
 * The ServiceLocator class is a singleton that provides a global point of access to the services in the game.
 * It is used to decouple the classes from each other and to make the code more modular and testable.
 * </summary>
 */
public class ServiceLocator
{
    private static ServiceLocator instance;
    public PlayerController PlayerController { get ; private set; }
    public FollowCamera FollowCamera { get ; private set; }
    public Terrain Terrain { get; private set; }
    public GameObject Target { get; private set; }
    public Configurator Configurator { get; private set; }

    public ServiceLocator()
    {
    }

    public static ServiceLocator Instance
    {
        get
        {
            instance ??= new ServiceLocator();
            return instance;
        }
    }

    public void RegisterPlayerController(PlayerController playerController)
    {
        PlayerController = playerController;
    }

    public void RegisterFollowCamera(FollowCamera followCamera )
    {
        FollowCamera = followCamera;
    }

    public void RegisterTerrain(Terrain terrain)
    {
        Terrain = terrain;
    }

    public void RegisterTerrain(SSTerrain terrain)
    {
        Terrain = terrain.terrainObj;
    }

    public void RegisterTarget(GameObject target)
    {
        Target = target;
    }

    public void RegisterConfigurator(Configurator configurator)
    {
        Configurator = configurator;
    }
}