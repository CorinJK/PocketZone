using System;
using Logic;
using UnityEngine;

namespace Player
{
    public class PlayerShooting : MonoBehaviour, IProjectile
    {
        private PlayerAim _aim;
        private GameObject[] _bullets;

        public int Damage;
        public int BulletSpeed;
        private int _bulletsPerShot = 1;
        
        public event Action ProjectileCountChanged;

        private int _currentBullet;
        private int _maxBullet;

        public int Current
        {
            get => _currentBullet;
            set
            {
                if (_currentBullet != value)
                {
                    _currentBullet = value;
                    ProjectileCountChanged?.Invoke();
                }
            }
        }

        public int Max
        {
            get => _maxBullet;
            set => _maxBullet = value;
        }
        
        public void Init(PlayerAim aim, GameObject[] bullets)
        {
            _aim = aim;
            _bullets = bullets;
        }

        public void Startup()
        {
            _aim.OnShoot += OnShoot;
        }

        private void OnShoot(PlayerAim.OnShootEvent obj)
        {
            Debug.DrawLine(obj.GunEndPointPosition, obj.ShootPosition + Vector3.up, Color.white, 0.1f);
            
            int index = FindBullet();
            _bullets[index].transform.position = obj.GunEndPointPosition;

            Vector3 direction = ((obj.ShootPosition + Vector3.up) - obj.GunEndPointPosition).normalized;
            _bullets[index].GetComponent<Bullet>().SetDirection(direction, Damage, BulletSpeed);

            SpendBullet();
        }

        private int FindBullet()
        {
            for (int i = 0; i < _bullets.Length; i++)
            {
                if (!_bullets[i].activeInHierarchy)
                    return i;
            }
            return 0;
        }

        private void OnDisable()
        {
            _aim.OnShoot -= OnShoot;
        }

        private void SpendBullet()
        {
            if (Current <= 0)
                return;

            Current -= _bulletsPerShot;
        }
    }
}