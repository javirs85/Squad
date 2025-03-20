using UnityEngine;

public interface iAlphaController
{
    public void SetAlphaPosition(float v);
    public void SetReferenceValue(float v);
    public float AlphaValue { get; set; }

    public void StartMathTraining();
    public void StartRelaxTraining();
    public void FreeRun();

}
