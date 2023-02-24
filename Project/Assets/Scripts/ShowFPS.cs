using UnityEngine;
using UnityEngine.UI;
public class ShowFPS : MonoBehaviour 
{
    [SerializeField, Tooltip("拖入显示FPS的TEXT文本组件")]
    public Text fpsText;
    [SerializeField, Tooltip("true:显示FPS;false:隐藏FPS")]
    public bool isShowFPS = true;
    [SerializeField, Tooltip("FPS刷新周期T")]
    public float updateTimeT = 0.5f;
    private int framesCount = 0;
    private float accumTime = 0.0f;
    private float leftTime = 0.0f;
    void Update()
    {
        if (fpsText != null && isShowFPS)
        {
            ShowFps();
        }
    }
    private void ShowFps()
    {
        leftTime -= Time.unscaledDeltaTime;     //无缩放的delta time
        accumTime += Time.unscaledDeltaTime;
        ++framesCount;
        if (leftTime <= 0)
        {
            float fps = framesCount / accumTime; //帧数/帧数对应的实际时间
            fpsText.text = string.Format("FPS: {0:f0}", fps);
            accumTime = 0.0f;                     //初始化
            framesCount = 0;
            leftTime = updateTimeT;
        }
    }
}