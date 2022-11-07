using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
    [SerializeField]
    private float[] _parameters ;

    private void OnDrawGizmosSelected()
    {
        
        if (_parameters.Length == 0) return ;

        foreach (float parameter in _parameters)
        {
            Gizmos.DrawWireSphere(transform.position, parameter);
        }
    }
}
