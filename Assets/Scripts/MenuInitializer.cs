using UnityEngine;

public class MenuInitializer : MonoBehaviour
{
    public ScreenFader screenFader; // pøetáhni sem ScreenFader z Inspectoru

    void Start()
    {
        if (screenFader != null)
            screenFader.ResetFade(); // hned odstraní èernou barvu po naètení menu
    }
}
