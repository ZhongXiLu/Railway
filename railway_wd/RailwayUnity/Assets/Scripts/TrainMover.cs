using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
TrainMover: class responsible for physically moving the train on the tracks.
*/
public class TrainMover : MonoBehaviour {


    /**
    Move an object to a new position over a duration.

    @param objectToMove     The object to move.
    @param newRotation      The new position of the object.
    @param duration         The duration of the transformation in seconds.

    Taken from: http://answers.unity.com/answers/1146981/view.html
    */
    IEnumerator moveOverSeconds(GameObject objectToMove, Vector3 newPosition, float duration) {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while(elapsedTime < duration) {
            if(objectToMove != null) {
                objectToMove.transform.position = Vector3.Lerp(startingPos, newPosition, (elapsedTime / duration));
            }
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if(objectToMove != null) {
            objectToMove.transform.position = newPosition;
        }
    }

    /**
    Rotate an object to a new angle over a duration.

    @param objectToMove     The object to rotate.
    @param newRotation      The new angle of the object.
    @param duration         The duration of the transformation in seconds.

    Taken from: https://stackoverflow.com/a/37588536
    */
    IEnumerator rotateOverSeconds(GameObject objectToMove, Quaternion newRotation, float duration) {
        Quaternion currentRot = objectToMove.transform.rotation;

        float counter = 0;
        while(counter < duration) {
            counter += Time.deltaTime;
            if(objectToMove != null) {
                objectToMove.transform.rotation = Quaternion.Lerp(currentRot, newRotation, counter / duration);
            }
            yield return null;
        }
    }

    /**
    Determine if the train is aligned with a track.

    @param train The train.
    @param track The track.
    @return True if the train and the track are aligned.
    */
    bool isAligned(GameObject train, GameObject track) {
        // TODO: fix?
        // Sometimes train and track are not unit-perfectly aligned, so add a margin of 15 units.
        return (train.transform.position.x - track.transform.position.x) < 15;
    }

    /**
    Does not actually move the train (i.e. change the transform.position), it only makes sure 
    the train's angle is set accordingly to the (new) track's angle. For example, when entering 
    the diagonal track of a junction, we have to rotate the train.

    @param trainId  The id of the train.
    @param toTrack  The id of the track the train is moving to.
    */
    public void moveTrainToTrack(string trainId, string toTrack, string schedule) {
        GameObject train = GameObject.Find("Train_" + trainId);
        GameObject track = GameObject.Find(toTrack);

        Quaternion rotation;
        if(!isAligned(train, track)) {
            // Rotate accordingly
            // TODO: check for crossing
            rotation = Quaternion.Euler(new Vector3(0, -20, 0));
        } else {
            rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        StartCoroutine(rotateOverSeconds(train, rotation, 3f));

        train.GetComponent<Train>().currentTrack = toTrack;
        train.GetComponent<Train>().train.schedule = schedule;
    }

    /**
    Move the train to the 1km mark on its current track.
    (The train can see the lights of the next track starting from this 1km mark)

    @param trainId          The id of the train.
    @param trainSchedule    The schedule of the train (so we can determine if a train has to turn or not on a split).
    @param currentTrack     The id of the track the train is currently on.
    @param trackType        The type of track the train is currently on.
    @param duration         The duration of the movement in seconds.
    */
    public void moveTrainTo1KmMark(string trainId, string trainSchedule, string currentTrack, string trackType, float duration) {
        if(duration > 5.0f) {
            GameObject train = GameObject.Find("Train_" + trainId);
            GameObject track = GameObject.Find(currentTrack);

            Vector3 newPosition = track.transform.position;
            // TODO: check for crossing
            newPosition.z += (3*80/5)-10;

            string[] schedule = trainSchedule.Split(',');
            if(!isAligned(train, track) && (trackType == "Join" || trackType == "Crossing")) {
                // TODO: check for crossing
                newPosition.x += 12;
            } else if((trackType == "Split" || trackType == "Crossing") && schedule.Length > 0 && schedule[0] == "TURN") {
                newPosition.x += 18;    // TODO:check
                StartCoroutine(rotateOverSeconds(train, Quaternion.Euler(new Vector3(0, 20, 0)), 3f));
            }

            StartCoroutine(moveOverSeconds(train, newPosition, duration));
        }
    }

    /**
    Move the train to the end of its current track.

    @param trainId          The id of the train.
    @param currentTrack     The id of the track the train is currently on.
    @param duration         The duration of the movement in seconds.
    */
    public void moveTrainToEnd(string trainId, string currentTrack, float duration) {
        if(duration > 5.0f) {
            GameObject train = GameObject.Find("Train_" + trainId);
            GameObject track = GameObject.Find(currentTrack);

            Vector3 newPosition = track.transform.position;
            // TODO: check for crossing
            newPosition.z += 70;
            StartCoroutine(moveOverSeconds(train, newPosition, duration));
        }
    }

}
