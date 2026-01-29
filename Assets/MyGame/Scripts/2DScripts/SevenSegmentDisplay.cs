using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class SevenSegmentDisplay : MonoBehaviour
{
    [SerializeField] private Manager2D manager;

    [SerializeField] private float extendedYPostion = -0.7f;
    [SerializeField] private float retractedYPosition = 0f;

    private SegmentMover[] segments;

    public bool allOn = false;
    public bool isNumberSet = false;
    private int currentNumber = 0;

    public bool isActive = true;

    private bool[] inputDisplay = new bool[7];
    public Image[] segmentFeedback;

    public Color32 activeColor = Color.green, inactiveColor = Color.white;
    private Dictionary<KeyCode, Segments> keySegmentMap;

    // Zweidimensionales Array - Direktes Mapping:
    // Jede Zeile ist eine Zahl (0-9), jedes Element ein Segment (A-G)
    private bool[,] numberPatterns =
    {
        { true,  true,  true,  true,  true,  true,  false }, // 0 
        { false, true,  true,  false, false, false, false }, // 1 
        { true,  true,  false, true,  true,  false, true  }, // 2 
        { true,  true,  true,  true,  false, false, true  }, // 3 
        { false, true,  true,  false, false, true,  true  }, // 4 
        { true,  false, true,  true,  false, true,  true  }, // 5 
        { true,  false, true,  true,  true,  true,  true  }, // 6 
        { true,  true,  true,  false, false, false, false }, // 7 
        { true,  true,  true,  true,  true,  true,  true  }, // 8 
        { true,  true,  true,  false, false, true,  true  }  // 9
    };

    private void Awake()
    {
        segments = GetComponentsInChildren<SegmentMover>();
    }

    private void Start()
    {
        keySegmentMap = new Dictionary<KeyCode, Segments>
        {
            { KeyCode.W, Segments.A },
            { KeyCode.A, Segments.B },
            { KeyCode.UpArrow, Segments.C },
            { KeyCode.LeftArrow, Segments.D },
            { KeyCode.DownArrow, Segments.E },
            { KeyCode.RightArrow, Segments.F },
            { KeyCode.S, Segments.G }
        };

        SetPosition();
        AllSegmentON();
    }

    public void ExtendSegmentsFor(int number)
    {
        if (number < 0 || number > 9) return; // Card Clause; Early Return: Nur Zahlen 0–9 zulassen

        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].ExtendSegment(numberPatterns[number, i]); // Segmente ausfahren oder einfahren
        }
    }

    private void SetPosition()
    {
        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].SetExtendedPosition(extendedYPostion);
            segments[i].SetHiddenPosZ(retractedYPosition);
        }
    }

    public void ResetSegmentDisplay()
    {
        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].ResetNoNumberSet();
        }
    }

    public void AllSegmentON()
    {
        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].ExtendSegment(true);
        }
    }

    public int DecodeDigit(bool[] input)
    {
        if (input.Length != 7)
        {
            Debug.LogError("Eingabe muss genau 7 bool-Werte enthalten.");
            return -1;
        }


        for (int digit = 0; digit < 10; digit++)
        {
            bool match = true;
            for (int i = 0; i < 7; i++)
            {
                if (input[i] != numberPatterns[digit, i])
                {
                    match = false;
                    break;
                }
            }

            if (match)
                return digit;
        }

        return -1; // Keine Übereinstimmung gefunden
    }

    public void SetDigit()
    {
        manager.SetDigit(manager.GetTaskProgress(), currentNumber);
    }

    private void Update() 
    {
        inputDisplay[(int)Segments.A] = Input.GetKey(KeyCode.W);
        inputDisplay[(int)Segments.B] = Input.GetKey(KeyCode.A);
        inputDisplay[(int)Segments.C] = Input.GetKey(KeyCode.UpArrow);
        inputDisplay[(int)Segments.D] = Input.GetKey(KeyCode.LeftArrow);
        inputDisplay[(int)Segments.E] = Input.GetKey(KeyCode.DownArrow);
        inputDisplay[(int)Segments.F] = Input.GetKey(KeyCode.RightArrow);
        inputDisplay[(int)Segments.G] = Input.GetKey(KeyCode.S);


        string all = " ";

        for (int digit = 0; digit < inputDisplay.Length; digit++)
        {
            all += inputDisplay[digit].ToString() + " ";
        }
        Debug.Log(all);

        for (int i = 0; i < segmentFeedback.Length; i++)
        {
            segmentFeedback[i].color = inputDisplay[i] ? activeColor : inactiveColor;
        }

        int temp = DecodeDigit(inputDisplay);

        if (temp != -1)
        {
            ExtendSegmentsFor(temp);

            currentNumber = temp;
        }
        
    }
}
