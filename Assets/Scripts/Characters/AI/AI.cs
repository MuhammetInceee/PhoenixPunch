using System;
using System.Threading.Tasks;
using HelperScripts;
using Managers;
using RootMotion.Dynamics;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Characters.AI
{
    public class AI : CharacterBase.Character
    {
        private const int PlayerUnTouchableLayer = 22;

        private static readonly int BlockHash = Animator.StringToHash("Block");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Tired = Animator.StringToHash("Tired");
        private readonly Action[] _attackActionsArr = new Action[6];

        private bool isFatalityReady = false;

        [Header("Enemy Specific")] 
        [SerializeField] private Player.Player player;
        [SerializeField] private NavMeshAgent agent;

        private AIData _data;
        private CameraManager _cameraManager;
        private GameManager _gameManager;

        //Run
        private Vector3 _destination;
        private float _lastRunTime;

        //Attack
        private float _lastAttackTime;

        //Block 
        private bool _isBlocking;
        private float _lastBlockTime;
        private float _nextBlockTime;

        [SerializeField] private bool stuck;

        protected override void ReadDataResource()
        {
            base.ReadDataResource();
            _data = Resources.Load<AIData>("Game/AI/AIData");
            Type = Resources.Load<CharacterTypesSO>("Game/CharacterTypes/AICharacterTypes");
        }

        protected override void InitVariables()
        {
            base.InitVariables();

            #region Actions Init

            _attackActionsArr[0] = RightPunch;
            _attackActionsArr[1] = LeftPunch;
            _attackActionsArr[2] = NormalPunchRight;
            _attackActionsArr[3] = UpperCut;
            _attackActionsArr[4] = BodyShot;
            _attackActionsArr[5] = NormalPunchLeft;

            #endregion

            agent.speed = _data.AISpeed;
            _cameraManager = CameraManager.Instance;
            _gameManager = GameManager.Instance;
        }

        protected override void Update()
        {
            base.Update();

            if (_gameManager.GetGameState() != GameStates.Game) return;

            LookAt();

            if (stuck) return;
            Attack();
            //Escape();
            Block();
        }

        private void LookAt()
        {
            transform.LookAt(player.transform.position);
        }

        private void Attack()
        {
            var interval = HasHurry ? _data.HurryAttackInterval : _data.NormalAttackInterval;
            if (!(Time.time - _lastAttackTime > interval) || HasBlock) return;
            GetRandomAttack();
            _lastAttackTime = Time.time;
        }

        private void Escape()
        {
            if(isFatalityReady) return;
            animator.SetBool(Walk, agent.velocity.magnitude > 0.2f);

            float interval = HasHurry ? _data.HurryRunInterval : _data.NormalRunInterval;

            if (Time.time - _lastRunTime > interval && !HasBlock && !HasAttack)
            {
                SetNewDestination();
                _lastRunTime = Time.time;
            }
        }
        
         private void SetNewDestination()
         {
             Vector3 randomDirection = Random.insideUnitSphere * 5.0f;
             randomDirection += transform.position;
             NavMesh.SamplePosition(randomDirection, out var navMeshHit, 5.0f, NavMesh.AllAreas);
             _destination = navMeshHit.position;
        
             agent.SetDestination(_destination);
         }

        private void Block()
        {
            if (player.HasAttack && Time.time - _lastBlockTime >= _nextBlockTime)
            {
                if (Random.value <= (_data.BlockChancePercentage / 100))
                {
                    BlockBegin();
                    _isBlocking = true;
                }

                _nextBlockTime = Random.Range(1f, 3f);
                _lastBlockTime = Time.time;
            }
            else if (_isBlocking && Time.time - _lastBlockTime >= _data.BlockDuration)
            {
                BlockEnd();
                _isBlocking = false;
            }
        }

        private void GetRandomAttack()
        {
            _attackActionsArr?[Random.Range(0, _attackActionsArr.Length)]?.Invoke();
        }

        #region AttackActions

        private void UpperCut()
        {
            rightPunchController.CanTouch = true;
            animator.PlayAnimation("UpperCut", ref HasAttack, 0.65f, () =>
            {
                HasAttack = false;
                rightPunchController.CanTouch = false;
            });
        }


        public void BodyShot()
        {
            rightPunchController.CanTouch = true;
            animator.PlayAnimation("BodyShot", ref HasAttack, 0.45f, () =>
            {
                HasAttack = false;
                rightPunchController.CanTouch = false;
            });
        }

        private void RightPunch()
        {
            rightPunchController.CanTouch = true;
            SpecialPartLayerChanger(PlayerUnTouchableLayer);
            animator.PlayAnimation("RightPunch", ref HasAttack, 0.4375f, () =>
            {
                HasAttack = false;
                rightPunchController.CanTouch = false;
                SpecialPartLayerChanger(9);
            });
        }

        private void LeftPunch()
        {
            leftPunchController.CanTouch = true;
            SpecialPartLayerChanger(PlayerUnTouchableLayer);
            animator.PlayAnimation("LeftPunch", ref HasAttack, 0.4375f, () =>
            {
                HasAttack = false;
                leftPunchController.CanTouch = false;
                SpecialPartLayerChanger(9);
            });
        }

        private void NormalPunchRight()
        {
            rightPunchController.CanTouch = true;
            animator.PlayAnimation("NormalPunchR", ref HasAttack, 0.45f, () =>
            {
                HasAttack = false;
                rightPunchController.CanTouch = false;
            });
        }

        private void NormalPunchLeft()
        {
            leftPunchController.CanTouch = true;
            animator.PlayAnimation("NormalPunchL", ref HasAttack, 0.45f, () =>
            {
                HasAttack = false;
                leftPunchController.CanTouch = false;
            });
        }

        #endregion

        #region BlockActions

        private void BlockBegin()
        {
            HasBlock = true;
            animator.SetBool(BlockHash, true);
        }

        private void BlockEnd()
        {
            HasBlock = false;
            animator.SetBool(BlockHash, false);
        }

        #endregion

        protected override async void Death()
        {
            if (currentHealth <= 0 && player.puppetMaster.state == PuppetMaster.State.Alive && !isFatalityReady)
            {
                player.rightPunchController.enabled = false;
                player.leftPunchController.enabled = false;

                animator.SetBool(Tired, true);
                isFatalityReady = true;
                _cameraManager.SetCameraDuration(0.5f);
                GameManager.Instance.SetGameState(GameStates.Wait);

                await Task.Delay(500); //TODO

                GameManager.Instance.SetGameState(GameStates.Fatality);
                UIManager.Instance.fatality.SetActive(true);
            }

            else if (isFatalityReady)
            {
                puppetMaster.state = PuppetMaster.State.Dead;
                GameManager.Instance.SetGameState(GameStates.End);

                FatalEnd();
            }
        }

        private async void FatalEnd()
        {
            await Task.Delay(1200);
            _cameraManager.SetHappyCamera();
            LevelEndVisualizer();
        }

        private async void LevelEndVisualizer()
        {
            await Task.Delay(2500);
            UIManager.LevelEndAction?.Invoke(Type.characterType);
            ParticleManager.ConfettiParticleAction?.Invoke();
        }

        #region Editor Codes

#if UNITY_EDITOR
        [Button]
        public void SelectAiSettings()
        {
            Selection.activeObject = Resources.Load<AIData>("Game/AI/AIData");
        }
#endif

        #endregion
    }
}