namespace IOTReef_HubModule.Models
{
    public enum Actions
    {
        OUTLETON,
        OUTLETOFF,
        EMAIL,
        TEXTMESSAGE
    }

    public enum TriggerData
    {
        TEMPERATURE,
        PH
    }

    public enum TriggerOperator
    {
        GREATERTHAN,
        LESSTHAN
    }

    public class Trigger
    {
        TriggerData dataToCheck;
        TriggerOperator dataOperator;
        Actions actionToTake;
        string value;

        public string Value { get => value; set => this.value = value; }
        public Actions ActionToTake { get => actionToTake; set => actionToTake = value; }
        public TriggerOperator DataOperator { get => dataOperator; set => dataOperator = value; }
        public TriggerData DataToCheck { get => dataToCheck; set => dataToCheck = value; }
    }
}
