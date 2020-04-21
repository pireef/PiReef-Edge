namespace IOTReefLib.Circuits
{
    public class EZOrtd : EZOBase
    {
        public EZOrtd(int address) : base(address)
        {

        }

        public EZOrtd() : base(0x66)
        {
            //default address of the circuit
        }

        public void TakeReading()
        {
            string command = "R";
            ExecuteCommand(command, 700);
        }

        public void SetFarenheit()
        {
            string command = "S,f";
            ExecuteCommand(command, 500);
        }

        public void SetCelsius()
        {
            string command = "S,c";
            ExecuteCommand(command);
        }

        public void SetKelvin()
        {
            string command = "S,k";
            ExecuteCommand(command);
        }

        //public void Status()
        //{
        //    string command = "Status";
        //    ExecuteCommand(command);
        //}
    }
}
