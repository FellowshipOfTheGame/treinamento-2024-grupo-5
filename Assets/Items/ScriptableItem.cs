using Mantega;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class ScriptableItem : ScriptableObject
{
    public new string name;
    public Sprite icon;

    [Header("Prefab")]
    public GameObject prefab;
    public Item item;

    public void Instantiate(Vector3 position, Quaternion rotation)
    {
        Instantiate(prefab, position, rotation);
    }

    private void OnValidate()
    {
        if(item == null && prefab != null)
        {
            if(!Generics.FamilyTryGetComponent(prefab, out item))
            {
                Debug.LogAssertion("Prefab must have a Item component");
            }
        }
    }
}
