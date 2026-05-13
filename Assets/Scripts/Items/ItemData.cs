using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField][Range(0,1)] private float hardness;
    [SerializeField] private float weight;
    [SerializeField] private float handiness;

    public float Hardness() => hardness;
    public float Weight() => weight;
    public float Handiness() => handiness;
}
