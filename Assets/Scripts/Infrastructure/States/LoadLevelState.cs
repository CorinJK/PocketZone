using Cinemachine;
using Logic;
using Services;
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
        private ILootSpawnSystem _lootSpawn;

        public LoadLevelState(GameStateMachine stateMachine, SceneLoader sceneLoader, LoadingCurtain curtain)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _curtain = curtain;
            
            _mutantSpawn = SystemsManager.Get<IMutantSpawnSystem>();
            _playerSpawn = SystemsManager.Get<IPlayerSpawnSystem>();
            _hudSpawn = SystemsManager.Get<IHUDSpawnSystem>();
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
            _hudSpawn.InitHUD();
            _lootSpawn.InitCollectibleHolder();
            
            CameraFollow(_playerSpawn.GetPlayer().transform);
        }

        private static void CameraFollow(Transform player)
        {
            Camera.main.GetComponentInChildren<CinemachineVirtualCamera>().Follow = player;
        }
    }
}