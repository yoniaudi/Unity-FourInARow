using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColliderClickHandler : MonoBehaviour
{
    [SerializeField] private int m_ChosenIndex = -1;
    private static int s_Id = 0;

    private void OnMouseDown()
    {
        string colliderIndx = new string(gameObject.name.Where(char.IsDigit).ToArray());

        //Debug.Log($"{colliderIndx} have been clicked");
        GameManager.s_Instance.PlayTurn(m_ChosenIndex);
    }

    private void Awake()
    {
        m_ChosenIndex = s_Id++;
    }

    private void OnDestroy()
    {
        s_Id--;
    }
}
