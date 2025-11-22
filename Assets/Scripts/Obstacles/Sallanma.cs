using UnityEngine;

public class Sallanma : MonoBehaviour
{
    public float hiz = 1f;
    public float maksimumAci = 45f;

    void FixedUpdate()
    {
        // Zamanla -1 ve 1 arasýnda gidip gelen bir sinüs dalgasý oluþtur
        float sinDalga = Mathf.Sin(Time.time * hiz);

        // Bu dalgayý açýya dönüþtür
        float aci = sinDalga * maksimumAci;

        // Objenin Z açýsýný bu deðere ayarla
        transform.rotation = Quaternion.Euler(0, 0, aci);
    }
}