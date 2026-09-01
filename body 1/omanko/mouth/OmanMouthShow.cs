using UnityEngine;

public class OmanMouthShow : MonoBehaviour
{
    //ターゲット
    [SerializeField] private GameObject targetObject;

    void Start()
    {
        // ������Ԃ͔�\��
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }

    // �{�^����������Ƃ��ɌĂяo���֐�
    public void OnPointerDown()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
    }

    // �{�^���𗣂����Ƃ��ɌĂяo���֐�
    public void OnPointerUp()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }
}