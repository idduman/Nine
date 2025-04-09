using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using DG.Tweening;

public class MobileInputManager : MonoBehaviour
{

    //IPointerDownHandler, IDragHandler, IPointerUpHandler
    //#region Singleton
    //public static MobileInputController instance = null;
    //private void Awake()
    //{
    //    instance = this;
    //    controlType = GetControlType();
    //    startType = GameSettingsData.gameSettings.GameStarterType;
    //    resetDuration = GameSettingsData.gameSettings.InputResetDuration;
    //}
    //#endregion

    //[LabelText("On Pointer Down"), SerializeField] private Vector2Event onPointerDownEvent;
    //[LabelText("On Pointer Drag"), SerializeField] private Vector2Event onPointerDragEvent;
    //[LabelText("On Pointer Up"), SerializeField] private Vector2Event onPointerUpEvent;
    //[SerializeField] private FloatVariable sensitivity;

    //private MobileControlType GetControlType() { return GameSettingsData.gameSettings.MobileInputType; }

    //#region Input Data

    //private Vector2 inputData;

    //public Vector2 InputData
    //{
    //    private set { }
    //    get
    //    {
    //        return inputData;
    //    }
    //}

    //public Vector2 InputDataClamped
    //{
    //    private set { }
    //    get
    //    {
    //        Vector2 result = Vector2.zero;
    //        result.x = Mathf.Clamp(inputData.x, -1f, 1f);
    //        result.y = Mathf.Clamp(inputData.y, -1f, 1f);
    //        return result;
    //    }
    //}

    //#endregion

    //private Vector2 startPoint;
    //private float rangeMultiplier;

    //[HideInInspector] public bool isTouched = false;
    //[HideInInspector] public bool isDrag;
    //private MobileControlType controlType;
    //private GameStartType startType;
    //private float resetDuration = 0.5f;

    //Sequence resetSeq = null;

    //int isReversedInput = 1;
    //int inputCounter = 0;
    //Vector2 lastPos;
    //bool canStart = true;
    //bool isStarted = false;

    //private void Start()
    //{
    //    rangeMultiplier = (1f / (GetComponent<RectTransform>().rect.width * 0.5f)) * sensitivity.Value;

    //    if (controlType == MobileControlType.Drag)
    //    {
    //        isDrag = true;
    //    }
    //    else if (controlType == MobileControlType.Swipe)
    //    {
    //        isReversedInput = GameSettingsData.gameSettings.reversedInputData ? -1 : 1;
    //    }
    //}

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log("tap");
    //    if (!isStarted && startType == GameStartType.Tap && canStart)
    //    {
    //        isStarted = true;
    //        GameManager.instance.StartGame();
    //    }

    //    startPoint = eventData.position;
    //    inputData = Vector2.zero;

    //    isTouched = true;
    //    inputCounter = 0;

    //    if (resetSeq != null)
    //        resetSeq.Kill();

    //    if (onPointerDownEvent)
    //        onPointerDownEvent.Raise(inputData);
    //}

    //private Vector2 totalInput;
    //public void OnDrag(PointerEventData eventData)
    //{
    //    if (controlType == MobileControlType.TapTiming)
    //        return;

    //    Vector2 dis = new Vector2(eventData.delta.x, eventData.position.y - startPoint.y);

    //    if (!isStarted && startType == GameStartType.WhenSwipe && dis.magnitude != 0 && canStart)
    //    {
    //        isStarted = true;
    //        GameManager.instance.StartGame();
    //    }

    //    if (isDrag)
    //    {
    //        inputData = dis * rangeMultiplier * isReversedInput;
    //        totalInput += inputData;
    //    }
    //    else
    //    {
    //        inputData = dis * rangeMultiplier * isReversedInput;
    //    }

    //    if (onPointerDragEvent)
    //        onPointerDragEvent.Raise(inputData);
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    if (controlType == MobileControlType.TapTiming)
    //        return;

    //    startPoint = Vector2.zero;
    //    isTouched = false;
    //    inputCounter = 0;

    //    if (onPointerUpEvent)
    //        onPointerUpEvent.Raise(inputData);

    //    if (resetDuration == 0)
    //    {
    //        inputData = Vector2.zero;
    //    }
    //    else
    //    {
    //        resetSeq = DOTween.Sequence();
    //        resetSeq.Append(
    //            DOTween.To(() => inputData, x => inputData = x, Vector2.zero, resetDuration)
    //        );
    //    }
    //}
}
