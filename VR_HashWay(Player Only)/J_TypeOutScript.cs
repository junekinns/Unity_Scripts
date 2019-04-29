using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text;


[Serializable]
public class J_TypeOutScript : MonoBehaviour
{

    public bool On = true;
    public Text FinalText;

    public float TotalTypeTime = -1f;

    public float TypeRate;
    private float LastTime;

    public string RandomCharactor;
    public float RandomCharacterChangeRate = 0.1f;
    private float RandomCharacterTime;

    public int i;

    //	void Start () 
    //	{
    //		try
    //		{
    //			gameObject.AddComponent(typeof(GUIText));
    //		}
    //		catch(UnityException)
    //		{
    //
    //		}
    //
    //	}

    private string RandomChar()
    {
        byte value = (byte)UnityEngine.Random.Range(41f, 128f);

        string c = Encoding.ASCII.GetString(new byte[] { value });

        return c;
    }

    public void Skip()
    {
        GetComponent<Text>().text = FinalText.text;
        On = false;
    }

    // Update is called once per frame
    void OnGUI()
    {
        if (TotalTypeTime != -1f)
        {
            TypeRate = TotalTypeTime / (float)FinalText.text.Length;
        }

        if (On == true)
        {

            if (Time.time - RandomCharacterTime >= RandomCharacterChangeRate)
            {
                RandomCharactor = RandomChar();
                RandomCharacterTime = Time.time;
            }

            try
            {
                GetComponent<Text>().text = FinalText.text.Substring(0, i) + RandomCharactor;
            }
            catch (ArgumentOutOfRangeException)
            {
                On = false;
            }

            if (Time.time - LastTime >= TypeRate)
            {
                i++;
                LastTime = Time.time;
            }

            bool isChar = false;

            while (isChar == false)
            {
                if ((i + 1) < FinalText.text.Length)
                {
                    if (FinalText.text.Substring(i, 1) == " ")
                    {
                        i++;
                    }
                    else
                    {
                        isChar = true;
                    }
                }
                else
                {
                    isChar = true;
                }
            }

            if (GetComponent<Text>().text.Length == FinalText.text.Length + 1)
            {
                RandomCharactor = RandomChar();
                GetComponent<Text>().text = FinalText.text;
                On = false;
            }

        }
    }
}
