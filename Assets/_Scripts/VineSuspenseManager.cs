using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 TODO: 101723 There is a mem leak:
    - Currently have the queue process disabled; All vines are suspended at start;
    - Try handling the adding of vines and starting the queue mgr process from VineRoot and/or vine factory/ level gen;
*/

public class VineSuspenseManager : MonoBehaviour
{
    public float suspensionWaitSeconds = 5f; // how many seconds to wait between triggering suspend and executing it
    public bool isVineSuspended;
    LineRenderer lineRenderer;
    VineRoot vineRoot;

    bool isRestingStateReached; //has this vine achieved resting state
    public float timeUnsuspended = 0; // tracks the amount of time this vine has been in an unsuspended state;
    float timeSinceLastSuspend = 0; // tracks amount of time since last suspended (will be used to accumulate timeUnsuspended without needing Update)
    static int restingGroupSize = 10; // max nVines to allow to be unsuspended simultaneously
    static int restingInterval = 2; // nSeconds that each group will be allowed to be unsuspended
    static int minRestingTime = 10; // the amount of seconds that a vine will be unsuspended to be allowed to achieve resting state
    static int nAwaitingReply = 0; // n of instances we're awaiting to finish their resting interval
    public static List<VineSuspenseManager> VineSuspenseManagerInstancesRef = new List<VineSuspenseManager>(); // Holds ref to all vineSuspenseManager instances;
    public static List<VineSuspenseManager> vineRestingQueue = new List<VineSuspenseManager>(); // tracks which vines have not reached resting state (i.e. have not been unsuspended for the full initSuspenseRestingTime)
    static int lastVineRestingQueueCount = 0; // tracks count of initSuspenseList at the start of each call to BeginStaticRestingProcess;
    static bool isStaticRestingProcessRunning = false;

    void Awake()
    {
        vineRoot = GetComponent<VineRoot>();
        lineRenderer = GetComponent<LineRenderer>();
        // Add this instance to the static ref list;
        VineSuspenseManagerInstancesRef.Add(this);

    }

    void Start()
    {
        // If this vine intance is not visible at start, our vineRoot is attached and we haven't reached resting state
        if (!lineRenderer.isVisible && !isRestingStateReached)
        {
            // suspend self
            // SuspendSegments(); // * No longer need this; Will be handled via external call (from levelGen) to SuspendAllSegments on init 
            // add self to queue
            // AddToQueue(this); // ! Commented out; Only using old vinesuspense system until mem issue sorted
            // Debug.Log("Debug: On Start: Adding to queue");
        }
        // else
        // {
        //     Debug.Log("DEBUG: On Start: Unsuspended");
        // }

    }

