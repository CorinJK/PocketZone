using Data;
using Services.PersistentProgress;
using Services.SaveLoad;

namespace Infrastructure.States
{
    public class LoadProgressState : IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly IPersistentProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;

        public LoadProgressState(GameStateMachine stateMachine, IPersistentProgressService progressService, ISaveLoadService saveLoadService)
        {
            _stateMachine = stateMachine;
            _progressService = progressService;
            _saveLoadService = saveLoadService;
        }

        public void Enter()
        {
            LoadProgressOrInitNew();
            _stateMachine.Enter<LoadLevelState, string>(_progressService.Progress.WorldData.PositionOnLevel.Level);
        }

        public void Exit()
        {
            
        }
        
        private void LoadProgressOrInitNew()
        {
            _progressService.Progress = _saveLoadService.LoadProgress() ?? NewProgress();
        }

        private PlayerProgress NewProgress()
        {
            PlayerProgress playerProgress = new PlayerProgress(initialLevel: "Main");

            playerProgress.Health.MaxHP = 50;
            playerProgress.Health.ResetHP();

            playerProgress.Stats.Damage = 10f;
            playerProgress.Stats.Speed = 15f;
            playerProgress.Stats.DamageRadius = 4.5f;
            
            return playerProgress;
        }
    }
}