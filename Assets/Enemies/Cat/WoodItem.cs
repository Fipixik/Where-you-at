using UnityEngine;

public class WoodItem : MonoBehaviour
{
    public StoveLogic stove;

    void OnMouseDown()
    {
        // Posíláme vzkaz kamnům: "Někdo na mě kliknul, pořeš si to v ruce"
        stove.ToggleWood();
    }
}