using DefaultNamespace;
using Data;
using Data.Position;
using Services.Input;
using Services.PersistentProgress;
using Services.SaveLoad;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerMovement : MonoBehaviour, ISavedProgress
    {
        private float _movementSpeed = 4f;
        
        private Rigidbody2D _rb;
        private Collider2D _collider;
        private Transform _mesh;
        
        private IInputService _inputService;
        private ISaveLoadService _saveLoadService;

        public void Init(IInputService inputService, ISaveLoadService saveLoadService, Rigidbody2D rb, Collider2D collider, Transform mesh, float movementSpeed)
        {
            _inputService = inputService;
            _saveLoadService = saveLoadService;
            _rb = rb;
            _collider = collider;
            _mesh = mesh;
            _movementSpeed = movementSpeed;
        }

        public void FixedTick(bool canMove)
        {
            if (!canMove)
            {
                return;
            }
            
            Vector2 movementVector;
            
            if (_inputService.Axis.sqrMagnitude > Constants.Epsilon)
            {
                movementVector = _inputService.Axis;
                
                if (_inputService.Axis.x < Constants.Epsilon)
                {
                    ChangeDirection(-1);
                }
                else
                {
                    ChangeDirection(1);
                }
            }
            else
            {
                movementVector = Vector2.zero;
            }
            
            _rb.velocity = _movementSpeed * movementVector.normalized;
        }

        private void ChangeDirection(float x)
        {
            _mesh.transform.localScale = new Vector3(x, 1, 1);
        }

        public void SaveProgress(PlayerProgress progress)
        {
            progress.WorldData.PositionOnLevel = new PositionOnLevel(CurrentLevel(), transform.position.AsVectorData());
        }

        public void LoadProgress(PlayerProgress progress)
        {
            if (CurrentLevel() == progress.WorldData.PositionOnLevel.Level)
            {
                Vector3Data savedPosition = progress.WorldData.PositionOnLevel.Position;
                if (savedPosition != null)
                    Warp(to: savedPosition);
            }
        }

        private void Warp(Vector3Data to)
        {
            _collider.enabled = false;
            transform.position = to.AsUnityVector();
            _collider.enabled = true;
        }

        private static string CurrentLevel()
        {
            return SceneManager.GetActiveScene().name;
        }

        private void OnDisable()
        {
            _saveLoadService.SaveProgress();
            Debug.Log("Progress Saved.");
        }
    }
}