    void OnApplicationQuit()
    {
        CancelInvoke();
        VineSuspenseManagerInstancesRef = null;
        vineRestingQueue = null;
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void OnDestroy()
    {
        CancelInvoke();
    }

    void AddToQueue(VineSuspenseManager vineSuspenseManager)
    {
        // Add item to queue
        vineRestingQueue.Add(vineSuspenseManager);
        // If the manager is not already running, initialize it
        if (!isStaticRestingProcessRunning)
        {
            StartCoroutine(InitRestingQueueManager());
        }
    }

    static IEnumerator InitRestingQueueManager()
    {
        //If process is already running, return; //TODO: may want to check if the count is diff first, if it is, we can restart the process
        if (isStaticRestingProcessRunning) { yield break; }
        Debug.Log("InitRestingQueueManager > Started");
        isStaticRestingProcessRunning = true;

        // If no items are in the list, return;
        if (vineRestingQueue.Count == 0)
        {
            isStaticRestingProcessRunning = false;
            yield break;
        }
        // wait for vines to init
        yield return new WaitForSeconds(1f); //TODO: if the wait doesn't work; we can init by just grabbing all instances by type instead of having them add themselves; would prefer not to do this.
        // Init list by sorting //TODO: this may not be useful here if we end up calling before a sufficient number of vines have added themselves to the list; If not, try delaying the init by a little bit.
        float playerXpos = GameManager.GetPlayerRef().transform.position.x;
        vineRestingQueue.Sort((a, b) => //TODO: ensure sort works
        {
            float aDist = Mathf.Abs(a.transform.position.x - playerXpos);
            float bDist = Mathf.Abs(b.transform.position.x - playerXpos);
            return aDist > bDist ? 1 : -1;
        });

        Debug.Log("InitRestingQueueManager() > vineRestingQueue Count: " + vineRestingQueue.Count);

        // Select a group to continue ususpense
        int groupSize = Mathf.Min(vineRestingQueue.Count, restingGroupSize);
        Debug.Log("InitRestingQueueManager() > groupSize: " + groupSize);
        for (int i = 0; i < groupSize; i++)
        {
            SelectVineToUnsuspend();
        }
    }

    static void SelectVineToUnsuspend()
    { //TODO: there is a smarter way to select; should be some factor of distance to player and time already unsuspended
        //if the count is 0; return
        if (vineRestingQueue.Count == 0)
        {
            // if nAwaitingReply is also 0, the queue is complete and we can indicate the process is stopped;
            if (nAwaitingReply == 0)
            {
                isStaticRestingProcessRunning = false;
            }
            return;
        }
        // otherwise, select the next vine to unsuspend, and remove it from the queue // !Commented out to track down mem issue; not removing/adding inbetween replies
        // vineRestingQueue[0].ContinueTowardInitRestingState(restingInterval);
        // vineRestingQueue.RemoveAt(0);

        // select random
        VineSuspenseManager selected = RNG.RandomChoice<VineSuspenseManager>(vineRestingQueue);
        if (!selected.isVineSuspended) selected.ContinueTowardInitRestingState(restingInterval);
        else
        {
            SelectVineToUnsuspend();
        }
        //Add to nAwaitingReply
        nAwaitingReply++;
    }

    static void OnRestIntervalComplete(VineSuspenseManager vineSuspenseManagerInstance)
    {
        // called from vine instance when ContinueTowardRestingState is complete
        // update nAwaitingReply
        nAwaitingReply--;
        Debug.Log("OnRestIntervalComplete() > " + vineSuspenseManagerInstance.name + " timeUnsused: " + vineSuspenseManagerInstance.timeUnsuspended);
        // if rest not reached, requeue // ! commented out to track down mem problem; Not removing/adding inbetween replies;
        // if (vineSuspenseManagerInstance.timeUnsuspended < VineSuspenseManager.minRestingTime)
        // {
        //     vineRestingQueue.Add(vineSuspenseManagerInstance);
        // }

        // if resting time reached, remove from list; // ! Testing this instead of adding;
        // if (vineSuspenseManagerInstance.timeUnsuspended >= minRestingTime) // !Commented out mem issue test
        // {
        //     vineRestingQueue.Remove(vineSuspenseManagerInstance);
        // }
        // Select a new vine to Unsuspend
        SelectVineToUnsuspend();
    }

    public static void SuspendAllNotVisible()
    {
        foreach (VineSuspenseManager vineSuspenseManagerInstance in VineSuspenseManagerInstancesRef)
        {
            vineSuspenseManagerInstance.SuspendSegments();
        }
    }


    // ==== v1 (unfinished)
    // static IEnumerator ManageRestingQueue()
    // {//Called externally to begin the process of emptying the initSuspenseList


    //     // if list count > last count, then reinit the list (this is where you can sort by player distance or whatever as needed)
    //     if (vineRestingQueue.Count > lastVineRestingQueueCount)
    //     {
    //         float playerXpos = GameManager.GetPlayerRef().transform.position.x;
    //         //Reprep list; 
    //         vineRestingQueue.Sort((a, b) => //TODO: ensure sort works
    //         {
    //             float aDist = Mathf.Abs(a.transform.position.x - playerXpos);
    //             float bDist = Mathf.Abs(b.transform.position.x - playerXpos);
    //             return aDist > bDist ? 1 : -1;
    //         });
    //         // Update last count
    //         lastVineRestingQueueCount = vineRestingQueue.Count;
    //     }

    //     // pick n number of vines at random (or based on distance from player) to continue unsuspense
    //     // run continue resting process on each;
    //     int groupSize = Mathf.Max(vineRestingQueue.Count, restingGroupSize);
    //     for(int i=0; i<groupSize; i++) {
    //         vineRestingQueue[i].ContinueTowardInitRestingState(restingInterval);
    //     }
    // }

    void OnBecameVisible()
    {
        // Debug.Log("VineRoot.OnBecameVisible() name: " + name);
        UnsuspendSegments();
    }

    void OnBecameInvisible()
    {
        // Debug.Log("VineRoot.OnBecameInvisible() name: " + name);
        if (vineRoot.segmentRbs != null) SuspendAfterSeconds(suspensionWaitSeconds);
    }

    public void SuspendAfterSeconds(float nSeconds)
    {
        Invoke("SuspendSegments", nSeconds);
    }

    public void UnsuspendAfterSeconds(float nSeconds)
    {
        Invoke("UnsuspendSegments", nSeconds);
    }

    void SuspendSegments()
    {// Method2 controls rb simulated flag; operation must be applied from bottom segment upward to avoid stretching issue.
        if (isVineSuspended || lineRenderer.isVisible) { return; }

        // Suspend segments;
        for (int i = vineRoot.nSegments - 1; i > 0; i--) { vineRoot.segmentRbs[i].simulated = false; }
        isVineSuspended = true;
        // // If resting state not yet reached:
        // if (!isRestingStateReached)
        // {
        //     // Accumulate the time spent unsuspended
        //     timeUnsuspended += Time.realtimeSinceStartup - timeSinceLastSuspend;
        //     // ReCheck if resting state has been reached
        //     CheckIfRestingStateHasBeenReached();
        // }
    }

    void UnsuspendSegments()
    {// Method2 controls rb simulated flag; operation must be applied from bottom segment upward to avoid stretching issue.
        CancelInvoke(); //stop any existing SuspendSegmentCalls
        if (!isVineSuspended || vineRoot.segmentRbs == null) return; // * Note: checking that segments are not null since this was running when play ended, causing error.
        // // Log the time when the suspension ended;
        // timeSinceLastSuspend = Time.realtimeSinceStartup;
        // Unsuspend segments;
        for (int i = vineRoot.nSegments - 1; i > 0; i--) { vineRoot.segmentRbs[i].simulated = true; }
        // Update suspended status;
        isVineSuspended = false;
    }

    public void ContinueTowardInitRestingState(float nSeconds)
    { // ! NOTE: the mem issue seems to appear when the OnRestIntervalComplete is called;
        // Used during Initial unsuspension to allow vine to get to resting state intermittently;
        // if we're not suspended (e.g. already on screen), skip this interval and return.
        Debug.Log("ContinueTowardInitRestingState() > " + name);
        // if (!isVineSuspended) // ! commented out to debug mem issue
        // {
        //     Debug.Log("ContinueTowardInitRestingState() > " + name + " | Already Visible; returning...");
        //     // VineSuspenseManager.OnRestIntervalComplete(this); // ! commented out to debug mem issue
        //     return;
        // }
        // Unsuspend the segments
        UnsuspendSegments();
        // Invoke a call to resuspend after nSeconds
        SuspendAfterSeconds(nSeconds);
        // Update the time spent Unsuspended
        timeUnsuspended += nSeconds;
        // Call OnRestIntervalComplete
        // VineSuspenseManager.OnRestIntervalComplete(this); // ! Commented out to debug mem issue
        // if the vine's total unsuspended time is greater than the resting time, then remove self from the static initSuspenseList
        CheckIfRestingStateHasBeenReached(); // ! testing the local method handling removal
        // SelectVineToUnsuspend(); // ! Testing calling this from here/ avoiding OnRestIntervalComplte

    }

    void CheckIfRestingStateHasBeenReached()
    {
        // If this vine instance has reached minRestingTime, remove it from the initSuspenseList
        if (timeUnsuspended >= minRestingTime)
        {
            VineSuspenseManager.vineRestingQueue.Remove(this);
            isRestingStateReached = true;
        }
    }
}
