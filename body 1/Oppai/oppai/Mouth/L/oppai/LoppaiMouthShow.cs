using UnityEngine;

public class LoppaiMouthShow : MonoBehaviour
{
    // 表示・非表示を切り替えたいオブジェクトをインスペクターで指定
    [SerializeField] private GameObject targetObject;

    void Start()
    {
        // 初期状態は非表示
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }

    // ボタンを押したときに呼び出す関数
    public void OnPointerDown()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
    }

    // ボタンを離したときに呼び出す関数
    public void OnPointerUp()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }
}