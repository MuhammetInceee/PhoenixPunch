using CollisionPhysics;
using Managers;
using RootMotion.Dynamics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


namespace Characters.CharacterBase
{
    public class Character : MonoBehaviour
    {
        protected float currentHealth;
        private CharacterBaseData BaseData;
        private float _healthRatio;
        
        internal bool HasAttack;
        internal bool HasBlock;
        internal bool HasHurry;
        
        internal CharacterTypesSO Type;

        [Header("Character Values"), Space]
        public PuppetMaster puppetMaster;
        public Animator animator;
        [SerializeField] protected Slider healthSlider;
        [SerializeField] protected GameObject[] untouchableParts;
        public PunchController rightPunchController;
        public PunchController leftPunchController;

        protected bool CanPlay => puppetMaster.state == PuppetMaster.State.Alive &&
                                  GameManager.Instance.GetGameState() == GameStates.Game;
        
        protected virtual void Awake()
        {
            ReadDataResource();
            InitVariables();
        }

        protected virtual void Update() { }

        protected virtual void ReadDataResource()
        {
            BaseData = Resources.Load<CharacterBaseData>("Game/CharacterBase/CharacterBaseData");
        }

        protected virtual void InitVariables()
        {
            currentHealth = BaseData.StartHealth;
            healthSlider.maxValue = BaseData.StartHealth;
            healthSlider.minValue = 0;
            rightPunchController.enabled = true;
            leftPunchController.enabled = true;
        }
        
        public void GetDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= BaseData.StartHealth / 2) HasHurry = true;

            UIManager.HealthUIUpdateAction?.Invoke(healthSlider, damage);
            
            Death();
        }

        protected void SpecialPartLayerChanger(int targetLayer)
        {
            foreach (var part in untouchableParts)
            {
                part.layer = targetLayer;
            }
        }

        protected virtual void Death() { }

        internal float GetHealthRatio()
        {
            _healthRatio = currentHealth / BaseData.StartHealth;
            _healthRatio = Mathf.Clamp01(_healthRatio);
            
            return _healthRatio;
        }
    }
}
