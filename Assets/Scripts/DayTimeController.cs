using PlateauToolkit.Rendering;
using UnityEngine;

[RequireComponent(typeof(EnvironmentController))]
public class DayTimeController : MonoBehaviour
{
    public bool m_DayTimeIsDynamic;
    public EnvironmentController m_EnvironmentController;
    public float speed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        m_EnvironmentController = GetComponent<EnvironmentController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_DayTimeIsDynamic)
        {
            m_EnvironmentController.m_TimeOfDay += speed * Time.deltaTime;
            m_EnvironmentController.m_TimeOfDay %= 1f; // Ensure time of day wraps around
        }
    }
}