
using UnityEngine.EventSystems;


    
public class CustomStandaloneInputModule : StandaloneInputModule
{

       
    public PointerEventData GetPointerData(int a_mousePointerId)
    {
        return m_PointerData[a_mousePointerId];
    }

        
    public PointerEventData PointerEventDataLeft()
    {
        return m_PointerData[kMouseLeftId];
    }

}
