using System;
using UnityEngine;
using UnityEngine.UI;

public class InputSwitchHandler : MonoBehaviour
{
   public Button input1Btn;
   public Button input2Btn;
   public Button aimAssistBtn;

   public event Action<int> OnInputStyleSelect;

   public static InputSwitchHandler Instance;

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
         OnInputStyleSelect?.Invoke(1);
      } else if (btnHash == input2Btn.GetHashCode())
      {
         btn.GetComponent<Image>().color = Color.red;
         input1Btn.GetComponent<Image>().color = Color.white;
         
         OnInputStyleSelect?.Invoke(2);
      }
   }

   public void ToggleAimAssist()
   {
      //TODO: add aim assist system
   }
}
