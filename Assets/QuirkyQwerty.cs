using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuirkyQwerty : MonoBehaviour
{
    public KMAudio audio;
    public KMBombModule module;
    public KMSelectable[] keys = new KMSelectable[26];
    public GameObject[] switches = new GameObject[26];

    List<string> keyboard = new List<string> {"Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "Z", "X", "C", "V", "B", "N", "M"};
    List<string> words = new List<string>
    { 
        "LLANFAIRPWLLGWYNGYLL", 
        "ANTIESTABLISHMENTISM", "ELECTROCARDIOGRAPHIC", "HYPERCOAGULABILITIES", "INDISTINGUISHABILITY", "LIPOCHONDRODYSTROPHY", 
        "NONINSTITUTIONALIZED", "PSEUDOHERMAPHRODITIC", "SEMIAUTOBIOGRAPHICAL", "UNCHARACTERISTICALLY", "DISESTABLISMENTARIAN", 
        "BALLISTOCARDIOGRAPHY", "INTERDIFFERENTIATION", "PARALLELOGRAMMATICAL", "NONCONDESCENDINGNESS", "THERMOPOLYMERIZATION", 
        "SUPERINQUISITIVENESS", "PHILOSOPHICOJURISTIC", "KERATOCONJUNCTIVITIS", "UNEXTINGUISHABLENESS",
    };

    int[] code = new int[13];

    int[] scrambleStart = {0, 0, 10, 19};
    int[] scrambleEnd = {25, 9, 18, 25};

    public TextMesh codeText;
    public TextMesh inputText;

    string solution = "";
    string input = "";

    bool canInput = true;

    private static int _moduleIdCounter = 1;
    private int _moduleId = 0;

    void Start()
    {
        _moduleId = _moduleIdCounter++;

        codeText.text = "";
        inputText.text = "";

        string codeStr = "";
        for(int i = 0; i < code.Length; i++)
        {
            code[i] = Random.Range(0, 10);
            codeStr += code[i];
        }
        codeText.text = codeStr;
        Debug.LogFormat("[quirkyQwerty #{0}] Given code: " + codeStr, _moduleId);
        
        ScrambleKeys();
        string line1 = "";
        string line2 = "";
        string line3 = "";
        for (int i = 0; i < 10; i++)
        {
            line1 += keyboard[i] + " ";
        }
        for (int i = 10; i < 19; i++)
        {
            line2 += keyboard[i] + " ";
        }
        for (int i = 19; i < 26; i++)
        {
            line3 += keyboard[i] + " ";
        }
        Debug.LogFormat("[quirkyQwerty #{0}] Scrambled keyboard Layout:", _moduleId);
        Debug.LogFormat("[quirkyQwerty #{0}] " + line1, _moduleId);
        Debug.LogFormat("[quirkyQwerty #{0}] " + line2, _moduleId);
        Debug.LogFormat("[quirkyQwerty #{0}] " + line3, _moduleId);

        int totalVal = 0;
        foreach(int i in code)
        {
            totalVal += i;
        }
        solution = words[totalVal % 20];
        Debug.LogFormat("[quirkyQwerty #{0}] Expected answer: " + solution.ToLower(), _moduleId);
    }

    void Awake()
    {
        foreach(KMSelectable key in keys)
        {
            key.OnInteract += delegate (){ Keypress(key); return false; };
        }
    }

    void Keypress(KMSelectable key)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if (canInput && keys[i] == key)
            {
                StartCoroutine(Click(i));
                audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, keys[i].transform);


                input += keyboard[i];
                inputText.text = input;

                if (input.Length == 20)
                {
                    if (input == solution)
                    {
                        canInput = false;
                        module.HandlePass();
                    }
                    else
                    {
                        StartCoroutine(Strike());
                    }
                }
            }
        }       
    }

    void ScrambleKeys()
    {
        int curScramble = 0;
        int curCodeNum = 0;
        while (curCodeNum <= 11)
        {
            int loops = Mathf.Abs(code[curCodeNum] - code[curCodeNum + 1]);
            while (loops > 0)
            {
                string storedValue = keyboard[scrambleEnd[curScramble % 4]];
                keyboard.Remove(storedValue);
                keyboard.Insert(scrambleStart[curScramble % 4], storedValue);
                loops--;
            }
            curScramble++;
            curCodeNum++;
        }
    }

    IEnumerator Strike()
    {
        module.HandleStrike();
        inputText.GetComponent<MeshRenderer>().material.color = new Color32(255, 0, 0, 255);
        canInput = false;

        yield return new WaitForSeconds(1);

        canInput = true;
        inputText.GetComponent<MeshRenderer>().material.color = new Color32(0, 255, 0, 255);
        input = "";
        inputText.text = input;
    }

    IEnumerator Click(int i)
    {
        if (!(switches[i].transform.localPosition.z <= 0.0001 && switches[i].transform.localPosition.z >= -0.0001))
        {
            switches[i].transform.localPosition -= new Vector3(0, 0, 0.005468956f);
            yield return new WaitForSeconds(0.05f);
            switches[i].transform.localPosition += new Vector3(0, 0, 0.005468956f);
        }
    }
}
