// Vytvoø nový soubor: BaseNightManager.cs
using UnityEngine;

public class BaseNightManager : MonoBehaviour
{
    // FINAL GAME OVER FUNKCE: Musí být virtual, aby ji mohl každý Manager pøepsat.
    public virtual void GameOver(string killerName)
    {
        Debug.LogError($"Chyba! BaseNightManager.GameOver() volán. Nepøítel {killerName} nenašel konkrétní Manager pro spuštìní Game Over.");
    }
}