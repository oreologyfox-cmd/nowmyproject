using UnityEngine;

public class LtkbMouthShow : MonoBehaviour
{
    // �\���E��\����؂�ւ������I�u�W�F�N�g��C���X�y�N�^�[�Ŏw��
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