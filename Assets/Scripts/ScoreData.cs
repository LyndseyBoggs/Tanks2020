using System; //Allows us to use IComparable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //make it visible in the editor

public class ScoreData : IComparable<ScoreData>
{
    public string name;     //name of the score holder
    public float score;     //score value

    //Constructor
    public ScoreData(string Name, float Score)
    {
        name = Name;
        score = Score;
    }



    //Compares one score to another
    public int CompareTo(ScoreData other)
    {
        //If other score does not exist
        if (other == null)
        {
            return 1;
        }

        //if this score is greater than other
        if (this.score > other.score)
        {
            return 1;
        }

        //if this score is less than other
        if (this.score < other.score)
        {
            return -1;
        }

        //else, they are equal
        return 0;
    }



}
