using UnityEngine;

[System.Serializable]
public class Card
{
    public GameObject Piece;
    public int ID;

    public Card(GameObject obj, int index)
    {
        Piece = obj;
        ID = index;
        Piece.GetComponent<Renderer>().material.SetTexture("_MainTex", Resources.Load<Texture2D>("Cards/Card_" + ID));
    }

    ~Card()
    {
        //Destroy(Piece)
        ID = 0;
    }
}
