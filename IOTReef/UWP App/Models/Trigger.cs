using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_App.Models
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

        public override string ToString()
        {
            string sentence;

            sentence = "When ";
            if(DataToCheck == TriggerData.TEMPERATURE)
            {
                sentence += "temperature is ";
            }
            else if (DataToCheck == TriggerData.PH)
            {
                sentence += "PH is ";
            }

            if(DataOperator == TriggerOperator.GREATERTHAN)
            {
                sentence += "greater than ";
            }
            else if (DataOperator == TriggerOperator.LESSTHAN)
            {
                sentence += "less than ";
            }

            sentence += Value + " ";

            if(ActionToTake == Actions.OUTLETON)
            {
                sentence += "turn on this outlet.";
            }
            else if(actionToTake == Actions.OUTLETOFF)
            {
                sentence += "turn off this outlet.";
            }
            return sentence;
            //return base.ToString();
        }
    }
}
