using UnityEngine;

namespace Managers
{
    public class FanInstantiateManager : MonoBehaviour
    {
        [SerializeField] private Transform ringTr;
        [SerializeField] private Transform[] holders;
        [SerializeField] private GameObject[] fanTypes;

        private void Awake() => InstantiateFans();

        private void InstantiateFans()
        {
            foreach (var t in holders)
            {
                var randomType = fanTypes[Random.Range(0, fanTypes.Length)];
                var fan = Instantiate(randomType, t.position, Quaternion.Euler(0,0,0), t);
                fan.transform.LookAt(ringTr);
            }
        }
    }
}
