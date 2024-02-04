using System;
using System.Threading.Tasks;
using Characters.CharacterBase;
using CollisionPhysics;
using DG.Tweening;
using HelperScripts;
using Managers;
using RootMotion.Demos;
using RootMotion.Dynamics;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Characters.Player
{
    public class Player : Character
    {
        private const int AIUnTouchableLayer = 21;
        
        private static readonly int Block = Animator.StringToHash("Block");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Fatality = Animator.StringToHash("Fatality");
        private static readonly int WalkBackward = Animator.StringToHash("WalkBackward");
        private static readonly int Meia = Animator.StringToHash("Meia");
        private static readonly int Bencao = Animator.StringToHash("Bencao");


        private const float HoldTime = 2f;
        private const float HoldTimeStationary = 0.35f;

        private GameManager _gameManager;
        private CameraManager _cameraManager;
        private UIManager _uiManager;
        
        private Vector2 _fingerDownPosition;
        private Vector2 _fingerUpPosition;
        private Vector2 _pressPosition;

        private float _distance;
        private float _pressTime;
        private float _screenWith;

        private readonly Action[] fatalityMovements = new Action[2];

        private PlayerData _playerData;

        private CapsuleCollider _capsuleCollider;
        
        [Header("Player Specific")] 
        [SerializeField] private Transform fightTr;
        [SerializeField] private Character enemy;
        [SerializeField] private FootCollision rightFootController;
        [SerializeField] private FBIKBoxing fbIK;


        protected override void ReadDataResource()
        {
            base.ReadDataResource();
            _playerData = Resources.Load<PlayerData>("Game/Player/PlayerData");
            Type = Resources.Load<CharacterTypesSO>("Game/CharacterTypes/PayerCharacterTypes");
        }

        protected override void InitVariables()
        {
            base.InitVariables();
            _capsuleCollider = rightPunchController.GetComponent<CapsuleCollider>();
            _screenWith = Screen.width;
            _gameManager = GameManager.Instance;
            _cameraManager = CameraManager.Instance;
            _uiManager = UIManager.Instance;

            // fatalityMovements[0] = MeiaAction;
            fatalityMovements[0] = JumpPunchAction;
            fatalityMovements[1] = BencaoAction;
        }
 
        protected override void Update()
        {
            base.Update();
            
            if (!CanPlay) return;
            PlayerInputControl();
            // PlayerControllerMouse();
            LookAt();
        }

        private void LookAt()
        {
            transform.LookAt(enemy.transform);
        }
        
        private void PlayerInputControl()
        {
            if (Input.touches.Length <= 0) return;
            var touch = Input.touches[0];

            switch (touch)
            {
                case { phase: TouchPhase.Began }:
                    _pressTime = Time.time;
                    _pressPosition = touch.position;
                    _fingerDownPosition = touch.position;
                    
                    break;
                case { phase: TouchPhase.Ended }:
                {
                    _fingerUpPosition = touch.position;
                    if (Vector2.Distance(_fingerDownPosition, _fingerUpPosition) > _playerData.MinSwipeDistance)
                    {
                        var swipeDirection = _fingerUpPosition - _fingerDownPosition;
                        if (!HasAttack)
                        {
                            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                            {
                                if (swipeDirection.x > 0) Hook("LeftPunch", leftPunchController, FullBodyBipedEffector.LeftHand);
                                else Hook("RightPunch", rightPunchController, FullBodyBipedEffector.RightHand);

                                break;
                            }
                            
                            if (swipeDirection.y > 0) UpSwipe();
                            else DownSwipe();
                        }
                        break;
                    }

                    if (Vector2.Distance(_pressPosition, touch.position) < _playerData.MinSwipeDistance &&
                        Time.time - _pressTime < HoldTime && !HasAttack && !HasBlock)
                    {
                        TapTouch(touch);
                        break;
                    }
                    EndBlock();
                    break;
                }
                
                case { phase: TouchPhase.Stationary }:
                {
                    if (Vector2.Distance(_pressPosition, touch.position) < _playerData.MinSwipeDistance &&
                        Time.time - _pressTime > HoldTimeStationary && !HasBlock)
                    {
                        BeginBlock();
                    }

                    break;
                }
            }
        }
        
        private void PlayerControllerMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _pressTime = Time.time;
                _pressPosition = Input.mousePosition;
                _fingerDownPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _fingerUpPosition = Input.mousePosition;
                if (Vector2.Distance(_fingerDownPosition, _fingerUpPosition) > _playerData.MinSwipeDistance)
                {
                    var swipeDirection = _fingerUpPosition - _fingerDownPosition;
                    if (!HasAttack)
                    {
                        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                        {
                            if (swipeDirection.x > 0) Hook("LeftPunch", leftPunchController, FullBodyBipedEffector.LeftHand);
                            else Hook("RightPunch", rightPunchController, FullBodyBipedEffector.RightHand);
                            
                            return;
                        }
                            
                        if (swipeDirection.y > 0) UpSwipe();
                        else DownSwipe();
                    }
                    return;
                }

                if (Vector2.Distance(_pressPosition, Input.mousePosition) < _playerData.MinSwipeDistance &&
                    Time.time - _pressTime < HoldTime && !HasAttack && !HasBlock)
                {
                    TapMouse();
                    return;
                }
                EndBlock();
                return;
            }

            if (Input.GetMouseButton(0))
            {
                if (Vector2.Distance(_pressPosition, Input.mousePosition) < _playerData.MinSwipeDistance &&
                    Time.time - _pressTime > HoldTimeStationary && !HasBlock)
                {
                    BeginBlock();
                }

                return;
            }
        }

        #region Controls

        private void UpSwipe()
        {
            rightPunchController.CanTouch = true;
            _capsuleCollider.height = 1;
            SpecialPartLayerChanger(AIUnTouchableLayer);
            animator.PlayAnimation("UpperCut", ref HasAttack, 0.65f, () =>
            {
                HasAttack = false;
                rightPunchController.CanTouch = false;
                _capsuleCollider.height = 0.2403504f;
                SpecialPartLayerChanger(9);
            });
        }

        private void DownSwipe()
        {
            rightPunchController.CanTouch = true;
            animator.PlayAnimation("BodyShot", ref HasAttack, 0.45f, () =>
            {
                HasAttack = false;
                rightPunchController.CanTouch = false;
            });
        }

        private void TapTouch(Touch touch)
        {
            if (touch.position.x < _screenWith / 2)
            {
                LeftNormal();
            }
                
            if (touch.position.x >= _screenWith / 2)
            {
                RightNormal();
            }
        }
        
        private void TapMouse()
        {
            if (Input.mousePosition.x < _screenWith / 2)
            {
                LeftNormal();
            }
                
            if (Input.mousePosition.x >= _screenWith / 2)
            {
                RightNormal();
            }
        }

        private void Hook(string targetHand, PunchController targetPunch, FullBodyBipedEffector effector)
        {
            fbIK.effector = effector;
            targetPunch.CanTouch = true;
            SpecialPartLayerChanger(AIUnTouchableLayer);
            animator.PlayAnimation(targetHand, ref HasAttack, 0.4375f,  () =>
            {
                HasAttack = false;
                targetPunch.CanTouch = false;
                SpecialPartLayerChanger(9);
                fbIK.effector = FullBodyBipedEffector.Body;
            });
        }

        private void BeginBlock()
        {
            HasBlock = true;
            animator.SetBool(Block, true);
        }

        private void EndBlock()
        {
            HasBlock = false;
            animator.SetBool(Block, false);
        }

        private void RightNormal()
        {
            rightPunchController.CanTouch = true;
            animator.PlayAnimation("NormalPunchR", ref HasAttack, 0.45f,  () =>
            {
                HasAttack = false;
                rightPunchController.CanTouch = false;
            });
        }

        private void LeftNormal()
        {
            leftPunchController.CanTouch = true;
            animator.PlayAnimation("NormalPunchL", ref HasAttack, 0.45f,  () =>
            {
                HasAttack = false;
                leftPunchController.CanTouch = false;
            });
        }

        #endregion

        #region Fatalities

        internal async void FatalityMovement()
        {
            _cameraManager.SetFatalityCamera();
            _uiManager.fatality.SetActive(false);
            await Task.Delay(500);
            fatalityMovements[Random.Range(0, fatalityMovements.Length)].Invoke();
        }

        // private void MeiaAction()
        // {
        //     rightFootController.enabled = true;
        //     
        //     transform.DOMoveZ(-1.1f, 1f)
        //         .SetRelative()
        //         .OnUpdate(() =>
        //         {
        //             if (animator.GetBool(WalkBackward)) return;
        //             animator.SetBool(WalkBackward, true);
        //         })
        //         .OnComplete(Kick);
        // }

        // private async void Kick()
        // {
        //     Time.timeScale = 0.5f;
        //     animator.SetBool(Meia, true);
        //     HasAttack = true;
        //     await Task.Delay(2000);
        //     HasAttack = false;
        //     rightFootController.enabled = false;
        //     Time.timeScale = 1f;
        // }

        private void BencaoAction()
        {
            rightFootController.enabled = true;
            KickBencao();
        }

        private async void KickBencao()
        {
            Time.timeScale = 0.5f;
            animator.SetBool(Bencao, true);
            HasAttack = true;
            await Task.Delay(1367);
            HasAttack = false;
            rightFootController.enabled = false;
            Time.timeScale = 1f;
        }

        private void JumpPunchAction()
        {
            leftPunchController.CanTouch = true;
            transform.DOMoveZ(-1.4f, 1f)
                .SetRelative()
                .OnUpdate(() =>
                {
                    if(animator.GetBool(WalkBackward)) return;
                    animator.SetBool(WalkBackward, true);
                })
                .OnComplete(JumpPunch);
        }
        
        
        private async void JumpPunch()
        {
            Time.timeScale = 0.5f;
            animator.SetBool(Fatality, true);
            HasAttack = true;
            await Task.Delay(2800);
            HasAttack = false;
            leftPunchController.CanTouch = false;
            Time.timeScale = 1f;
            puppetMaster.mode = PuppetMaster.Mode.Kinematic;
        }
        
        #endregion

        public void Movement()
        {
            transform.DOMove(fightTr.position, 1f)
                .OnUpdate(() =>
                {
                    if(animator.GetBool(Walk) || gameObject == null) return;
                    animator.SetBool(Walk, true);
                })
                .OnComplete(() =>
                {
                    animator.SetBool(Walk, false);
                    puppetMaster.mode = PuppetMaster.Mode.Active;
                    GameManager.Instance.SetGameState(GameStates.Game);
                });
        }
        
        protected override void Death()
        {
            if (currentHealth <= 0 && enemy.puppetMaster.state == PuppetMaster.State.Alive)
            {
                puppetMaster.state = PuppetMaster.State.Dead;
                UIManager.LevelEndAction?.Invoke(Type.characterType);
                _gameManager.SetGameState(GameStates.End);
            }
        }

        #region Editor Codes

#if UNITY_EDITOR
        [Button]
        public void SelectPlayerSettings()
        {
            Selection.activeObject = Resources.Load<PlayerData>("Game/Player/PlayerData");
        }
#endif

        #endregion
    }
}