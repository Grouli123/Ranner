using UnityEngine;

public class SwipeController : MonoBehaviour
{
    public static SwipeController instance;

    public enum Direction {Left, Right, Up, Down};

    private bool[] swipe = new bool[4];

    const float SWIPE_THERESHOLD = 50;

    public delegate void MoveDelegate(bool[] swips);
    public MoveDelegate MoveEvent;

    public delegate void ClickDelegate(Vector2 pos);
    public ClickDelegate ClickEvent;

    private Vector2 startTouch;
    private Vector2 swipeDelta;

    private Vector2 TouchPosition() { return (Vector2)Input.mousePosition; }

    private bool TouchBegan() { return Input.GetMouseButtonDown(0); }
    private bool TouchEnded() { return Input.GetMouseButtonUp(0); }
    private bool GetTouch() { return Input.GetMouseButton(0); }

    private bool touchMoved;

    private void Awake() => instance = this;

    private void Update() 
    {
        if(TouchBegan())
        {
            startTouch = TouchPosition();
            touchMoved = true;
        }    

        swipeDelta = Vector2.zero;
        if(touchMoved && GetTouch())
        {
            swipeDelta = TouchPosition() - startTouch;
        }

        if(swipeDelta.magnitude > SWIPE_THERESHOLD)
        {
            if(Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                swipe[(int)Direction.Left] = swipeDelta.x < 0;
                swipe[(int)Direction.Right] = swipeDelta.x > 0;

            }
            else
            {
                swipe[(int)Direction.Up] = swipeDelta.y > 0;
                swipe[(int)Direction.Down] = swipeDelta.y < 0;
            }
            SendSwipe();
        }
    }

    private void SendSwipe()
    {
        if(swipe[0] || swipe[1] || swipe[2] || swipe[3])
        {
            Debug.Log(swipe[0] +"|"+ swipe[1] +"|"+ swipe[2] +"|"+ swipe[3]);

            MoveEvent?.Invoke(swipe);
        }
        else
        {
            ClickEvent?.Invoke(TouchPosition());
        }

        Reset();
    }

    private void Reset() 
    {
        startTouch = swipeDelta = Vector2.zero;
        touchMoved = false;

        for (int i = 0; i < 4; i++)
        {
            swipe[i] = false;
        }    
    }
}