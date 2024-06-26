using Cinemachine;
using Logic;
using Services;
using Services.Inventory;
using Services.Spawner;
using UnityEngine;

namespace Infrastructure.States
{
    public class LoadLevelState : IPayloadedState<string>, ISystem
    {
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _curtain;
        
        private IMutantSpawnSystem _mutantSpawn;
        private IPlayerSpawnSystem _playerSpawn;
        private IHUDSpawnSystem _hudSpawn;
        private IInventorySystem _inventory;
        private ILootSpawnSystem _lootSpawn;

        public LoadLevelState(GameStateMachine stateMachine, SceneLoader sceneLoader, LoadingCurtain curtain)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _curtain = curtain;
            
            _mutantSpawn = SystemsManager.Get<IMutantSpawnSystem>();
            _playerSpawn = SystemsManager.Get<IPlayerSpawnSystem>();
            _hudSpawn = SystemsManager.Get<IHUDSpawnSystem>();
            _inventory = SystemsManager.Get<IInventorySystem>();
            _lootSpawn = SystemsManager.Get<ILootSpawnSystem>();
        }

        public void Enter(string sceneName)
        {
            _curtain.Show();
            _sceneLoader.Load(sceneName, OnLoaded);
        }

        public void Exit()
        {
            _curtain.Hide();
        }

        private void OnLoaded()
        {
            InitGameWorld();
            _stateMachine.Enter<GameLoopState>();
        }

        private void InitGameWorld()
        {
            _playerSpawn.InitPlayer();
            _mutantSpawn.InitSpawner();
            _lootSpawn.InitLootHolder();
            _hudSpawn.InitHUD();
            _inventory.InformUpdateViewInventory();
            
            CameraFollow(_playerSpawn.GetPlayer().transform);
        }

        private static void CameraFollow(Transform player)
        {
            Camera.main.GetComponentInChildren<CinemachineVirtualCamera>().Follow = player;
        }
    }
}