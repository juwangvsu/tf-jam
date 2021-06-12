using UnityEngine;

public class ForceModeExample : MonoBehaviour
{
    public Transform TransformGoal;
    public Transform TransformAim;
    //Use to switch between Force Modes
    enum ModeSwitching { Start, Impulse, Acceleration, Force, VelocityChange , Idle, Velshot};
    ModeSwitching m_ModeSwitching;

    Vector3 m_StartPos, m_StartForce;
    Vector3 m_NewForce;
    Rigidbody m_Rigidbody;

    string m_ForceXString = string.Empty;
    string m_ForceYString = string.Empty;

    float m_ForceX, m_ForceY;
    float m_Result;

    void Start()
    {
        //You get the Rigidbody component you attach to the GameObject
        m_Rigidbody = GetComponent<Rigidbody>();

        //This starts at first mode (nothing happening yet)
        m_ModeSwitching = ModeSwitching.Start;

        //Initialising the force which is used on GameObject in various ways
        m_NewForce = new Vector3(-5.0f, 1.0f, 0.0f);

        //Initialising floats
        m_ForceX = 0;
        m_ForceY = 0;

        //The forces typed in from the text fields (the ones you can manipulate in Game view)
        m_ForceXString = "0";
        m_ForceYString = "0";

        //The GameObject's starting position and Rigidbody position
        m_StartPos = transform.position;
        m_StartForce = m_Rigidbody.transform.position;
    }

    void FixedUpdate()
    {
                 var gv2 = new Vector2(
                TransformGoal.position.x,
                TransformGoal.position.z);
        var gv3 = new Vector3(
                TransformGoal.position.x,
                TransformGoal.position.y,TransformGoal.position.z);

                var tv2 = new Vector2(
                    transform.position.x, transform.position.z);
        var tv3 = new Vector3(
                    transform.position.x, transform.position.y, transform.position.z);

                var dir = (gv2 - tv2).normalized;
                var dist = (gv2 - tv2).magnitude;
        if (Mathf.Abs(m_Rigidbody.velocity.y) >0.00001 || Mathf.Abs(m_Rigidbody.velocity.x) > 0.00001)
        {
            Debug.Log(m_Rigidbody.velocity.y > 0);
            Debug.Log(m_Rigidbody.velocity.y.ToString("F5"));
            Debug.Log(m_Rigidbody.velocity.x > 0);
            Debug.Log("velocity: "+ m_Rigidbody.velocity.ToString("F5") + " pos " + m_Rigidbody.transform.position.ToString("F5") );
        }
            
        //If the current mode is not the starting mode (or the GameObject is not reset), the force can change
        if (m_ModeSwitching != ModeSwitching.Start)
        {
            //The force changes depending what you input into the text fields
            m_NewForce = new Vector3(m_ForceX, m_ForceY, 0);
        }
        MakeCustomForce();
        //Here, switching modes depend on button presses in the Game mode
        switch (m_ModeSwitching)
        {
            //This is the starting mode which resets the GameObject
            case ModeSwitching.Start:
                //This resets the GameObject and Rigidbody to their starting positions
                transform.position = m_StartPos;
                m_Rigidbody.transform.position = m_StartForce;

                //This resets the velocity of the Rigidbody
                m_Rigidbody.velocity = new Vector3(0f, 0f, 0f);
                m_ModeSwitching = ModeSwitching.Idle;
                break;

            //These are the modes ForceMode can force on a Rigidbody
            //This is Acceleration mode
            case ModeSwitching.Acceleration:
                //The function converts the text fields into floats and updates the Rigidbody’s force
                //MakeCustomForce();
                m_NewForce = new Vector3(m_ForceX, m_ForceY, 0);
                //Use Acceleration as the force on the Rigidbody
                Debug.Log("apply acceleration force" + m_NewForce);
                m_Rigidbody.AddForce(m_NewForce, ForceMode.Acceleration);

                break;

            //This is Force Mode, using a continuous force on the Rigidbody considering its mass
            case ModeSwitching.Force:
                //Converts the text fields into floats and updates the force applied to the Rigidbody
                //MakeCustomForce();
                m_NewForce = new Vector3(m_ForceX, m_ForceY, 0);
                //Use Force as the force on GameObject’s Rigidbody
                Debug.Log("apply force" + m_NewForce);
                m_Rigidbody.AddForce(m_NewForce, ForceMode.Force);
                break;

            //This is Impulse Mode, which involves using the Rigidbody’s mass to apply an instant impulse force.
            case ModeSwitching.Impulse:
                //The function converts the text fields into floats and updates the force applied to the Rigidbody
               // MakeCustomForce();
                m_NewForce = new Vector3(m_ForceX, m_ForceY, 0);
                //Use Impulse as the force on GameObject
                Debug.Log("apply impulse force" + m_NewForce);
                m_Rigidbody.AddForce(m_NewForce, ForceMode.Impulse);
                m_ModeSwitching = ModeSwitching.Idle;
                break;

            //This is VelocityChange which involves ignoring the mass of the GameObject and impacting it with a sudden speed change in a direction
            case ModeSwitching.VelocityChange:
                //Converts the text fields into floats and updates the force applied to the Rigidbody
                //MakeCustomForce();
                m_NewForce = new Vector3(m_ForceX, m_ForceY, 0);
                //Make a Velocity change on the Rigidbody
                m_Rigidbody.AddForce(m_NewForce, ForceMode.VelocityChange);
                m_ModeSwitching = ModeSwitching.Idle;
                break;
            case ModeSwitching.Velshot:
                //dist	vx	deltay	t=dist/vx	vy=deltay+5t^2/t
                var vx = 5;
                var deltay = gv3.y - tv3.y;
                Debug.Log("deltay: "+deltay +"," + "gv3.y " +  gv3.y  + ", tv3.y " + tv3.y);
                var t = dist / vx;
                var vy = (deltay + 5 * Mathf.Pow(t , 2)) / t;
                Debug.Log("vs, deltay, t, vy " + vx + "," + deltay + "," + t + "," + vy);
                Vector3 newvel = new Vector3(
                vx, //m_ForceX, //dir.x ,
                vy, //m_ForceY,  //dist*m_ForceY,dist*10/(2*dir.x)
                dir.y
                );
                Debug.Log("apply velocity: " + newvel +" dist: "+ dist +" goal " + gv3 + " mypos " + tv3);
                m_Rigidbody.AddForce(newvel, ForceMode.VelocityChange);
                m_ModeSwitching = ModeSwitching.Idle;
                break;
            case ModeSwitching.Idle:
                break;
        }
    }

