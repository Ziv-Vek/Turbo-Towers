using System;
using UnityEngine;
using UnityEngine.UI;
using TurboTowers.Turrets.Combat;

namespace TurboTowers.Turrets.Controls
{
    public class InputSwitchHandler : MonoBehaviour
    {
       public Button input1Btn;
       public Button input2Btn;
       public Button aimAssistBtn;
    
       public event Action<InputStyle> OnInputStyleSelect;
    
       public static InputSwitchHandler Instance;
    
       public AimAssist aimAssist;
    
       private void Awake()
       {
          if (Instance == null)
          {
             Instance = this;
             DontDestroyOnLoad(gameObject);
          }
          else
          {
             Destroy(gameObject);
          }
       }
    
       public void SelectInputStyle(Button btn)
       {
          var btnHash = btn.GetHashCode();
    
          if (btnHash == input1Btn.GetHashCode())
          {
             btn.GetComponent<Image>().color = Color.red;
             input2Btn.GetComponent<Image>().color = Color.white;
             OnInputStyleSelect?.Invoke(InputStyle.First);
          } else if (btnHash == input2Btn.GetHashCode())
          {
             btn.GetComponent<Image>().color = Color.red;
             input1Btn.GetComponent<Image>().color = Color.white;
             
             OnInputStyleSelect?.Invoke(InputStyle.Second);
          }
       }
    
       public void ToggleAimAssist()
       {
          aimAssist.enabled = !aimAssist.enabled;
          if (aimAssist.enabled)
           {
               aimAssistBtn.GetComponent<Image>().color = Color.red;
           }
           else
           {
               aimAssistBtn.GetComponent<Image>().color = Color.white;
           }
       }
    }
}
