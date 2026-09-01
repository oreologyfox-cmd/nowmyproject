using UnityEngine;
using UnityEngine.InputSystem;

public class ModeChecker : MonoBehaviour
{
    // インスペクターでModechangeがアタッチされたオブジェクトを指定
    [SerializeField] private Modechange modeChangeScript;

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
    UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            CheckCurrentMode();
        }
    }

    private void CheckCurrentMode()
    {
        if (modeChangeScript == null) return;

        // enumの値を条件分岐で判定
        switch (modeChangeScript.CurrentMode)
        {
            case Modechange.TouchMode.Hand:
                Debug.Log("現在は【手モード】です。");
                break;

            case Modechange.TouchMode.Mouth:
                Debug.Log("現在は【口モード】です。");
                break;

            case Modechange.TouchMode.Tin:
                Debug.Log("現在は【男性モード】です。");
                break;
        }
    }
}