    //The function outputs buttons, text fields, and other interactable UI elements to the Scene in Game view
    void OnGUI()
    {
        //Getting the inputs from each text field and storing them as strings
        m_ForceXString = GUI.TextField(new Rect(300, 10, 200, 20), m_ForceXString, 25);
        m_ForceYString = GUI.TextField(new Rect(300, 30, 200, 20), m_ForceYString, 25);

        //Press the button to reset the GameObject and Rigidbody
        if (GUI.Button(new Rect(100, 0, 150, 30), "Reset"))
        {
            //This switches to the start/reset case
            m_ModeSwitching = ModeSwitching.Start;
        }

        //When you press the Acceleration button, switch to Acceleration mode
        if (GUI.Button(new Rect(100, 30, 150, 30), "Apply Acceleration"))
        {
            //Switch to Acceleration (apply acceleration force to GameObject)
            m_ModeSwitching = ModeSwitching.Acceleration;
        }

        //If you press the Impulse button
        if (GUI.Button(new Rect(100, 60, 150, 30), "Apply Impulse"))
        {
            //Switch to impulse (apply impulse forces to GameObject)
            m_ModeSwitching = ModeSwitching.Impulse;
        }

        //If you press the Force Button, switch to Force state
        if (GUI.Button(new Rect(100, 90, 150, 30), "Apply Force"))
        {
            //Switch to Force (apply force to GameObject)
            m_ModeSwitching = ModeSwitching.Force;
        }

        //Press the button to switch to VelocityChange state
        if (GUI.Button(new Rect(100, 120, 150, 30), "Apply Velocity Change"))
        {
            //Switch to velocity changing
            m_ModeSwitching = ModeSwitching.VelocityChange;
        }
        if (GUI.Button(new Rect(100, 150, 150, 30), "Apply Velocity shot"))
        {
            //Switch to velocity changing
            m_ModeSwitching = ModeSwitching.Velshot;
        }
        if (GUI.Button(new Rect(100, 180, 150, 30), "Relocate"))
        { 
            MoveToRandomDistance();
            GetComponent<Renderer>().material = MaterialDefault ;
        }
    }

    //Changing strings to floats for the forces
    float ConvertToFloat(string Name)
    {
        float.TryParse(Name, out m_Result);
        return m_Result;
    }

    //Set the converted float from the text fields as the forces to apply to the Rigidbody
    void MakeCustomForce()
    {
        //This converts the strings to floats
        m_ForceX = ConvertToFloat(m_ForceXString);
        m_ForceY = ConvertToFloat(m_ForceYString);
    }
    public bool hasTriggeredTop = false;
    public bool hasBeenScored = false;
    public static int SuccessCount = 0;
    public static int ShotCount = 1;
    public Material MaterialBallScored;
    public Material MaterialDefault;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "TriggerTop")
        {
            Debug.Log("Top Scored... change color....");
            hasTriggeredTop = true;
        }
        else if (other.name == "TriggerBottom")
        {
            if (hasTriggeredTop && !hasBeenScored)
            {
                GetComponent<Renderer>().material = MaterialBallScored;
                Debug.Log("Bottom Scored... change color....");
               
            }
            hasBeenScored = true;
        }
    }
    void MoveToRandomDistance()
    {
        var newPosition = new Vector3(TransformGoal.position.x + Random.Range(-23f, -2.5f), transform.position.y, TransformGoal.position.z);
        transform.position = newPosition;
        hasBeenScored = false;
    }
}