using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WindowsInput;

public class Keyboard : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

    [DllImport("user32.dll")]
    internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    //Mouse actions
    private const int MOUSEEVENTF_LEFTDOWN = 0x02;
    private const int MOUSEEVENTF_LEFTUP = 0x04;
    private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
    private const int MOUSEEVENTF_RIGHTUP = 0x10;

    HandPresence RightHandScript;
    HandPresence LeftHandScript;
    RectangleTool RectTool;
    bruhsh bruhshScript;
    planeCreation planeMaker;

    public GameObject RightHand;
    public GameObject LeftHand;
    public GameObject LeftCursor;
    public GameObject RightCursor;
    public static GameObject selectedDesktop;
    public GameObject LeftKeyboard;
    public GameObject RightKeyboard;
    public static GameObject TextField;
    public GameObject LeftKeyboardRay;
    public GameObject RightKeyboardRay;
    public GameObject LSelectionOverlay;
    public GameObject RSelectionOverlay;
    public GameObject RectoTool;
    public GameObject bruhshG;
    public GameObject planeCreator;
    public static string TextInput = "";

    private static int LeftKeyboardWidth = 187;
    private static int KeyboardHeight = 133;
    private static int RightKeyboardWidth = 233;
    private bool leftWasTouchpadClicked;
    private bool rightWasTouchpadClicked;
    private static bool KeyboardsActive = false;
    private bool shiftActive = false;
    bool SelectEnter = true;
    bool holdingMouse = false;
    float holdTime = 0f;
    float lastTime = 0f;
    (uint, uint) cursorPos = (0, 0);
    bool justSelectedDesktop = false;

    [SerializeField]
    private Vector2 right2DAxis;
    [SerializeField]
    private Vector2 left2DAxis;

    private static float TabSpacing = 0.21390374f;
    private static float CapsSpacing = 0.2513369f;
    private static float LShiftSpacing = 0.32085562f;
    private static float LCtrlSpacing = 0.2139037433f;
    private static float LAltSpacing = 0.2192513368984f;
    private static float LSpaceSpacing = 0.320855614973262f;
    private static float NormalKeySpacing = 0.144385026737967914f;
    private static float BackSpaceSpacing = 0.28877005347594f;
    private static float RightFirstSpacing = 0.0815450643776824f;
    private static float RightSecondSpacing = 0.0257510729614f;
    private static float PipeSpacing = 0.213903743315508f;
    private static float RightThirdSpacing = 0.055793991416309f;
    private static float EnterSpacing = 0.2575107296137339f;
    private static float RShiftSpacing = 0.317596566523605f;
    private static float RSpaceSpacing = 0.42918454935622318f;
    private static float RAltSpacing = 0.1759656652360515f;
    private static float RCtrlSpacing = 0.17167381974248927f;

    //First Row Left Keyboard
    static Vector2 KeyTilde = new Vector2(0 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 12), 0 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 Key1 = new Vector2(1 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 12), 0 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 Key2 = new Vector2(2 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 12), 0 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 Key3 = new Vector2(3 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 12), 0 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 Key4 = new Vector2(4 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 12), 0 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 Key5 = new Vector2(5 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 12), 0 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 Key6 = new Vector2(6 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 12), 0 * (KeyboardHeight / 5) + (KeyboardHeight / 10));

    //Second Row Left Keyboard
    static Vector2 KeyTab = new Vector2(TabSpacing * LeftKeyboardWidth / 2, 1 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyQ = new Vector2(0 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 10) + TabSpacing * LeftKeyboardWidth, 1 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyW = new Vector2(1 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 10) + TabSpacing * LeftKeyboardWidth, 1 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyE = new Vector2(2 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 10) + TabSpacing * LeftKeyboardWidth, 1 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyR = new Vector2(3 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 10) + TabSpacing * LeftKeyboardWidth, 1 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyT = new Vector2(4 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 10) + TabSpacing * LeftKeyboardWidth, 1 * (KeyboardHeight / 5) + (KeyboardHeight / 10));

    //Third Row Left Keyboard
    static Vector2 KeyCaps = new Vector2(CapsSpacing * LeftKeyboardWidth / 2, 2 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyA = new Vector2(0 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 10) + CapsSpacing * LeftKeyboardWidth, 2 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyS = new Vector2(1 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 10) + CapsSpacing * LeftKeyboardWidth, 2 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyD = new Vector2(2 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 10) + CapsSpacing * LeftKeyboardWidth, 2 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyF = new Vector2(3 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 10) + CapsSpacing * LeftKeyboardWidth, 2 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyG = new Vector2(4 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 10) + CapsSpacing * LeftKeyboardWidth, 2 * (KeyboardHeight / 5) + (KeyboardHeight / 10));

    //Fourth Row Left Keyboard
    static Vector2 KeyLShift = new Vector2(LShiftSpacing * LeftKeyboardWidth / 2, 3 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyZ = new Vector2(0 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 10) + LShiftSpacing * LeftKeyboardWidth, 3 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyX = new Vector2(1 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 10) + LShiftSpacing * LeftKeyboardWidth, 3 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyC = new Vector2(2 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 10) + LShiftSpacing * LeftKeyboardWidth, 3 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyV = new Vector2(3 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 10) + LShiftSpacing * LeftKeyboardWidth, 3 * (KeyboardHeight / 5) + (KeyboardHeight / 10));

    //Fifth Row Left Keyboard
    static Vector2 KeyLCtrl = new Vector2(LCtrlSpacing * LeftKeyboardWidth / 2, 4 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyLWin = new Vector2((LeftKeyboardWidth / 10) + LCtrlSpacing * LeftKeyboardWidth, 4 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyLAlt = new Vector2((LeftKeyboardWidth / 5) + LCtrlSpacing * LeftKeyboardWidth + LAltSpacing * LeftKeyboardWidth, 4 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyLSpace = new Vector2(2 * (LeftKeyboardWidth / 7) + (LeftKeyboardWidth / 10) + LCtrlSpacing * LeftKeyboardWidth + LSpaceSpacing * LeftKeyboardWidth, 4 * (KeyboardHeight / 5) + (KeyboardHeight / 10));

    //First Row Right Keyboard
    static Vector2 Key7 = new Vector2(0 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth / 2) + (RightFirstSpacing * RightKeyboardWidth), 0 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 Key8 = new Vector2(1 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth / 2) + (RightFirstSpacing * RightKeyboardWidth), 0 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 Key9 = new Vector2(2 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth / 2) + (RightFirstSpacing * RightKeyboardWidth), 0 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 Key0 = new Vector2(3 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth / 2) + (RightFirstSpacing * RightKeyboardWidth), 0 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyMinus = new Vector2(4 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth / 2) + (RightFirstSpacing * RightKeyboardWidth), 0 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyPlus = new Vector2(5 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth / 2) + (RightFirstSpacing * RightKeyboardWidth), 0 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyBackspace = new Vector2(6 * (NormalKeySpacing * RightKeyboardWidth) + (BackSpaceSpacing * RightKeyboardWidth / 2) + (RightFirstSpacing * RightKeyboardWidth), 0 * (KeyboardHeight / 5) + (KeyboardHeight / 10));

    //Second Row Right Keyboard
    static Vector2 KeyY = new Vector2(0 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2 + RightSecondSpacing * RightKeyboardWidth, 1 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyU = new Vector2(1 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2 + RightSecondSpacing * RightKeyboardWidth, 1 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyI = new Vector2(2 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2 + RightSecondSpacing * RightKeyboardWidth, 1 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyO = new Vector2(3 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2 + RightSecondSpacing * RightKeyboardWidth, 1 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyP = new Vector2(4 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2 + RightSecondSpacing * RightKeyboardWidth, 1 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyCurlyBracketOpen = new Vector2(5 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2 + RightSecondSpacing * RightKeyboardWidth, 1 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyCurlyBracketClose = new Vector2(6 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2 + RightSecondSpacing * RightKeyboardWidth, 1 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyPipe = new Vector2(7 * (NormalKeySpacing * RightKeyboardWidth) + RightSecondSpacing * RightKeyboardWidth + (PipeSpacing * RightKeyboardWidth) / 2, 1 * (KeyboardHeight / 5) + (KeyboardHeight / 10));

    //Third Row Right Keyboard
    static Vector2 KeyH = new Vector2(0 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2 + RightThirdSpacing * RightKeyboardWidth, 2 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyJ = new Vector2(1 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2 + RightThirdSpacing * RightKeyboardWidth, 2 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyK = new Vector2(2 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2 + RightThirdSpacing * RightKeyboardWidth, 2 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyL = new Vector2(3 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2 + RightThirdSpacing * RightKeyboardWidth, 2 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyColon = new Vector2(4 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2 + RightThirdSpacing * RightKeyboardWidth, 2 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyQuote = new Vector2(5 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2 + RightThirdSpacing * RightKeyboardWidth, 2 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyEnter = new Vector2(6 * (NormalKeySpacing * RightKeyboardWidth) + (EnterSpacing * RightKeyboardWidth) / 2 + RightThirdSpacing * RightKeyboardWidth, 2 * (KeyboardHeight / 5) + (KeyboardHeight / 10));

    //Fourth Row Right Keyboard
    static Vector2 KeyB = new Vector2(0 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2, 3 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyN = new Vector2(1 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2, 3 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyM = new Vector2(2 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2, 3 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyLessThan = new Vector2(3 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2, 3 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyGreaterThan = new Vector2(4 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2, 3 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyQuestionMark = new Vector2(5 * (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2, 3 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyRShift = new Vector2(6 * (NormalKeySpacing * RightKeyboardWidth) + (RShiftSpacing * RightKeyboardWidth) / 2, 3 * (KeyboardHeight / 5) + (KeyboardHeight / 10));

    //Fifth Row Left Keyboard
    static Vector2 KeyRSpace = new Vector2(RSpaceSpacing * LeftKeyboardWidth / 2, 4 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyRAlt = new Vector2((RSpaceSpacing * LeftKeyboardWidth) + (RAltSpacing * LeftKeyboardWidth) / 2, 4 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyRWin = new Vector2((RSpaceSpacing * LeftKeyboardWidth) + (RAltSpacing * LeftKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2, 4 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyMenu = new Vector2((RSpaceSpacing * LeftKeyboardWidth) + (RAltSpacing * LeftKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) + (NormalKeySpacing * RightKeyboardWidth) / 2, 4 * (KeyboardHeight / 5) + (KeyboardHeight / 10));
    static Vector2 KeyRCtrl = new Vector2((RSpaceSpacing * LeftKeyboardWidth) + (RAltSpacing * LeftKeyboardWidth) + (2 * NormalKeySpacing * RightKeyboardWidth) + (RCtrlSpacing * RightKeyboardWidth) / 2, 4 * (KeyboardHeight / 5) + (KeyboardHeight / 10));

    Vector2[] LKeys = new Vector2[] { KeyTilde, Key1, Key2, Key3, Key4, Key5, Key6, KeyTab, KeyQ, KeyW, KeyE, KeyR, KeyT, KeyCaps, KeyA, KeyS, KeyD, KeyF, KeyG, KeyLShift, KeyZ, KeyX, KeyC, KeyV, KeyLCtrl, KeyLWin, KeyLAlt, KeyLSpace };
    Vector2[] RKeys = new Vector2[] { Key7, Key8, Key9, Key0, KeyMinus, KeyPlus, KeyBackspace, KeyY, KeyU, KeyI, KeyO, KeyP, KeyCurlyBracketOpen, KeyCurlyBracketClose, KeyPipe, KeyH, KeyJ, KeyK, KeyL, KeyColon, KeyQuote, KeyEnter, KeyB, KeyN, KeyM, KeyLessThan, KeyGreaterThan, KeyQuestionMark, KeyRShift, KeyRSpace, KeyRAlt, KeyRWin, KeyMenu, KeyRCtrl };

    string[] LTrans = { };
    string[] RTrans = { };
    string[] LTransShift = { };
    string[] RTransShift = { };

    void Start()
    {
        RightHandScript = RightHand.GetComponent<HandPresence>();
        LeftHandScript = LeftHand.GetComponent<HandPresence>();
        RectTool = RectoTool.GetComponent<RectangleTool>();
        bruhshScript = bruhshG.GetComponent<bruhsh>();
        planeMaker = planeCreator.GetComponent<planeCreation>();

        leftWasTouchpadClicked = false;
        rightWasTouchpadClicked = false;

        LeftKeyboard.SetActive(false);
        RightKeyboard.SetActive(false);

        LTrans = new string[] { "~", "1", "2", "3", "4", "5", "6", "", "q", "w", "e", "r", "t", "", "a", "s", "d", "f", "g", "", "z", "x", "c", "v", "", "", "", " " };
        RTrans = new string[] { "7", "8", "9", "0", "-", "+", "", "y", "u", "i", "o", "p", "{", "}", "|", "h", "j", "k", "l", ":", "\"", "", "b", "n", "m", "<", ">", "?", "", " ", "", "", "", "" };
        
        LTransShift = new string[] { "`", "!", "@", "#", "$", "%", "^", "", "Q", "W", "E", "R", "T", "", "A", "S", "D", "F", "G", "", "Z", "X", "C", "V", "", "", "", " " };
        RTransShift = new string[] { "&", "*", "(", ")", "_", "=", "", "Y", "U", "I", "O", "P", "[", "]", "\\", "H", "J", "K", "L", ";", "\'", "", "B", "N", "M", ",", ".", "/", "", " ", "", "", "", "" };
    }

    void Update()
    {
        //Bei jeder Messung sind die 1px Kanten mitgezählt. d.H. bei Hitboxen einführen diese herausnehmen
        //LeftKeyboard ist 187px breit und 133px hoch
        //RightKeyboard ist 233px breit und 133px hoch
        //40x27 ctrl
        //27x27 win key
        //60x28 shift
        //47x27 caps lock
        //39x28 tab
        //27x27 tilde
        //tilde -> 1, 27px, 27x27 [13 Mal und dann Backspace]
        //Alternativ: nimm an jeder Key ist 27x27 und mach Abfrage radius 13px
        //
        //26 backspace
        //20 slash
        //30 enter
        //36 r-shift
        //20 r-ctrl
        //20 r-alt
        //50 r-space

        if (KeyboardsActive)
        {
            LSelectionOverlay.SetActive(false);
            RSelectionOverlay.SetActive(false);

            LeftKeyboard.SetActive(true);
            RightKeyboard.SetActive(true);
        }
        else if (!KeyboardsActive && RectTool.getMode() == 3)
        {
            LSelectionOverlay.SetActive(true);
            RSelectionOverlay.SetActive(true);

            LeftKeyboard.SetActive(false);
            RightKeyboard.SetActive(false);
        }
        else
        {
            LSelectionOverlay.SetActive(false);
            RSelectionOverlay.SetActive(false);

            LeftKeyboard.SetActive(false);
            RightKeyboard.SetActive(false);
        }

        if (KeyboardsActive && (RectTool.getMode() == 3 || (RectTool.getMode() == 1 && bruhshScript.toolSelect == 9)))
        {
            float TrueRightKeyboardWidth = RKeys[6].x + (BackSpaceSpacing * RightKeyboardWidth) / 2;
            float TrueLeftKeyboardWidth = LKeys[6].x + (NormalKeySpacing * LeftKeyboardWidth) / 2;
            float TrueKeyboardHeight = RKeys[RKeys.Length - 1].y + (NormalKeySpacing * KeyboardHeight) / 2;

            right2DAxis = RightHandScript.getPrimary2DAxis();
            left2DAxis = LeftHandScript.getPrimary2DAxis();

            RightCursor.SetActive(true);
            RightKeyboard.transform.eulerAngles = new Vector3(0, 0, 0);
            RightKeyboard.transform.position = RightHand.transform.position + new Vector3(0f, 0f, 0f);
            RightKeyboard.transform.eulerAngles = RightHand.transform.eulerAngles;

            RightCursor.transform.localPosition = new Vector3(-right2DAxis.x * 0.5f, 0f, -right2DAxis.y * 0.5f);

            LeftCursor.SetActive(true);
            LeftKeyboard.transform.eulerAngles = new Vector3(0, 0, 0);
            LeftKeyboard.transform.position = LeftHand.transform.position + new Vector3(0f, 0f, 0f);
            LeftKeyboard.transform.eulerAngles = LeftHand.transform.eulerAngles;

            LeftCursor.transform.localPosition = new Vector3(-left2DAxis.x * 0.5f, 0f, -left2DAxis.y * 0.5f);

            if (LeftHandScript.getPressedTouchpad() && !leftWasTouchpadClicked && RectTool.getSelectOverlay() == 1 && KeyboardsActive)
            {
                var closestKey = new Vector2(0f, 0f);
                int i = 0;
                int closestNumber = 0;
                foreach (Vector2 key in LKeys)
                {
                    if (
                            Vector2.Distance(
                                new Vector2(key.x / TrueLeftKeyboardWidth, key.y / TrueKeyboardHeight),
                                new Vector2((left2DAxis.x + 1f) / 2, (-left2DAxis.y + 1f) / 2)
                            ) <
                            Vector2.Distance(
                                new Vector2(closestKey.x / TrueLeftKeyboardWidth, closestKey.y / TrueKeyboardHeight),
                                new Vector2((left2DAxis.x + 1f) / 2, (-left2DAxis.y + 1f) / 2)
                            )
                    )
                    {
                        closestKey = key;
                        closestNumber = i;
                    }
                    i += 1;
                }
                i = 0;

                switch (shiftActive)
                {
                    case false:
                        TextInput += LTrans[closestNumber];
                        break;
                    case true:
                        TextInput += LTransShift[closestNumber];
                        break;
                    default:
                        UnityEngine.Debug.Log("shiftActive isn't defined");
                        break;
                }

                if (LKeys[closestNumber] == KeyLShift)
                {
                    shiftActive = !shiftActive;
                }

                /*
                 * KeyLCtrl
                 * KeyRCtrl
                 * KeyMenu
                 * KeyCaps
                 * KeyLAlt
                 * KeyRAlt
                 * KeyLWin
                 * KeyRWin
                 * KeyTab
                 */
                if (RectTool.getMode() == 3)
                {
                    TextField.GetComponent<Text>().text = TextInput;
                }
                else
                {
                    bruhshScript.setTextInput(TextInput);
                }

                leftWasTouchpadClicked = true;
            }
            else if (!LeftHandScript.getPressedTouchpad() && RectTool.getSelectOverlay() == 1)
            {
                leftWasTouchpadClicked = false;
            }
            else
            {
                LeftCursor.SetActive(false);
            }

            if (RightHandScript.getPressedTouchpad() && !rightWasTouchpadClicked && RectTool.getSelectOverlay() == 1 && KeyboardsActive)
            {
                var closestKey = new Vector2(0f, 0f);
                int i = 0;
                int closestNumber = 0;
                foreach (Vector2 key in RKeys)
                {
                    if (
                            Vector2.Distance(
                                new Vector2(key.x / TrueRightKeyboardWidth, key.y / TrueKeyboardHeight),
                                new Vector2((right2DAxis.x + 1f) / 2, (-right2DAxis.y + 1f) / 2)
                            ) <
                            Vector2.Distance(
                                new Vector2(closestKey.x / TrueRightKeyboardWidth, closestKey.y / TrueKeyboardHeight),
                                new Vector2((right2DAxis.x + 1f) / 2, (-right2DAxis.y + 1f) / 2)
                            )
                    )
                    {
                        closestKey = key;
                        closestNumber = i;
                    }
                    i += 1;
                }
                i = 0;

                switch (shiftActive)
                {
                    case false:
                        TextInput += RTrans[closestNumber];
                        break;
                    case true:
                        TextInput += RTransShift[closestNumber];
                        break;
                    default:
                        UnityEngine.Debug.Log("shiftActive isn't defined");
                        break;
                }

                if (RKeys[closestNumber] == KeyRShift)
                {
                    shiftActive = !shiftActive;
                }

                else if (RKeys[closestNumber] == KeyBackspace)
                {
                    if (TextInput.Length != 0)
                    {
                        TextInput = TextInput.Remove(TextInput.Length - 1);
                    }
                }

                else if (RKeys[closestNumber] == KeyEnter)
                {
                    if (TextInput == "Desktop")
                    {
                        selectedDesktop.GetComponent<uWindowCapture.UwcWindowTexture>().type = uWindowCapture.WindowTextureType.Desktop;
                    }
                    else
                    {
                        selectedDesktop.GetComponent<uWindowCapture.UwcWindowTexture>().type = uWindowCapture.WindowTextureType.Window;
                        selectedDesktop.GetComponent<uWindowCapture.UwcWindowTexture>().partialWindowTitle = TextInput;
                    }
                    //Note to self: trying to change case sensitivity on this search is a pain in the arse
                }

                if (RectTool.getMode() == 3)
                {
                    TextField.GetComponent<Text>().text = TextInput;
                }
                else
                {
                    bruhshScript.setTextInput(TextInput);
                }

                rightWasTouchpadClicked = true;
            }
            else if (!RightHandScript.getPressedTouchpad() && RectTool.getSelectOverlay() == 1)
            {
                rightWasTouchpadClicked = false;
            }
            else
            {
                RightCursor.SetActive(false);
            }
        }

        //KeyboardEnabling / Disabling

        if ((RightHandScript.getPressedTrigger() || LeftHandScript.getPressedTrigger()) && KeyboardsActive)
        {
            KeyboardsActive = false;

            LeftKeyboard.SetActive(false);
            RightKeyboard.SetActive(false);
            LSelectionOverlay.SetActive(true);
            RSelectionOverlay.SetActive(true);
        }

        if (RectTool.getMode() == 3 && RightHandScript.getPressedTriggerDegree() > 0.3f || justSelectedDesktop) //could do it with leftHand
        {
            Vector3 rayDirection = -RightHand.transform.forward;
            Vector3 rayStart = RightHand.transform.position;
            RaycastHit rayHit;

            if (Physics.Raycast(rayStart, rayDirection, out rayHit) && rayHit.collider.gameObject != null && selectedDesktop != null && rayHit.collider.gameObject == selectedDesktop)
            {
                Vector3 localHit = selectedDesktop.GetComponent<Collider>().gameObject.transform.InverseTransformPoint(rayHit.point);

                int xCursor = (int)((0.49 + localHit.x) * 2000);
                int yCursor = (int)((0.47 - localHit.y) * 1150);
                SetCursorPos(xCursor, yCursor);
                cursorPos = ((uint)xCursor, (uint)yCursor);
                
                if (!holdingMouse)
                {
                    holdingMouse = true;
                    DoMouseEnterHold((uint)xCursor, (uint)yCursor);
                }
            }
        }

        if (holdingMouse)
        {
            SetCursorPos((int)cursorPos.Item1, (int)cursorPos.Item2);

            if (!RightHandScript.getPressedTrigger() && !LeftHandScript.getPressedTrigger())
            {
                holdTime = UnityEngine.Time.time - lastTime;

                DoMouseExitHold(cursorPos.Item1, cursorPos.Item2);

                if (holdTime < 0.5f)
                {
                    DoMouseClick(cursorPos.Item1, cursorPos.Item2);
                }
                else if (holdTime >= 0.5f)
                {
                    //DoMouseClick(cursorPos.Item1, cursorPos.Item2); //does a mouse click if let go by dragging
                    //do nothing
                    UnityEngine.Debug.Log("held for longer than 0.5f");
                }

                holdingMouse = false;
                justSelectedDesktop = false;
            }
        }

        if(!holdingMouse) lastTime = UnityEngine.Time.time;
    }

    public void SelectNewDesktop(GameObject sender)
    {
        if (RightHandScript.getPressedTrigger() || LeftHandScript.getPressedTrigger())
        {
            TextField = sender.GetComponentInChildren<Text>().gameObject;
            selectedDesktop = sender.gameObject;
            TextInput = "";
            TextInput += sender.GetComponentInChildren<Text>().text;

            KeyboardsActive = true;
        }
        justSelectedDesktop = true;

        return;
    }

    public void SelectNewDrawText(string preText)
    {
        TextInput = "";
        TextInput += preText;

        KeyboardsActive = true;
        return;
    }

    public bool getKeyboardsActive()
    {
        return KeyboardsActive;
    }

    public void setKeyboardsActive(bool keyActive)
    {
        KeyboardsActive = keyActive;
    }

    public GameObject getSelectedDesktop()
    {
        return selectedDesktop;
    }

    public string getTextInput()
    {
        return TextInput;
    }

    public void DoMouseClick(uint X, uint Y)
    {
        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
    }

    public void DoMouseEnterHold(uint X, uint Y)
    {
        mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
    }

    public void DoMouseExitHold(uint X, uint Y)
    {
        mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
    }

    public void FocusProcess(string processName) //static
    {
        IntPtr hWnd;
        Process[] processRunning = Process.GetProcesses();
        foreach (Process pr in processRunning)
        {
            if (pr.ProcessName == processName)
            {
                hWnd = pr.MainWindowHandle;
                ShowWindow(hWnd, 3);
                SetForegroundWindow(hWnd);
            }
        }
    }

    public void pressKey(string key, Process myProcess)
    {
        if (key == "N")
        {
            InputSimulator sim = new InputSimulator();
            sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.VK_N);
        }
    }
}
