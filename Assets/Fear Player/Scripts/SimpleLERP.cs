using UnityEngine;
using UnityEngine.Events;

public class SimpleLERP : MonoBehaviour
{
    public Transform start;
    public Transform end;
    public float speed = 1.0f;
    public bool move = false;

    private float startTime;
    private float lenght;

    public UnityEvent m_endMove;

    private void Start()
    {
        startTime = 0;
        lenght = Vector3.Distance(start.position, end.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!move)
            return;

        if (startTime == 0)
            startTime = Time.time;

        Move();
    }

    private void Move()
    {
        float distCovered = (Time.time - startTime) / speed;
        float fractionOfJourney = distCovered / lenght;
        transform.position = Vector3.Lerp(start.position, end.position, fractionOfJourney);

        if( transform.position == end.position )
        {
            Destroy(this.gameObject);
            m_endMove.Invoke();
        }
    }

    public void StartMove ()
    {
        move = true;
    }
}
