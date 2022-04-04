using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private GameResourceManager _gm;

    public int CurrentTurn { get; private set; }

    public int CurrentTurnAsYear
    {
        get
        {
            return (2050 + CurrentTurn);
        }
    }

    public int TurnsUntilGameEndAsYear
    {
        get
        {
            int yearsRemaining = Mathf.Min(_gm.BiodiversityYearsRemaining, _gm.SeaLevelsYearsRemaining);
            return yearsRemaining;
        }
    }

    private ClimateEventManager climateEventManager;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.FindObjectOfType<GameResourceManager>();
        climateEventManager = GameObject.FindObjectOfType<ClimateEventManager>();

        StartGame();
    }

    private void StartGame()
    {
        _gm.CalculateResources();
        climateEventManager.StepClimateState(CurrentTurn);
        PublishBeginTurnNotification();
    }

    private void ProcessEndOfTurnEvents()
    {

    }

    private bool EndGameConditionIsReached()
    {
        return false;
    }

    private void EndGame()
    {

    }

    private void AdvanceToNextTurn()
    {
        climateEventManager.StepClimateState(CurrentTurn);

        CurrentTurn++;

        _gm.CalculateResources();

        userDidHandleEvent = false;
        PublishBeginTurnNotification();
        CheckAndSendValidNextTurnEvents();
    }

    public void EndTurn()
    {
        ProcessEndOfTurnEvents();

        if (EndGameConditionIsReached())
        {
            EndGame();
        }
        else
        {
            AdvanceToNextTurn();
        }
    }

    public void CheckAndSendValidNextTurnEvents() {

        if (!IsCurrentEventHandled()) {
            PublishForbidNextTurnNotification("Current Event not Handled");
            return;
        }

        if (!_gm.ResourceManagerApprovesNextTurn()) {
            PublishForbidNextTurnNotification("At least one resource over utilized!");
            return;
        }

        PublishAllowNextTurnNotification();
    }

    public void PublishBeginTurnNotification() {
        GetComponent<PubSubSender>().Publish("turn.begin");
    }

    public void PublishAllowNextTurnNotification() {
        Debug.Log("Next Turn Allowed!");
        GetComponent<PubSubSender>().Publish("turn.next.allow");
    }

    public void PublishForbidNextTurnNotification(string reason) {
        Debug.Log("Next Turn Forbidden:" + reason);
        GetComponent<PubSubSender>().Publish("turn.next.forbidden", reason);
    }

    private bool userDidHandleEvent = false;
    public bool IsCurrentEventHandled() {
        if (climateEventManager.CurrentClimateEvent == null) { return true; }
        if (climateEventManager.CurrentClimateEvent.Responses.Count == 0) { return true; }
        return userDidHandleEvent;
    }

    public void UserDidHandleEvent() {
        userDidHandleEvent = true;
        CheckAndSendValidNextTurnEvents();
    }

    public void ResourcesChanged() {
        CheckAndSendValidNextTurnEvents();
    }

}
