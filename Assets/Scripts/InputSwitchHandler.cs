using System;
using UnityEngine;
using UnityEngine.UI;

public class InputSwitchHandler : MonoBehaviour
{
   public Button input1Btn;
   public Button input2Btn;
   public Button input3Btn;

   public Action<int> onInputStyleSelect;

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
         input3Btn.GetComponent<Image>().color = Color.white;
         
         onInputStyleSelect?.Invoke(1);
      } else if (btnHash == input2Btn.GetHashCode())
      {
         btn.GetComponent<Image>().color = Color.red;
         input1Btn.GetComponent<Image>().color = Color.white;
         input3Btn.GetComponent<Image>().color = Color.white;
         
         onInputStyleSelect?.Invoke(2);
      }else if (btnHash == input3Btn.GetHashCode())
      {
         btn.GetComponent<Image>().color = Color.red;
         input1Btn.GetComponent<Image>().color = Color.white;
         input2Btn.GetComponent<Image>().color = Color.white;
         
         onInputStyleSelect?.Invoke(3);
      }
   }
}
