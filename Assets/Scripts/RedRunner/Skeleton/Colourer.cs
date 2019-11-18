using UnityEngine;

public class Colourer : MonoBehaviour
{
    public enum RunnerColours
    {
        Red = 0,
        Green = 1,
        Brown = 2,
        Pink = 3
    }

    #region Fields
    [SerializeField]
    private SpriteRenderer m_Body;
    [SerializeField]
    private SpriteRenderer m_RightFoot;
    [SerializeField]
    private SpriteRenderer m_LeftFoot;
    [SerializeField]
    private SpriteRenderer m_RightHand;
    [SerializeField]
    private SpriteRenderer m_LeftHand;
    [SerializeField]
    private SpriteRenderer m_RightArm;
    [SerializeField]
    private SpriteRenderer m_LeftArm;
    [SerializeField]
    private SpriteRenderer m_LeftEye;
    [SerializeField]
    private SpriteRenderer m_RightEye;
    #endregion

    #region Public Methods

    public RunnerColours RndRunnerColor(uint id)
    {
        var colours = RunnerColours.GetValues(typeof(RunnerColours));
        return (RunnerColours) colours.GetValue(id % colours.Length);
    }

    public void SetColor(RunnerColours runnerColour)
    {
        m_Body.sprite = getRunnerSprite(runnerColour, "Body");
        m_LeftEye.sprite = getRunnerSprite(runnerColour, "Eye");
        m_RightEye.sprite = getRunnerSprite(runnerColour, "Eye");
        m_LeftArm.sprite = getRunnerSprite(runnerColour, "Left Arm");
        m_LeftFoot.sprite = getRunnerSprite(runnerColour, "Left Foot");
        m_LeftHand.sprite = getRunnerSprite(runnerColour, "Left Hand");
        m_RightArm.sprite = getRunnerSprite(runnerColour, "Right Arm");
        m_RightFoot.sprite = getRunnerSprite(runnerColour, "Right Foot");
        m_RightHand.sprite = getRunnerSprite(runnerColour, "Right Hand");
    }

    #endregion

    private Sprite getRunnerSprite(RunnerColours runnerColour, string bodyPart)
    {
        return Resources.Load<Sprite>("Sprites/Characters/" + runnerColour + "/" + bodyPart) ;
    }
}

// /Users/georgeeisa/Code/FYDP/RedRunner/Assets/Sprites/RedRunner/Characters/Pink/Left Arm.png