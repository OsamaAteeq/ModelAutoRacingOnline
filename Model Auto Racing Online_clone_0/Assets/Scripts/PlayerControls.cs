using UnityEngine;  //library of unity engine that is included in game
using System.Collections; // this library contains interferences and classes such as list ,queue etc

[RequireComponent(typeof(Motor))] // require component motor is required for this script to run , it is mendatory for it
public class PlayerControls : MonoBehaviour {

    public int selectedcamera = 0;  // selectedcamre int value is set to 0 
    public cameraMotionScript cam; // cam varaible of cameraMotionScript is defined here from cameraMotion script .

    private bool free = true; // free bool varaible is set to true

    private bool enab = true; //enab bool varaible  is also set to true

    void FixedUpdate() // fixed update is called when the game is running (it is basically used for the physics in game)
    {

        Motor motor = GetComponent<Motor>();  // it gets the motor component in the script

        if (enab && motor.tim.startrace) //enab bool and motor.time.startrace is true the game will start
        {

            float accel = Input.GetAxis("Vertical"); //get the vertical input that is z axis values
            float turn = Input.GetAxis("Horizontal"); // gets the horizontal input that is x axis values
            float brake = Input.GetAxis("Jump"); // when the space bar is pressed brake is applied to the car that is moving 

            motor.Move(turn, accel, 0f, 0f); // value of turn and acceleration are transferd to the move fucntion values
            motor.HandBrake(Input.GetAxis("Fire1")); //if fire one button is pressed hardbrake will be applied to the car that is moving 

        }
        else
        {
            motor.Move(0, 0, 1f, 0f); // if the condition upper if condition failes motor.move fuction is called here  
        }

        if (Input.GetAxis("Camera") == 0) // if the camera position is equaled to zero 
        {
            free = true; // free boolean is set to true the camera starts following player
         
        }
        if (free && Input.GetAxis("Camera") == 1) // if the camera position is equaled to one
        {
            free = false; // free is false
            selectedcamera++; // camera varaible is incremented
        }
        if (free && Input.GetAxis("Camera") == -1) // if values of free and camers goes to negetive
        {
            free = false; //free is false
            selectedcamera--; // camera varaible is decremented by 1;
        }
        if (selectedcamera == 4) // if camera value == 4 .
        {
            selectedcamera = 0; // set the value to 0.
        }
        if (selectedcamera == -1) // ifcamera value == -1.
        {
            selectedcamera = 3; // selected camera varaible is set to -3.
        }

        switch (selectedcamera) // this is the camera control switch in which the camera distance ,heigth and mode is set
        {
            case 0: //when value is zero of the selected camera int , these lines of code are called
                cam.distance = 15;
                cam.height = 10;
                cam.mode = 0;
                break;
            case 1: // if value is one
                cam.mode = 1; // camera mode is set to one
                break;
            case 2: //if value is two this case is called with the following values
                cam.distance = 5;
                cam.height = 50;
                cam.mode = 0;
                break;
            case 3:  // if value is three
                cam.mode = 2; // camera mod is set to two
                break;
        }

    }

    public void finished() // when the level is finished this function is called
    {
        enab = false; // enab is true 
        cam.mode = 3; // camera mode is set to 3
    }

}

